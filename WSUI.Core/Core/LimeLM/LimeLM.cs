using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml;

namespace OF.Core.Core.LimeLM
{
    public class LimeLMApi
    {
        /// <summary>
        /// Your API key to accessing LimeLM.
        /// </summary>
        public static string APIKey
        {
            get { return "1m115715277fb67cc5a51.73953699"; }
        }

        public const string VersionId = "1432";

        public const int Quantity = 1;
        public const int DefaultTrialPeriod = 14;

        public const string TrialExpires = "trial_expires";
        public const string TimesUsed = "times_used";
        public const string UserEmail = "user_email";
        public const string IsTrialKey = "is_trial_key";

        //NOTE: If you're using the self-hosted version of LimeLM (that is,
        //      LimeLM running on your own servers), then replace the URL with your own.

        // Almost all users should leave this line unchanged.
        private const string post_url = "https://wyday.com/limelm/api/rest/";

        /// <summary>
        /// Add a new license feature for a particular version. Note that if you want to set the feature values for existing product keys, use the SetDetails() function.
        /// </summary>
        /// <param name="versionID">The version ID to add the new feature in.</param>
        /// <param name="name">The name of the feature.</param>
        /// <param name="required">Whether this feature will be required to be filled out when a new product key is created.</param>
        /// <param name="type">The type of the feature. Possible values are "string", "date_time", "int"</param>
        /// <returns>XML block with either an error or the returned feature id.</returns>
        public static string AddFeature(string versionID, string name, bool required, string type)
        {
            List<KeyValuePair<string, object>> postData = new List<KeyValuePair<string, object>>
                                          {
                                              new KeyValuePair<string, object>("method", "limelm.feature.add"),
                                              new KeyValuePair<string, object>("version_id", versionID),
                                              new KeyValuePair<string, object>("name", name),
                                              new KeyValuePair<string, object>("required", required ? "true" : "false"),
                                          };

            if (type != null)
                postData.Add(new KeyValuePair<string, object>("type", type));

            return PostRequest(postData);
        }

        /// <summary>
        /// Delete an existing license feature. If you delete a feature it is removed from all the product keys.
        /// </summary>
        /// <param name="featureID">The id of the feature you'll be deleting.</param>
        /// <returns>XML block with either an error or the returned success block.</returns>
        public static string DeleteFeature(string featureID)
        {
            List<KeyValuePair<string, object>> postData = new List<KeyValuePair<string, object>>
                                          {
                                              new KeyValuePair<string, object>("method", "limelm.feature.delete"),
                                              new KeyValuePair<string, object>("feature_id", featureID)
                                          };

            return PostRequest(postData);
        }

        /// <summary>
        /// Edit an existing license feature. Note that if you want to set the feature values for existing product keys, use the SetDetails() function.
        /// </summary>
        /// <param name="featureID">The id of the feature you'll be editing.</param>
        /// <param name="name">The new name of the feature.</param>
        /// <param name="required">Whether this feature will be required to be filled out when a new product key is created.</param>
        /// <returns>XML block with either an error or the returned success block.</returns>
        public static string EditFeature(string featureID, string name, bool? required)
        {
            List<KeyValuePair<string, object>> postData = new List<KeyValuePair<string, object>>
                                          {
                                              new KeyValuePair<string, object>("method", "limelm.feature.edit"),
                                              new KeyValuePair<string, object>("feature_id", featureID)
                                          };

            if (name != null)
                postData.Add(new KeyValuePair<string, object>("name", name));

            if (required != null)
                postData.Add(new KeyValuePair<string, object>("required", required.Value ? "true" : "false"));

            return PostRequest(postData);
        }

        /// <summary>
        /// Finds the product keys matching an email address.
        /// </summary>
        /// <param name="versionID">The version ID to search for the product keys.</param>
        /// <param name="email">The email associated with the product keys.</param>
        /// <returns>XML block with either an error or the returned pkeys.</returns>
        public static string FindPKey(string versionID, string email)
        {
            List<KeyValuePair<string, object>> postData = new List<KeyValuePair<string, object>>
                                          {
                                              new KeyValuePair<string, object>("method", "limelm.pkey.find"),
                                              new KeyValuePair<string, object>("version_id", versionID),
                                              new KeyValuePair<string, object>("email", email)
                                          };

            return PostRequest(postData);
        }

        /// <summary>
        /// Generate product keys.
        /// </summary>
        /// <param name="version_id">The version ID to generate the product keys in.</param>
        /// <param name="num_keys">The number of product keys to generate.</param>
        /// <param name="num_acts">The number of activations per product key.</param>
        /// <param name="email">The email to associate with the product keys. Set to NULL for no email.</param>
        /// <param name="feature_names">A string array list of feature names to set. Set to null if you don't want to set any features.</param>
        /// <param name="feature_values">A string array list of feature values to set. Set to null if you don't want to set any features.</param>
        /// <returns>XML block with either an error or the returned pkeys.</returns>
        public static string GeneratePKeys(string version_id, int num_keys, int num_acts, string email, string[] feature_names, string[] feature_values)
        {
            if (num_keys < 1)
                num_keys = 1;

            if (num_acts < 1)
                num_acts = 1;

            List<KeyValuePair<string, object>> postData = new List<KeyValuePair<string, object>>
                                          {
                                              new KeyValuePair<string, object>("method", "limelm.pkey.generate"),
                                              new KeyValuePair<string, object>("version_id", version_id),
                                              new KeyValuePair<string, object>("num_keys", num_keys.ToString()),
                                              new KeyValuePair<string, object>("num_acts", num_acts.ToString())
                                          };

            if (email != null)
                postData.Add(new KeyValuePair<string, object>("email", email));

            if (feature_names != null)
            {
                postData.Add(new KeyValuePair<string, object>("feature_name", feature_names));
                postData.Add(new KeyValuePair<string, object>("feature_value", feature_values));
            }

            return PostRequest(postData);
        }

        /// <summary>
        /// Gets the details for a product key.
        /// </summary>
        /// <param name="pkey_id">The ID of the product key.</param>
        /// <returns>XML block with either an error or the details of the product key.</returns>
        public static string GetDetails(string pkey_id)
        {
            List<KeyValuePair<string, object>> postData = new List<KeyValuePair<string, object>>
                                          {
                                              new KeyValuePair<string, object>("method", "limelm.pkey.getDetails"),
                                              new KeyValuePair<string, object>("pkey_id", pkey_id)
                                          };

            return PostRequest(postData);
        }

        /// <summary>
        /// Get the ID for a product key.
        /// </summary>
        /// <param name="version_id">The version ID to look for.</param>
        /// <param name="pkey">The product key.</param>
        /// <returns>XML block with either an error or the product key ID.</returns>
        public static string GetPKeyID(string version_id, string pkey)
        {
            List<KeyValuePair<string, object>> postData = new List<KeyValuePair<string, object>>
                                          {
                                              new KeyValuePair<string, object>("method", "limelm.pkey.getID"),
                                              new KeyValuePair<string, object>("version_id", version_id),
                                              new KeyValuePair<string, object>("pkey", pkey)
                                          };

            return PostRequest(postData);
        }

        /// <summary>
        /// Manually activates using the contents of an activation request file.
        /// </summary>
        /// <param name="xml_act_request">The contents of the XML activation request file.</param>
        /// <returns>XML block with either an error or the XML Activation response block.</returns>
        public static string ManualActivation(string xml_act_request)
        {
            List<KeyValuePair<string, object>> postData = new List<KeyValuePair<string, object>>
                                          {
                                              new KeyValuePair<string, object>("method", "limelm.pkey.manualActivation"),
                                              new KeyValuePair<string, object>("act_req_xml", xml_act_request)
                                          };

            return PostRequest(postData);
        }

        /// <summary>
        /// Sets the details for a product key. This include the number of activations, email, and any license features.
        /// </summary>
        /// <param name="pkey_id">The ID for the product key. Use GetPKeyID to get the pkey ID from a product key.</param>
        /// <param name="num_acts">If non-zero this changes the number of activations allowed for the product key.</param>
        /// <param name="email">If this is non-null this changes the email address associated with the product key.</param>
        /// <param name="feature_names">A string array list of feature names to set. Set to null if you don't want to set any features.</param>
        /// <param name="feature_values">A string array list of feature values to set. Set to null if you don't want to set any features.</param>
        /// <returns>XML block with either an error or stating it was successful.</returns>
        public static string SetDetails(string pkey_id, int num_acts, string email, string[] feature_names, string[] feature_values)
        {
            List<KeyValuePair<string, object>> postData = new List<KeyValuePair<string, object>>
                                          {
                                              new KeyValuePair<string, object>("method", "limelm.pkey.setDetails"),
                                              new KeyValuePair<string, object>("pkey_id", pkey_id)
                                          };

            if (num_acts != 0)
                postData.Add(new KeyValuePair<string, object>("num_acts", num_acts.ToString()));

            if (email != null)
                postData.Add(new KeyValuePair<string, object>("email", email));

            if (feature_names != null)
            {
                postData.Add(new KeyValuePair<string, object>("feature_name", feature_names));
                postData.Add(new KeyValuePair<string, object>("feature_value", feature_values));
            }

            return PostRequest(postData);
        }

        /// <summary>
        /// Generates a trial extension for a version ID.
        /// </summary>
        /// <param name="version_id">The version ID to generate the trial extension in.</param>
        /// <param name="is_online">Is the trial extension a small online extension or the larger offline extension.</param>
        /// <param name="length">The length of the trial extension in days.</param>
        /// <param name="expires">When the trial expires in the form YYYY-MM-DD</param>
        /// <param name="customer_id">An optional customer ID (e.g. email) to associate with the trial extension.</param>
        /// <param name="max_uses">The maximum number of uses for the online trial extension.</param>
        /// <returns>XML block with either an error or the returned trial extension.</returns>
        public static string GenerateTrialExtension(string version_id, bool is_online, uint length, string expires, string customer_id, uint max_uses)
        {
            List<KeyValuePair<string, object>> postData = new List<KeyValuePair<string, object>>
                                          {
                                              new KeyValuePair<string, object>("method", "limelm.trialExtension.generate"),
                                              new KeyValuePair<string, object>("version_id", version_id),
                                              new KeyValuePair<string, object>("is_online", is_online ? "true" : "false"),
                                              new KeyValuePair<string, object>("length", length.ToString()),
                                              new KeyValuePair<string, object>("expires", expires)
                                          };

            if (is_online)
                postData.Add(new KeyValuePair<string, object>("max_uses", max_uses.ToString()));

            if (customer_id != null)
                postData.Add(new KeyValuePair<string, object>("customer_id", customer_id));

            return PostRequest(postData);
        }

        /// <summary>
        /// A simple test to confirm your API key is correct.
        /// </summary>
        /// <returns>XML block with either an error or the echoed input.</returns>
        public static string TestEcho()
        {
            List<KeyValuePair<string, object>> postData = new List<KeyValuePair<string, object>>
                                          {
                                              new KeyValuePair<string, object>("method", "limelm.test.echo")
                                          };

            return PostRequest(postData);
        }

        public static bool IsEmailPresent(string email)
        {
            string resp = LimeLMApi.FindPKey(LimeLMApi.VersionId, email);
            using (XmlReader reader = XmlReader.Create(new StringReader(resp)))
            {
                reader.ReadToFollowing("rsp");
                return reader["stat"] == "ok";
            }
        }

        public static Tuple<string, string> FindAndReturnKey(string email)
        {
            string resp = LimeLMApi.FindPKey(LimeLMApi.VersionId, email);
            using (XmlReader reader = XmlReader.Create(new StringReader(resp)))
            {
                reader.ReadToFollowing("rsp");
                if (reader["stat"] == "ok")
                {
                    reader.ReadToFollowing("pkeys");
                    List<Tuple<string, string>> pkeys = new List<Tuple<string, string>>();
                    GetIdAndPKeys(reader, pkeys);
                    return pkeys.Count > 0 ? pkeys[0] : null;
                }
                return null;
            }
        }

        public static Tuple<string, string> GenerateAndReturnKey(string email)
        {
            string trialExp = DateTime.Now.AddDays(DefaultTrialPeriod).ToString("yyyy-MM-dd HH:mm:ss");
            string resp = LimeLMApi.GeneratePKeys(LimeLMApi.VersionId, LimeLMApi.Quantity, 1, email, new[] { TrialExpires, UserEmail, IsTrialKey, TimesUsed }, new[] { trialExp, email, 1.ToString(), 0.ToString() });
            using (XmlReader reader = XmlReader.Create(new StringReader(resp)))
            {
                reader.ReadToFollowing("rsp");

                if (reader["stat"] != "ok")
                {
                    //TODO: handle errors how ever you need to (logging, throwing exceptions, etc.)
                    reader.ReadToFollowing("err");
                    throw new Exception("Failed to generate the product keys: " + reader["msg"]);
                }

                reader.ReadToFollowing("pkeys");

                List<Tuple<string, string>> pkeys = new List<Tuple<string, string>>();
                GetIdAndPKeys(reader, pkeys);
                return pkeys.Count > 0 ? pkeys[0] : null;
            }
        }

        private static void GetPKeys(XmlReader reader, List<string> pkeys)
        {
            while (reader.Read())
            {
                //end of node </pkeys>
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals("pkeys"))
                    return;

                if (reader.NodeType == XmlNodeType.Element && reader.LocalName.Equals("pkey"))
                    pkeys.Add(reader["key"]);
            }
        }

        private static void GetIdAndPKeys(XmlReader reader, List<Tuple<string, string>> pkeys)
        {
            while (reader.Read())
            {
                //end of node </pkeys>
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals("pkeys"))
                    return;

                if (reader.NodeType == XmlNodeType.Element && reader.LocalName.Equals("pkey"))
                    pkeys.Add(new Tuple<string, string>(reader["id"], reader["key"]));
            }
        }

        private static string PostRequest(List<KeyValuePair<string, object>> postData)
        {
            if (APIKey == null)
                throw new Exception("You must specify your LimeLM API key.");

            postData.Add(new KeyValuePair<string, object>("api_key", APIKey));

            // This section takes the input fields and converts them to the proper format
            // for an http post.  For example: "method=limelm.pkey.find&version_id=100"
            string post_string = "";

            foreach (KeyValuePair<string, object> kv in postData)
            {
                if (kv.Value.GetType() == typeof(string[]))
                {
                    foreach (string val in (string[])kv.Value)
                    {
                        post_string += kv.Key + "[]=" + val + "&";
                    }
                }
                else
                    post_string += kv.Key + "=" + (string)kv.Value + "&";
            }

            post_string = post_string.TrimEnd('&');

            // create an HttpWebRequest object to communicate with LimeLM
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(post_url);
            objRequest.Method = "POST";
            objRequest.ContentLength = post_string.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";
            objRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");

            // post data is sent as a stream
            StreamWriter myWriter = new StreamWriter(objRequest.GetRequestStream());
            myWriter.Write(post_string);
            myWriter.Close();

            // returned values are returned as a stream, then read into a string
            string post_response;
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

            Stream responseStream = objResponse.GetResponseStream();

            if (objResponse.ContentEncoding.ToLower().Contains("gzip"))
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            else if (objResponse.ContentEncoding.ToLower().Contains("deflate"))
                responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

            using (StreamReader streamReader = new StreamReader(responseStream))
            {
                post_response = streamReader.ReadToEnd();
            }

            return post_response;
        }
    }
}