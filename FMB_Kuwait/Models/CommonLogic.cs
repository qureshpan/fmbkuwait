using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace FMB_Kuwait.Models
{
    public class CommonLogic
    {
        public enum SettingSection
        {
            Theme = 1,
            ProductForm = 2
        }

        public enum CacheKeys
        {
            ProductSearch = 1
        }

        public static bool ApplicationBool(string paramName)
        {
            string tmpS = Application(paramName).ToUpperInvariant();
            return (tmpS == "TRUE" || tmpS == "YES" || tmpS == "1");
        }
        public static string Application(string paramName)
        {
            string tmpS = string.Empty;
            if ((ConfigurationManager.AppSettings[paramName] != null))
            {
                try
                {
                    tmpS = ConfigurationManager.AppSettings[paramName];
                }
                catch
                {
                    tmpS = string.Empty;
                }
            }
            return tmpS;
        }

        public static string CurrentLocale
        {
            get { return Thread.CurrentThread.CurrentCulture.Name; }
        }
        private static string _DBConn = ConnectionString("DefaultConnection");
        public static string ConnectionString(string paramName)
        {
            if (string.IsNullOrEmpty(_DBConn))
            {
                _DBConn = SetDbConn(paramName);
            }
            return _DBConn;
        }

        public static string SetDbConn(string paramName)
        {
            string tmpS = string.Empty;
            if ((WebConfigurationManager.ConnectionStrings[paramName] != null))
            {
                try
                {
                    tmpS = WebConfigurationManager.ConnectionStrings[paramName].ConnectionString;
                }
                catch
                {
                    tmpS = string.Empty;
                }
            }
            return tmpS;
        }

        public static string GetLocale(string lng)
        {
            if (String.IsNullOrEmpty(lng))
                lng = "en";

            if (lng.Equals("ar") || lng.Equals("ar-KW"))
                lng = "ar-KW";
            else
                lng = "en-US";

            return lng;
        }

        public static int GetLocaleCode(string lng)
        {
            int lngCode = 1;
            if (String.IsNullOrEmpty(lng))
                lng = "en";



            return lngCode;
        }

        public static string CookieCleanUp(string paramName, bool decode)
        {
            if (HttpContext.Current.Request.Cookies[paramName] == null)
            {
                return string.Empty;
            }
            try
            {
                string tmp = HttpContext.Current.Request.Cookies[paramName].Value.ToString();
                if (decode)
                {
                    tmp = HttpContext.Current.Server.UrlDecode(tmp);
                }
                return tmp;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void SetCookie(string cookieName, string cookieVal, TimeSpan ts)
        {
            try
            {
                HttpCookie cookie = new HttpCookie(cookieName);
                cookie.Value = HttpContext.Current.Server.UrlEncode(cookieVal);
                DateTime dt = DateTime.Now;
                cookie.Expires = dt.Add(ts);
                // you may change this to a certain domain
                string host = HttpContext.Current.Request.Url.Host.ToLower();
                if (!host.Equals("localhost"))
                    cookie.Domain = host;

                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch
            {
            }
        }

        public static void SetSessionCookie(string cookieName, string cookieVal)
        {
            try
            {
                HttpCookie cookie = new HttpCookie(cookieName);
                cookie.Value = HttpContext.Current.Server.UrlEncode(cookieVal);
                // you may change this to a certain domain
                //string host = HttpContext.Current.Request.Url.Host.ToLower();
                //if (!host.Equals("localhost"))
                //    cookie.Domain = host;

                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch
            {
            }
        }

        public static bool IsCrawler()
        {
            HttpRequest request = HttpContext.Current.Request;
            if (request != null)
            {
                bool isCrawler = request.Browser.Crawler;
                if (!isCrawler)
                {
                    string whiteListedCrawlers = Application("WhiteListedCrawlers");
                    Regex regEx = new Regex(whiteListedCrawlers, RegexOptions.IgnoreCase);
                    isCrawler = regEx.Match(request.UserAgent).Success;
                }

                return isCrawler;
            }

            return true;
        }

        public static int getWebsiteSource()
        {
            //this function will check if user was landed on some other page directly (other than default, restaurant_are and restaurant list), 
            //then set one cookie for not redirecting him to mobilesite
            HttpRequest request = HttpContext.Current.Request;
            if (request != null)
            {
                string strFullUserAgent = request.UserAgent.ToString().ToLower();
                if (strFullUserAgent.Contains("nokia") || strFullUserAgent.Contains("samsung") || strFullUserAgent.Contains("lg-") ||
               strFullUserAgent.Contains("motorola") || strFullUserAgent.Contains("blackberry") || strFullUserAgent.Contains("iphone") ||
               strFullUserAgent.Contains("ipod") || strFullUserAgent.Contains("android") || strFullUserAgent.Contains("sonyericsson") ||
               strFullUserAgent.Contains("sie-") || strFullUserAgent.Contains("up.b") || strFullUserAgent.Contains("up/"))
                {

                    return 2;

                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        public static string SafeMapPath(string fname)
        {
            if (string.IsNullOrEmpty(fname) || Path.IsPathRooted(fname))
            {
                return fname;
            }

            string result = fname;

            //Try it as a virtual path. Try to map it based on the Request.MapPath to handle Medium trust level and "~/" paths automatically 
            try
            {
                result = HttpContext.Current.Request.MapPath(fname);
            }
            catch
            {
                //Didn't like something about the virtual path.
                //May be a drive path. See if it will expand to a valid path
                try
                {
                    //Try a GetFullPath. If the path is not virtual or has other malformed problems
                    //Return it as is
                    result = Path.GetFullPath(fname);
                }
                catch (NotSupportedException e1)
                {
                    // Contains a colon, probably already a full path.
                    return fname;
                }
                catch (SecurityException exc)
                {
                    //Path is somewhere you're not allowed to access or is otherwise damaged
                    throw new SecurityException("If you are running in Medium Trust you may have virtual directories defined that are not accessible at this trust level," + exc.Message);
                }
            }
            return result;
        }
        public static string QueryStringCanBeDangerousContent(string paramName)
        {
            string tmpS = string.Empty;
            if ((HttpContext.Current.Request.QueryString[paramName] != null))
            {
                try
                {
                    tmpS = HttpContext.Current.Request.QueryString[paramName].ToString();
                }
                catch
                {
                    tmpS = string.Empty;
                }
            }

            return tmpS;
        }
        public static string GetLang(string locale)
        {
            if (String.IsNullOrEmpty(locale))
                locale = "en-US";

            return locale.Equals("ar-KW") || locale.ToLower().Equals("ar") ? "ar" : "en";
        }

        public static string ConvertDateStringToSQL(string dateString)
        {
            int index1 = dateString.IndexOf('-');
            if (index1 < 3)
            {
                string[] datePart = dateString.Split('-');
                return datePart[2] + "-" + datePart[1] + "-" + datePart[0];
            }
            else
            {
                return dateString;
            }
        }

        public static string ConvertDateStringToCSharp(string dateString)
        {
            int index1 = dateString.IndexOf('-');
            if (index1 < 3)
            {
                return dateString;
            }
            else
            {
                string[] datePart = dateString.Split('-');
                return datePart[2] + "-" + datePart[1] + "-" + datePart[0];
            }

        }
    }
}