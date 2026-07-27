
using System.Globalization;

using System.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AskalePortal.Constants
{
    public static class StaticMethods
    {
        public static string GenerateId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTimeNow().Ticks);
        }

        public static DateTime DateTimeNow()
        {
            return (DateTime.Now);
        }

        public static string RemoveText(string p, int limit)
        {
            if (p.Length > limit)
                return p.Remove(limit) + "...";
            return p;
        }

        public static string RemoveText_(string p, int limit)
        {
            if (p.Length > limit)
                return p.Remove(limit);
            return p;
        }

        public static string ReWriterPath(string Baslik)
        {
            string Temp = "";
            CultureInfo info = new CultureInfo("en");
            Temp = Baslik.ToLower(info);
            Temp = Temp.Replace(" ", "-");
            Temp = Temp.Replace("ç", "c"); Temp = Temp.Replace("ğ", "g");
            Temp = Temp.Replace("ı", "i"); Temp = Temp.Replace("ö", "o");
            Temp = Temp.Replace("ş", "s"); Temp = Temp.Replace("ü", "u");
            Temp = Temp.Replace("\"", ""); Temp = Temp.Replace("/", "-");
            Temp = Temp.Replace("(", "-"); Temp = Temp.Replace(")", "-");
            Temp = Temp.Replace("{", "-"); Temp = Temp.Replace("}", "-");
            Temp = Temp.Replace("%", "-"); Temp = Temp.Replace("&", "-");
            Temp = Temp.Replace("+", "-"); Temp = Temp.Replace(".", "-");
            Temp = Temp.Replace("?", ""); Temp = Temp.Replace(",", "-");
            Temp = Temp.Replace("'", "");
            Temp = Temp.Replace("“", "");
            Temp = Temp.Replace("”", "");
            Temp = Temp.Replace("’", "");

            Temp = Temp.Replace("!", "");
            Temp = Temp.Replace(":", "-");
            Temp = Temp.Replace("_", "-");
            Temp = Temp.Replace(";", "-");
            Temp = Temp.Replace("\"", " ");
            Temp = Temp.Replace("----", "-");
            Temp = Temp.Replace("---", "-");
            Temp = Temp.Replace("--", "-");
            Temp = Temp.Replace("…", "");

            if (Temp.EndsWith("-"))
                Temp = Temp.Remove(Temp.Length - 1, 1);
            if (Temp.StartsWith("-"))
                Temp = Temp.Remove(0, 1);
            return Temp;
        }

        public static string seoUrl(int ID, string type, string title)
        {
            return "/" + type + "/" + StaticMethods.ReWriterPath(title) + "/" + ID;
        }

        public static string GetMeta(string description, string keywords)
        {
            string text = default(string);

            if (!string.IsNullOrEmpty(description))
                text += string.Format("<meta name=\"description\" content=\"{0}\" />\n", description);
            if (!string.IsNullOrEmpty(keywords))
                text += string.Format("<meta name=\"keywords\" content=\"{0}\" />\n", keywords);

            return text;
        }

        public static string DateTimeToString(DateTime dt)
        {
            return dt.ToString("dd.MM.yyyy - HH:mm");
        }

        public static string GetFileIcon(string extension)
        {
            extension = extension.ToLower();
            string[] a = extension.Split('.');
            extension = "." + a[1];
            string imageName = "";
            switch (extension)
            {
                case ".pdf":
                    imageName = "pdfButton.png";
                    break;
                case ".zip":
                case ".rar":
                case ".gzip":
                    imageName = "zipButton.png";
                    break;
                case ".doc":
                case ".docx":
                    imageName = "wordButton.png";
                    break;
                case ".xls":
                case ".xlsx":
                    imageName = "excelButton.png";
                    break;
                case ".ppt":
                case ".pptx":
                    imageName = "powerpointButton.png";
                    break;
                case ".flv":
                case ".wma":
                    imageName = "videoButton.png";
                    break;
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                    imageName = "imageButton.png";
                    break;
                default:
                    imageName = "imageButton.png";
                    break;
            }
            return "<img src=\"/resources/Web/Images/" + imageName + "\" alt=\"\" style=\"margin:0px 3px 0px 0px;padding:0px;\"/>";
        }

        public static string PlainText(string text)
        {
            text = text.Trim();
            System.Text.RegularExpressions.Regex regHtml = new System.Text.RegularExpressions.Regex("<[^>]*>");
            string s = regHtml.Replace(text, "").Replace("&nbsp;", " ");
            return s;
        }
        public static List<string> PlainText2(string text)
        {
            text = text.Trim();
            System.Text.RegularExpressions.Regex regHtml = new System.Text.RegularExpressions.Regex("<[^>]*>");
            string s = regHtml.Replace(text, "").Replace("&nbsp;", " ").Replace("\n", "").Replace("\r", "");
            s = s.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            var noktali = s.Split('.');

            List<string> cumleler = new List<string>();
            foreach (var item in noktali)
            {
                if (item.Length > 100)
                {
                    var virgul = item.Split(',');
                    foreach (var item2 in virgul)
                    {
                        if (item2.Length > 100)
                        {
                            var wordGroups = item2.Split(' ')
                     .Select((word, i) => new { Word = word, Pos = i })
                     .GroupBy(w => w.Pos / 10)
                     .Select(g => string.Join(" ", g.Select(x => x.Word)))
                     .ToList();
                            foreach (var item3 in wordGroups)
                            {
                                cumleler.Add((item3.Trim().Replace("\r\n", "").Replace("\r", "").Replace("\n", "")));
                            }
                        }
                        else
                        {
                            cumleler.Add((item2.Trim().Replace("\r\n", "").Replace("\r", "").Replace("\n", "")));
                        }
                    }


                }
                else
                    cumleler.Add((item.Trim().Replace("\r\n", "").Replace("\r", "").Replace("\n", "")));
            }
            return cumleler;
        }

        private static string ReplaceFirst(string haystack, string needle, string replacement)
        {
            int pos = haystack.IndexOf(needle);
            if (pos < 0) return haystack;
            return haystack.Substring(0, pos) + replacement + haystack.Substring(pos + needle.Length);
        }

        private static string ReplaceAll(string haystack, string needle, string replacement)
        {
            int pos;
            // Avoid a possible infinite loop
            if (needle == replacement) return haystack;
            while ((pos = haystack.IndexOf(needle)) > 0)
                haystack = haystack.Substring(0, pos) + replacement + haystack.Substring(pos + needle.Length);
            return haystack;
        }

        public static string StripTags(string Input, string[] AllowedTags)
        {
            Regex StripHTMLExp = new Regex(@"(<\/?[^>]+>)");
            string Output = Input;

            foreach (Match Tag in StripHTMLExp.Matches(Input))
            {
                string HTMLTag = Tag.Value.ToLower();
                bool IsAllowed = false;

                foreach (string AllowedTag in AllowedTags)
                {
                    int offset = -1;

                    // Determine if it is an allowed tag
                    // "<tag>" , "<tag " and "</tag"
                    if (offset != 0) offset = HTMLTag.IndexOf('<' + AllowedTag + '>');
                    if (offset != 0) offset = HTMLTag.IndexOf('<' + AllowedTag + ' ');
                    if (offset != 0) offset = HTMLTag.IndexOf("</" + AllowedTag);

                    // If it matched any of the above the tag is allowed
                    if (offset == 0)
                    {
                        IsAllowed = true;
                        break;
                    }
                }

                // Remove tags that are not allowed
                if (!IsAllowed) Output = ReplaceFirst(Output, Tag.Value, "");
            }

            return Output;
        }

        public static string StripTagsAndAttributes(string Input, string[] AllowedTags)
        {
            /* Remove all unwanted tags first */
            string Output = StripTags(Input, AllowedTags);

            /* Lambda functions */
            MatchEvaluator HrefMatch = m => m.Groups[1].Value + "href..;,;.." + m.Groups[2].Value;
            MatchEvaluator ClassMatch = m => m.Groups[1].Value + "class..;,;.." + m.Groups[2].Value;
            MatchEvaluator UnsafeMatch = m => m.Groups[1].Value + m.Groups[4].Value;

            /* Allow the "href" attribute */
            Output = new Regex("(<a.*)href=(.*>)").Replace(Output, HrefMatch);

            /* Allow the "class" attribute */
            Output = new Regex("(<a.*)class=(.*>)").Replace(Output, ClassMatch);

            /* Remove unsafe attributes in any of the remaining tags */
            Output = new Regex(@"(<.*) .*=(\'|\""|\w)[\w|.|(|)]*(\'|\""|\w)(.*>)").Replace(Output, UnsafeMatch);

            /* Return the allowed tags to their proper form */
            Output = ReplaceAll(Output, "..;,;..", "=");

            return Output;
        }

        public static string GetGoogleMap(string p, string p_2, string p_3)
        {
            return string.Empty;
        }

        public static string getPageSource(string URL)
        {
            System.Net.WebClient webClient = new System.Net.WebClient();
            string strSource = webClient.DownloadString(URL);
            webClient.Dispose();
            return strSource;
        }
       
        public static string getPageSource2(string URL)
        {

            HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
            myWebRequest.Method = "GET";
            // make request for web page
            HttpWebResponse myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
            StreamReader myWebSource = new StreamReader(myWebResponse.GetResponseStream());
            string myPageSource = myWebSource.ReadToEnd();
            myWebResponse.Close();

            return myPageSource;
        }

        public static string GetExtensionDateTime()
        {
            return DateTimeNow().ToShortDateString().Replace(".", "") + "_" + DateTimeNow().ToShortTimeString().Replace(":", "");
        }

        public static string Sifrele_MD5(string metin)
        {
            MD5CryptoServiceProvider alg = new MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(metin);
            bs = alg.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }

            return s.ToString();
        }

        public static string TurnFirstToUpperTR(string input)
        {
            input = input.Trim().Replace("  ", " ");
            CultureInfo info = new CultureInfo("tr");
            return info.TextInfo.ToTitleCase(input.ToLower(info));
        }
        public static string TurnToUpperTR(string input)
        {
            CultureInfo info = new CultureInfo("tr");
            return input.ToUpper(info);
        }

        public static string HttpPostRequest(string url, string post)
        {
            var encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(post);
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();
            WebResponse response = request.GetResponse();
            string result;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                sr.Close();
            }
            return result;
        }

        public static string HttpGetRequest(string url, string[] headers)
        {
            string result;
            WebRequest request = WebRequest.Create(url);
            if (headers.Length > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header);
                }
            }
            WebResponse response = request.GetResponse();
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                sr.Close();
            }
            return result;
        }

        public static string ToJSON(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T ParseJsonObject<T>(string json) where T : class, new()
        {
            JObject jobject = JObject.Parse(json);
            return JsonConvert.DeserializeObject<T>(jobject.ToString());
        }
    }

}
