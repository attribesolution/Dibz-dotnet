using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace DIBZ.Common
{
    public static class ConversionHelper
    {
        private static CultureInfo _defaultCulture = null;
        //private const string DefaultDateTimeFormat = "MM/dd/yyyy hh:mm:ss";
        private const string DefaultDateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        private const string DATE_FORMAT = "MM-dd-yyyy";

        public static CultureInfo DefaultCulture
        {
            get
            {
                if (_defaultCulture == null)
                    _defaultCulture = new CultureInfo("en-US", false);

                return _defaultCulture;
            }
        }

        /// <summary>
        /// Safes the convert to int32.
        /// </summary>
        /// <param name="objToConvert">The obj to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static int SafeConvertToInt32(object objToConvert, int defaultValue)
        {
            if (objToConvert == null)
                return defaultValue;
            int value = 0;
            if (int.TryParse(objToConvert.ToString(), out value))
                return value;
            else
                return defaultValue;
        }

        /// <summary>
        /// Safes the convert to int64.
        /// </summary>
        /// <param name="objToConvert">The obj to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static long SafeConvertToInt64(object objToConvert, long defaultValue)
        {
            if (objToConvert == null)
                return defaultValue;
            long value = 0;
            if (long.TryParse(objToConvert.ToString(), out value))
                return value;
            else
                return defaultValue;
        }

        /// <summary>
        /// Safes the convert to int64.
        /// </summary>
        /// <param name="objToConvert">The obj to convert.</param>
        /// <returns></returns>
        public static long SafeConvertToInt64(object objToConvert)
        {
            return SafeConvertToInt64(objToConvert, 0);
        }


        /// <summary>
        /// Safes the convert to int32.
        /// </summary>
        /// <param name="objToConvert">The obj to convert.</param>
        /// <returns></returns>
        public static int SafeConvertToInt32(object objToConvert)
        {
            return SafeConvertToInt32(objToConvert, 0);
        }

        /// <summary>
        /// Safes the convert to decimal.
        /// </summary>
        /// <param name="objToConvert">The obj to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static decimal SafeConvertToDecimal(object objToConvert, decimal defaultValue)
        {
            if (objToConvert == null)
                return defaultValue;
            decimal value = 0;

            if (decimal.TryParse(objToConvert.ToString(), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                return value;
            else
                return defaultValue;
        }

        /// <summary>
        /// Safes the convert to decimal.
        /// </summary>
        /// <param name="objToConvert">The obj to convert.</param>
        /// <returns></returns>
        public static decimal SafeConvertToDecimal(object objToConvert)
        {
            return SafeConvertToDecimal(objToConvert, 0M);
        }

        public static decimal? SafeConvertToDecimalNullable(object objToConvert, decimal? defaultValue = null)
        {
            if (objToConvert == null)
                return defaultValue;
            decimal value = 0;

            if (decimal.TryParse(objToConvert.ToString(), out value))
                return value;
            else
                return defaultValue;
        }

        /// <summary>
        /// Safes the convert to int32 nullable.
        /// </summary>
        /// <param name="objToConvert">The obj to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static int? SafeConvertToInt32Nullable(object objToConvert, int? defaultValue)
        {
            if (objToConvert == null)
                return defaultValue;
            int value = 0;
            if (int.TryParse(objToConvert.ToString(), out value))
                return new int?(value);
            else
                return defaultValue;
        }

        /// <summary>
        /// Safes the convert to text.
        /// </summary>
        /// <param name="objToConvert">The obj to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string SafeConvertToText(object objToConvert, string defaultValue)
        {
            if (objToConvert == null)
                return defaultValue;
            return objToConvert.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objToConvert"></param>
        /// <returns></returns>
        public static string SafeConvertToText(object objToConvert)
        {
            return SafeConvertToText(objToConvert, string.Empty);
        }
        public static IEnumerable<int> StringToIntList(string str)
        {
            if (String.IsNullOrEmpty(str))
                yield break;

            foreach (var s in str.Split(','))
            {
                int num;
                if (int.TryParse(s, out num))
                {
                    yield return num;
                }
            }
        }

        /// <summary>
        /// Safes the convert to bool.
        /// </summary>
        /// <param name="objToConvert">The obj to convert.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns></returns>
        public static bool SafeConvertToBool(object objToConvert, bool defaultValue = false)
        {
            if (objToConvert == null)
                return defaultValue;

            bool value = false;

            if (bool.TryParse(objToConvert.ToString(), out value))
                return value;
            else
                return defaultValue;
        }

        /// <summary>
        /// Gets the limited string.
        /// </summary>
        /// <param name="objToGet">The obj to get.</param>
        /// <param name="limit">The limit for number of characters.</param>
        /// <returns></returns>
        public static string GetLimitedString(object objToGet, int limit)
        {
            string str = SafeConvertToText(objToGet, "");

            if (limit > 0)
                str = str.Length > limit ? str.Substring(0, limit) + "..." : str;

            return str;
        }

        /// <summary>
        /// Parses the int CSV.
        /// </summary>
        /// <param name="numbers">The numbers.</param>
        /// <returns></returns>
        public static List<int> ParseIntCSV(string numbers)
        {
            List<int> numb = new List<int>();
            if (numbers != null)
            {
                string[] splts = numbers.Split(',');
                for (int i = 0; i < splts.Length; i++)
                {
                    numb.Add(SafeConvertToInt32(splts[i].Trim(), 0));
                }
            }
            return numb;
        }

        /// <summary>
        /// Converts to min date.
        /// </summary>
        /// <param name="objDt">The obj dt.</param>
        /// <returns></returns>
        public static DateTime? ConvertToMinDate(object objDt)
        {
            try
            {
                DateTime dt = DateTime.Parse("" + objDt);
                dt = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                return dt;
            }
            catch { }
            return null;
        }
        public static DateTime? ConvertToMaxDate(object objDt)
        {
            try
            {
                DateTime dt = DateTime.Parse("" + objDt);
                dt = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
                return dt;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Encodes a string to be represented as a string literal. The format
        /// is essentially a JSON string.
        /// 
        /// The string returned includes outer quotes 
        /// Example Output: "Hello \"Rick\"!\r\nRock on"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EncodeJsString(string s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\'':
                        sb.Append("\\'");
                        break;
                    default:
                        int i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append("\"");

            return sb.ToString();
        }

        public static T CloneObject<T>(T obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            try
            {
                formatter.Serialize(ms, obj);
                ms.Flush();
                ms.Position = 0L;
                return (T)formatter.Deserialize(ms);
            }
            catch
            {
                return default(T);
            }
            finally
            {
                ms.Close();
                ms.Dispose();
            }
        }

        public static DateTime ConvertToMinDateTime(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 00, 00, 00);
        }

        public static DateTime ConvertToMaxDateTime(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
        }

        public static DateTime? ParseDateUsingDefaultFormat(string date, DateTime? defaultDate)
        {
            return ParseDate(date, DefaultDateTimeFormat, defaultDate);
        }

        public static DateTime? ParseDate(string date, DateTime? defaultDate)
        {
            return ParseDate(date, DATE_FORMAT, defaultDate);
            //return ParseDate(date, "dd/MM/yyyy", defaultDate);
        }

        public static DateTime ParseDate(string date, DateTime defaultDate)
        {
            if (date != null)
            {
                return Convert.ToDateTime(date);
            }
            else { return defaultDate; }
        }

        public static DateTime ParseDate(DateTime? date, DateTime defaultDate)
        {
            if (date != null)
            {
                return Convert.ToDateTime(date);
            }
            else { return defaultDate; }
        }


        public static DateTime ConvertToUtc(DateTime dt)
        {
            var utcDT = dt.ToUniversalTime();
            return utcDT;

        }
        public static TimeSpan ParseTime(string time)
        {
            string format = "hh:mm tt";
            TimeSpan ts = DateTime.ParseExact(time, format, CultureInfo.InvariantCulture).TimeOfDay;
            return ts;
        }

        public static DateTime ConvertDateToTimeZone(DateTime? dt)
        {
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

            var UtcDate1 = Convert.ToDateTime(dt);
            DateTime UtCTimeInUsZone1 = TimeZoneInfo.ConvertTimeFromUtc(UtcDate1, est);
            return UtCTimeInUsZone1;
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString(DefaultDateTimeFormat, DefaultCulture);
        }

        public static string FormatDate(DateTime date, string format)
        {
            return date.ToString(format, DefaultCulture);
        }

        public static DateTime? ParseDate(string date, string format, DateTime? defaultDate)
        {
            DateTime dtDate = DateTime.Now;
            if (DateTime.TryParseExact(date, format, DefaultCulture, System.Globalization.DateTimeStyles.None, out dtDate))
                return dtDate;
            else
                return defaultDate;
        }

        /// <summary>
        /// Convert time to datetime e.g 16:03 to 1/1/1900 4:03 PM
        /// </summary>
        /// <param name="time">Must be in HH:mm format</param>
        /// <returns></returns>
        public static DateTime? ConvertDateTimeFromTime(string time)
        {
            var cultureInfo = DefaultCulture;
            var dateTime = DateTime.Now;

            if (DateTime.TryParseExact(
                        string.Format("01/01/1900 {0}", time),
                        "MM/dd/yyyy HH:mm", cultureInfo, System.Globalization.DateTimeStyles.None, out dateTime))
                return dateTime;
            else
                return null;
        }

        public static string EncodeWithBase64(string text)
        {
            byte[] bSourceData = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] byteTextB = null;
            System.Text.Encoding ec = System.Text.Encoding.GetEncoding("ISO-8859-1");

            byteTextB = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, ec, bSourceData);

            return Convert.ToBase64String(byteTextB);

        }

        public static double SafeConvertToDouble(object objToConvert, double defaultValue)
        {
            if (objToConvert == null)
                return defaultValue;
            double value = 0;
            if (double.TryParse(objToConvert.ToString(), out value))
                return value;
            else
                return defaultValue;
        }

        public static double ConvertToJSMilliSeconds(int minute)
        {
            var minTime = new DateTime(1900, 1, 1).AddMinutes(minute);

            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = minTime.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);

            return ts.TotalMilliseconds;
        }
        public static double SafeConvertToDoubleCultureInd(object objToConvert, double defaultValue)
        {
            if (objToConvert == null)
                return defaultValue;
            double value = 0;
            if (double.TryParse(objToConvert.ToString(), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                return value;
            else
                return defaultValue;
        }

        /// <summary>
        /// Shuffles a list randomly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            var finalList = list;
        }

        public static double SafeConvertToDoubleCultureInd(object objToConvert)
        {
            return SafeConvertToDoubleCultureInd(objToConvert, 0d);
        }
        public static string ConvertToPlainText(string htmlText)
        {

            var server = HttpContext.Current.Server;
            if (server == null)
                return htmlText;

            return htmlText == null ? null : Regex.Replace(server.HtmlDecode(htmlText), "<[^>]+?>", "");
        }

        /// <summary>
        /// returns the absolute path for the given relative or absolute path.
        /// </summary>
        /// <param name="path">a path, that can be relative or absolute</param>
        /// <returns>the absolute path corresponding to the given path</returns>
        public static string GetSafeAbsolutePath(string path)
        {
            if (
                !System.IO.Path.IsPathRooted(path) &&
                !path.ToLower().Contains("photo.axd")
                )
            {
                return VirtualPathUtility.ToAbsolute(path);
            }
            else
            {
                return path;
            }
        }

        #region Image Conversion to text
        public static string ImageToBase64(System.Drawing.Image image,
                System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            if (imageBytes == null)
                return null;

            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
        #endregion

        public static string GetUniqueFileName(string fileName)
        {
            return string.Format("{0}{1}", Guid.NewGuid().ToString(), Path.GetExtension(fileName));
        }

        public static decimal GetFractionalPart(decimal number)
        {
            var numericPart = Math.Truncate(number);
            var fraction = number - numericPart;
            return fraction;
        }

        /// <summary>
        ///example: Rounds 1.01 to 1.5 and rounds 1.51 to 2.0
        /// Custom rounding method. Rounds any decimal having fractional part between 0.01 and 0.5 to "0.5", and any decimal having fractional part above 0.5 to the next integer.  
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal GetHalfRoundedDecimal(decimal value)
        {
            var fractionalPart = ConversionHelper.GetFractionalPart(value);

            if (fractionalPart > 0)
            {
                if (fractionalPart <= (decimal)0.5)
                {
                    value = Math.Truncate(value) + (decimal)0.5;
                }
                else
                {
                    value = Math.Truncate(value) + 1;
                }
            }

            return value;
        }

        /// <summary>
        /// returns the absolute path for the given relative or absolute path.
        /// </summary>
        /// <param name="path">a path, that can be relative or absolute</param>
        /// <returns>the absolute path corresponding to the given path</returns>
        public static string GetSafeServerMapPath(string path)
        {
            if (!path.Contains(":") &&
                !path.ToLower().Contains("photo.axd"))
            {
                return HttpContext.Current.Server.MapPath(path);
            }
            else
            {
                return path;
            }
        }

        #region General Helper methods


        /// <summary>
        /// Returns the datetime object from the given strings.
        /// </summary>
        /// <param name="date">a string in this format 10/27/2011</param>
        /// <param name="time">a string in this format 06:05 PM</param>
        /// <returns></returns>
        public static DateTime GetDateTime(string date, string time)
        {
            int hour, minute;
            if (time.ToLower().Contains("am"))
            {
                string newTime = time.ToLower();
                newTime = newTime.Replace("am", string.Empty).Replace(" ", string.Empty);
                hour = ConversionHelper.SafeConvertToInt32(newTime.Split(':')[0]);
                minute = ConversionHelper.SafeConvertToInt32(newTime.Split(':')[1]);

                if (hour == 12)
                {
                    hour = 0;
                }
            }
            else if (time.ToLower().Contains("pm"))
            {
                string newTime = time.ToLower();
                newTime = newTime.Replace("pm", string.Empty).Replace(" ", string.Empty);
                hour = ConversionHelper.SafeConvertToInt32(newTime.Split(':')[0]);

                hour = (hour % 12) + 12;
                minute = ConversionHelper.SafeConvertToInt32(newTime.Split(':')[1]);
            }
            else
            {
                string newTime = time.ToLower().Replace(" ", string.Empty);
                hour = ConversionHelper.SafeConvertToInt32(newTime.Split(':')[0]);
                minute = ConversionHelper.SafeConvertToInt32(newTime.Split(':')[1]);
            }

            var date1 = Convert.ToDateTime(date);
            var datetime = new DateTime(date1.Year, date1.Month, date1.Day,
                hour, minute, 0);

            return datetime;
        }

        public static DateTime? SafeConvertStringToDateTime(string date)
        {
            try
            {
                return Convert.ToDateTime(date);
            }
            catch
            {
                return null;
            }
        }
        public static string ConvertListToCSVString(List<string> listString)
        {
            return string.Join(",", listString);
        }
        #endregion
    }
}
