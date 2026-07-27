using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AskalePortal.Constants
{
    public class DataReader
    {
        public static string Getstring(object input)
        {
            return input?.ToString()??"";
        }
        public static string GenerateId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - StaticMethods.DateTimeNow().Ticks);
        }
        public static string Getstring(object input, bool dbCheck)
        {
            try
            {
                string str = Getstring(input);
                return str;
                //return str.Replace("'", "''").Replace("\\", "").Replace("/*", "").Replace("*/", "");
            }
            catch { return ""; }
        }

        public static bool GetBoolean(object input)
        {
            try { return (Convert.ToBoolean(input)); }
            catch { return false; }
        }

        public static bool? GetBooleanNullable(object input)
        {
            try
            {
                return (Convert.ToBoolean(input));
            }
            catch
            {
                return null;
            }
        }

        public static Int32 GetInt32(object input)
        {
            try
            {
                if (input != null)
                {
                    return Int32.Parse(input.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }

        public static Int32 GetInt32(object input, int defaultValue)
        {
            try { return Int32.Parse(input.ToString()); }
            catch { return defaultValue; }
        }

        public static Double GetDouble(object input)
        {
            try { return Convert.ToDouble(input.ToString()); }
            catch { return 0; }
        }

        public static DateTime GetDateTime(object input)
        {
            try { return (DateTime.Parse(input.ToString())); }
            catch
            {
                return GetDateTimeNOW();
            }
        }

        public static DateTime? GetDateTimeNullable(object input)
        {
            try { return (DateTime.Parse(input.ToString())); }
            catch
            {
                return null;
            }
        }

        public static string GetDateTimeNullablestring(object input)
        {
            try { return (DateTime.Parse(input.ToString()).ToString("dd.MM.yyyy")); }
            catch
            {
                return DateTime.Now.Date.ToString("dd.MM.yyyy");
            }
        }
        public static string GetDateTimeForFilter(object input)
        {
            try { return (DateTime.Parse(input.ToString()).ToString("dd.MM.yyyy")); }
            catch
            {
                return "";
            }
        }
        public static DateTime GetDateTimeNOW()
        {
            return (StaticMethods.DateTimeNow());
        }

        public static Decimal GetDecimal(object input)
        {
            try { return Decimal.Parse(input.ToString()); }
            catch { return 0; }
        }

        public static bool IsEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                 @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                 @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        public static string UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedstring = encoding.GetString(characters);
            return (constructedstring);
        }

        public static Byte[] stringToUTF8ByteArray(string pXmlstring)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlstring);
            return byteArray;
        }
    }
}
