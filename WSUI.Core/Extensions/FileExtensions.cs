using System;
using System.IO;

namespace OF.Core.Extensions
{
    public static class FileExtensions
    {
        private static string[] VideoExtensions = new string[] { ".FLV", ".ASF", ".AVI", ".WMV", ".MP4", ".MOV", ".3GP", ".M4V", ".3G2", ".MPEG", ".MPG", ".MPE" };
        private static string[] AllowExtensions = new string[] { ".DOC", ".DOCX", ".XLS", ".XLSX", ".PPT", ".PPTX", ".PDF", ".TXT", ".LOG" };


        public static bool IsVideoFile(this string filename)
        {
            return IsPresentInArray(VideoExtensions, filename);
        }

        public static bool IsFileAllowed(this string filename)
        {
            return IsPresentInArray(AllowExtensions,filename);
        }

        private static bool IsPresentInArray(string[] etxs, string filename)
        {
            return Array.IndexOf(etxs, Path.GetExtension(filename).ToUpperInvariant()) > -1;
        }

    }
}