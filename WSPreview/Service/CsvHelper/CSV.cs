﻿/*
 * 2006 - 2012 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://code.google.com/p/csharp-csv-reader/
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace WSPreview.PreviewHandler.Service.CsvHelper
{
    public static class CSV
    {
        private static readonly char[] Separators = new char[] {',',' ','|',';','\t',':'};

        public const char DEFAULT_DELIMITER = ',';
        public const char DEFAULT_QUALIFIER = '"';
        public const char DEFAULT_TAB_DELIMITER = '\t';

        #region Reading CSV Formatted Data

        public static char GetSeparator(string line)
        {
            char sep = DEFAULT_DELIMITER;
            foreach (var separator in Separators)
            {
                var arr = ParseLine(line, separator);
                if (arr != null && arr.Length > 0)
                {
                    sep = separator;
                    break;
                }
            }
            return sep;
        }


        /// <summary>
        /// Parse a line whose values may include newline symbols or CR/LF
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static string[] ParseMultiLine(StreamReader sr, char delimiter = DEFAULT_DELIMITER, char text_qualifier = DEFAULT_QUALIFIER)
        {
            StringBuilder sb = new StringBuilder();
            string[] array = null;
            while (!sr.EndOfStream) {

                // Read in a line
                sb.Append(sr.ReadLine());

                // Does it parse?
                string s = sb.ToString();
                if (TryParseLine(s, delimiter, text_qualifier, out array)) {
                    return array;
                }

                // We didn't succeed on the first try - our line must have an embedded newline in it
                sb.Append("\n");
            }

            // Fails to parse - return the best array we were able to get
            return array;
        }

        /// <summary>
        /// Parse the line and return the array if it succeeds, or as best as we can get
        /// </summary>
        /// <param name="s"></param>
        /// <param name="delimiter"></param>
        /// <param name="text_qualifier"></param>
        /// <returns></returns>
        public static string[] ParseLine(string s, char delimiter = DEFAULT_DELIMITER, char text_qualifier = DEFAULT_QUALIFIER)
        {
            string[] array = null;
            TryParseLine(s, delimiter, text_qualifier, out array);
            return array;
        }

        /// <summary>
        /// Read in a line of text, and use the Add() function to add these items to the current CSV structure
        /// </summary>
        /// <param name="s"></param>
        public static bool TryParseLine(string s, char delimiter, char text_qualifier, out string[] array)
        {
            bool success = true;
            List<string> list = new List<string>();
            StringBuilder work = new StringBuilder();
            for (int i = 0; i < s.Length; i++) {
                char c = s[i];

                // If we are starting a new field, is this field text qualified?
                if ((c == text_qualifier) && (work.Length == 0)) {
                    int p2;
                    while (true) {
                        p2 = s.IndexOf(text_qualifier, i + 1);

                        // for some reason, this text qualifier is broken
                        if (p2 < 0) {
                            work.Append(s.Substring(i + 1));
                            i = s.Length;
                            success = false;
                            break;
                        }

                        // Append this qualified string
                        work.Append(s.Substring(i + 1, p2 - i - 1));
                        i = p2;

                        // If this is a double quote, keep going!
                        if (((p2 + 1) < s.Length) && (s[p2 + 1] == text_qualifier)) {
                            work.Append(text_qualifier);
                            i++;

                            // otherwise, this is a single qualifier, we're done
                        } else {
                            break;
                        }
                    }

                    // Does this start a new field?
                } else if (c == delimiter) {
                    list.Add(work.ToString());
                    work.Length = 0;

                    // Test for special case: when the user has written a casual comma, space, and text qualifier, skip the space
                    // Checks if the second parameter of the if statement will pass through successfully
                    // e.g. "bob", "mary", "bill"
                    if (i + 2 <= s.Length - 1) {
                        if (s[i + 1].Equals(' ') && s[i + 2].Equals(text_qualifier)) {
                            i++;
                        }
                    }
                } else {
                    work.Append(c);
                }
            }
            list.Add(work.ToString());

            // If we have nothing in the list, and it's possible that this might be a tab delimited list, try that before giving up
            if (list.Count == 1 && delimiter != DEFAULT_TAB_DELIMITER) {
                string[] tab_delimited_array = ParseLine(s, DEFAULT_TAB_DELIMITER, DEFAULT_QUALIFIER);
                if (tab_delimited_array.Length > list.Count) {
                    array = tab_delimited_array;
                    return success;
                }
            }

            // Return the array we parsed
            array = list.ToArray();
            return success;
        }
        #endregion

        #region Output CSV formatted files from raw data
        /// <summary>
        /// Write a data table to disk at the designated file name in CSV format
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fn"></param>
        public static void SaveAsCSV(this DataTable dt, string filename, bool save_column_names, char delim = DEFAULT_DELIMITER, char qual = DEFAULT_QUALIFIER)
        {
            using (StreamWriter sw = new StreamWriter(filename)) {
                WriteToStream(dt, sw, save_column_names, delim, qual);
            }
        }

        /// <summary>
        /// Send this 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="from_address"></param>
        /// <param name="to_address"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="?"></param>
        public static void SendCsvAttachment(this DataTable dt, string from_address, string to_address, string subject, string body, string smtp_host, string attachment_filename)
        {
            // Save this CSV to a string
            string csv = WriteToString(dt, true);

            // Prepare the email message and attachment
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add(to_address);
            message.Subject = subject;
            message.From = new System.Net.Mail.MailAddress(from_address);
            message.Body = body;
            System.Net.Mail.Attachment a = System.Net.Mail.Attachment.CreateAttachmentFromString(csv, "text/csv");
            a.Name = attachment_filename;
            message.Attachments.Add(a);

            // Send the email
            using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtp_host)) {
                smtp.Send(message);
            }
        }

        /// <summary>
        /// Write the data table to a stream in CSV format
        /// </summary>
        /// <param name="dt">The data table to write</param>
        /// <param name="sw">The stream where the CSV text will be written</param>
        /// <param name="save_column_names">True if you wish the first line of the file to have column names</param>
        /// <param name="delim">The delimiter (comma, tab, pipe, etc) to separate fields</param>
        /// <param name="qual">The text qualifier (double-quote) that encapsulates fields that include delimiters</param>
        public static void WriteToStream(this DataTable dt, StreamWriter sw, bool save_column_names, char delim = DEFAULT_DELIMITER, char qual = DEFAULT_QUALIFIER)
        {
            using (CSVWriter cw = new CSVWriter(sw, delim, qual)) {
                cw.Write(dt, save_column_names);
            }
        }

        /// <summary>
        /// Serialize an object array to a stream in CSV format
        /// </summary>
        /// <param name="list">The object array to write</param>
        /// <param name="sw">The stream where the CSV text will be written</param>
        /// <param name="save_column_names">True if you wish the first line of the file to have column names</param>
        /// <param name="delim">The delimiter (comma, tab, pipe, etc) to separate fields</param>
        /// <param name="qual">The text qualifier (double-quote) that encapsulates fields that include delimiters</param>
        public static void WriteToStream<T>(this IEnumerable<T> list, StreamWriter sw, bool save_column_names, char delim = DEFAULT_DELIMITER, char qual = DEFAULT_QUALIFIER)
        {
            using (CSVWriter cw = new CSVWriter(sw, delim, qual)) {
                cw.Write(list, save_column_names);
            }
        }

        /// <summary>
        /// Serialize an object array to a stream in CSV format
        /// </summary>
        /// <param name="list">The object array to write</param>
        /// <param name="sw">The stream where the CSV text will be written</param>
        /// <param name="save_column_names">True if you wish the first line of the file to have column names</param>
        /// <param name="delim">The delimiter (comma, tab, pipe, etc) to separate fields</param>
        /// <param name="qual">The text qualifier (double-quote) that encapsulates fields that include delimiters</param>
        public static void WriteToStream<T>(this IEnumerable<T> list, string filename, bool save_column_names, char delim = DEFAULT_DELIMITER, char qual = DEFAULT_QUALIFIER)
        {
            using (StreamWriter sw = new StreamWriter(filename)) {
                WriteToStream<T>(list, sw, save_column_names, delim, qual);
            }
        }

        /// <summary>
        /// Write a DataTable to a string in CSV format
        /// </summary>
        /// <param name="dt">The datatable to write</param>
        /// <param name="sw">The stream where the CSV text will be written</param>
        /// <param name="save_column_names">True if you wish the first line of the file to have column names</param>
        /// <param name="delim">The delimiter (comma, tab, pipe, etc) to separate fields</param>
        /// <param name="qual">The text qualifier (double-quote) that encapsulates fields that include delimiters</param>
        /// <returns>The CSV string representing the object array.</returns>
        public static string WriteToString(this DataTable dt, bool save_column_names, char delim = DEFAULT_DELIMITER, char qual = DEFAULT_QUALIFIER)
        {
            using (var ms = new MemoryStream()) {
                var sw = new StreamWriter(ms);
                var cw = new CSVWriter(sw, delim, qual);
                cw.Write(dt, save_column_names);
                sw.Flush();
                ms.Position = 0;
                using (var sr = new StreamReader(ms)) {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Serialize an object array to a string in CSV format
        /// </summary>
        /// <param name="list">The object array to write</param>
        /// <param name="sw">The stream where the CSV text will be written</param>
        /// <param name="save_column_names">True if you wish the first line of the file to have column names</param>
        /// <param name="delim">The delimiter (comma, tab, pipe, etc) to separate fields</param>
        /// <param name="qual">The text qualifier (double-quote) that encapsulates fields that include delimiters</param>
        /// <returns>The CSV string representing the object array.</returns>
        public static string WriteToString<T>(this IEnumerable<T> list, bool save_column_names, char delim = DEFAULT_DELIMITER, char qual = DEFAULT_QUALIFIER)
        {
            using (var ms = new MemoryStream()) {
                var sw = new StreamWriter(ms);
                var cw = new CSVWriter(sw, delim, qual);
                cw.Write(list, save_column_names);
                sw.Flush();
                ms.Position = 0;
                using (var sr = new StreamReader(ms)) {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Output a single field value as appropriate
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Output(IEnumerable<object> line, char delimiter = DEFAULT_DELIMITER, char qualifier = DEFAULT_QUALIFIER, bool force_qualifiers = false)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object o in line) {

                // Null strings are just a delimiter
                if (o != null) {
                    string s = o.ToString();
                    if (s.Length > 0) {

                        // Does this string contain any risky characters?  Risky is defined as delim, qual, or newline
                        if (force_qualifiers || s.Contains(delimiter) || s.Contains(qualifier) || s.Contains(Environment.NewLine)) {
                            sb.Append(qualifier);

                            // Double up any qualifiers that may occur
                            sb.Append(s.Replace(qualifier.ToString(), qualifier.ToString() + qualifier.ToString()));
                            sb.Append(qualifier);
                        } else {
                            sb.Append(s);
                        }
                    }
                }

                // Move to the next cell
                sb.Append(delimiter);
            }

            // Subtract the trailing delimiter so we don't inadvertently add a column
            sb.Length -= 1;
            return sb.ToString();
        }
        #endregion

        #region Shortcuts for static read calls
        /// <summary>
        /// Read in a single CSV file into a datatable in memory
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="delim">The CSV field delimiter character.</param>
        /// <param name="qual">The CSV text qualifier character.</param>
        /// <returns>An data table of strings that were retrieved from the CSV file.</returns>
        public static DataTable LoadDataTable(string filename, bool first_row_are_headers = true, bool ignore_dimension_errors = true, char delim = CSV.DEFAULT_DELIMITER, char qual = CSV.DEFAULT_QUALIFIER)
        {
            return LoadDataTable(new StreamReader(filename), first_row_are_headers, ignore_dimension_errors, delim, qual);
        }

        /// <summary>
        /// Read in a single CSV file into a datatable in memory
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="delim">The CSV field delimiter character.</param>
        /// <param name="qual">The CSV text qualifier character.</param>
        /// <returns>An data table of strings that were retrieved from the CSV file.</returns>
        public static DataTable LoadDataTable(StreamReader stream, bool first_row_are_headers = true, bool ignore_dimension_errors = true, char delim = CSV.DEFAULT_DELIMITER, char qual = CSV.DEFAULT_QUALIFIER)
        {
            using (CSVReader cr = new CSVReader(stream, delim, qual)) {
                return cr.ReadAsDataTable(first_row_are_headers, ignore_dimension_errors, null);
            }
        }

        /// <summary>
        /// Read in a single CSV file into a datatable in memory
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="delim">The CSV field delimiter character.</param>
        /// <param name="qual">The CSV text qualifier character.</param>
        /// <returns>An data table of strings that were retrieved from the CSV file.</returns>
        public static DataTable LoadDataTable(string filename, string[] headers, bool ignore_dimension_errors = true, char delim = CSV.DEFAULT_DELIMITER, char qual = CSV.DEFAULT_QUALIFIER)
        {
            return LoadDataTable(new StreamReader(filename), headers, ignore_dimension_errors, delim, qual);
        }

        /// <summary>
        /// Read in a single CSV file into a datatable in memory
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="delim">The CSV field delimiter character.</param>
        /// <param name="qual">The CSV text qualifier character.</param>
        /// <returns>An data table of strings that were retrieved from the CSV file.</returns>
        public static DataTable LoadDataTable(StreamReader stream, string[] headers, bool ignore_dimension_errors = true, char delim = CSV.DEFAULT_DELIMITER, char qual = CSV.DEFAULT_QUALIFIER)
        {
            using (CSVReader cr = new CSVReader(stream, delim, qual)) {
                return cr.ReadAsDataTable(false, ignore_dimension_errors, headers);
            }
        }

        /// <summary>
        /// Read in a single CSV file as an array of objects
        /// </summary>
        /// <typeparam name="T">The type of objects to deserialize from this CSV.</typeparam>
        /// <param name="stream">The stream to read.</param>
        /// <param name="ignore_dimension_errors">Set to true if you wish to ignore rows that have a different number of columns.</param>
        /// <param name="ignore_bad_columns">Set to true if you wish to ignore column headers that don't match up to object attributes.</param>
        /// <param name="ignore_type_conversion_errors">Set to true if you wish to overlook elements in the CSV array that can't be properly converted.</param>
        /// <param name="delim">The CSV field delimiter character.</param>
        /// <param name="qual">The CSV text qualifier character.</param>
        /// <returns>An array of objects that were retrieved from the CSV file.</returns>
        public static List<T> LoadArray<T>(string filename, bool ignore_dimension_errors = true, bool ignore_bad_columns = true, bool ignore_type_conversion_errors = true, char delim = CSV.DEFAULT_DELIMITER, char qual = CSV.DEFAULT_QUALIFIER) where T : class, new()
        {
            return LoadArray<T>(new StreamReader(filename), ignore_dimension_errors, ignore_bad_columns, ignore_type_conversion_errors, delim, qual);
        }

        /// <summary>
        /// Saves an array of objects to a CSV string in memory.
        /// </summary>
        /// <typeparam name="T">The type of objects to serialize from this CSV.</typeparam>
        /// <param name="list">The array of objects to serialize.</param>
        /// <param name="save_column_names">Set to true if you wish the first line of the CSV to contain the field names.</param>
        /// <param name="force_qualifiers">Set to true to force qualifier characters around each field.</param>
        /// <param name="delim">The CSV field delimiter character.</param>
        /// <param name="qual">The CSV text qualifier character.</param>
        /// <returns>The CSV string.</returns>
        public static string SaveArray<T>(IEnumerable<T> list, bool save_column_names = true, bool force_qualifiers = false, char delim = CSV.DEFAULT_DELIMITER, char qual = CSV.DEFAULT_QUALIFIER) where T : class, new()
        {
            using (var ms = new MemoryStream()) {
                var sw = new StreamWriter(ms);
                var cw = new CSVWriter(sw, delim, qual);
                cw.Write<T>(list, save_column_names, force_qualifiers);
                sw.Flush();
                ms.Position = 0;
                using (var sr = new StreamReader(ms)) {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Read in a single CSV file as an array of objects
        /// </summary>
        /// <typeparam name="T">The type of objects to deserialize from this CSV.</typeparam>
        /// <param name="stream">The stream to read.</param>
        /// <param name="ignore_dimension_errors">Set to true if you wish to ignore rows that have a different number of columns.</param>
        /// <param name="ignore_bad_columns">Set to true if you wish to ignore column headers that don't match up to object attributes.</param>
        /// <param name="ignore_type_conversion_errors">Set to true if you wish to overlook elements in the CSV array that can't be properly converted.</param>
        /// <param name="delim">The CSV field delimiter character.</param>
        /// <param name="qual">The CSV text qualifier character.</param>
        /// <returns>An array of objects that were retrieved from the CSV file.</returns>
        public static List<T> LoadArray<T>(StreamReader stream, bool ignore_dimension_errors = true, bool ignore_bad_columns = true, bool ignore_type_conversion_errors = true, char delim = CSV.DEFAULT_DELIMITER, char qual = CSV.DEFAULT_QUALIFIER) where T : class, new()
        {
            using (CSVReader cr = new CSVReader(stream, delim, qual)) {
                return cr.Deserialize<T>(ignore_dimension_errors, ignore_bad_columns, ignore_type_conversion_errors);
            }
        }
        #endregion
    }
}
