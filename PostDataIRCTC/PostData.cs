using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using TomDataScrappingWinform;

namespace PostDataIRCTC
{
    public class PostData : WebClient
    {
        public static string Captcha;
        private CookieContainer m_container = new CookieContainer();
        static WebHeaderCollection headerCollection = new WebHeaderCollection();
        static CookieContainer cookieContainer = new CookieContainer();
        static int timeout = 0;
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = m_container;
            }
            return request;
        }


        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref uint pcchCookieData, int dwFlags, IntPtr lpReserved);
        const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        public static string GetGlobalCookies(string uri)
        {
            uint datasize = 1024;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (InternetGetCookieEx(uri, null, cookieData, ref datasize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero)
                && cookieData.Length > 0)
            {
                return cookieData.ToString(); //.Replace(';', ',');
            }
            else
            {
                return null;
            }
        }

        public static void SetCookie(string InputString)
        {
            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(InputString);
            for (int i = 0; i < headerCollection.Count; i++)
            {
                string key = headerCollection.GetKey(i);
                if (key == "Set-Cookie")
                {
                    key = "Cookie";
                }
                else
                {
                    continue;
                }
                string value = headerCollection.Get(i);
                request2.Headers.Add(key, value);
            }
            request2.Method = "GET";
            request2.KeepAlive = true;
            request2.Accept = "text/html, application/xhtml+xml, */*";
            request2.Headers.Add("Accept-Encoding", "gzip, deflate");
            request2.CookieContainer = cookieContainer;
            request2.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request2.Headers.Add("Accept-Language", "en-US");
            request2.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:29.0) Gecko/20100101 Firefox/29.0";

          
            Uri t2 = new Uri(InputString);

            WebResponse response2 = request2.GetResponse();
            StreamReader responseReader = new StreamReader(response2.GetResponseStream());
            string fullResponse = responseReader.ReadToEnd();
            response2.Close();


            string responseFromServer = fullResponse;
            HtmlDocument html = new HtmlDocument();
            html.OptionOutputAsXml = true;
            html.LoadHtml(fullResponse);
        }

        public static void FetchAllServices(string InputString, string TimeStamp, string Nonce, string Signature, ref List<Newtonsoft.Json.Linq.JToken> lstValServices, ref List<Newtonsoft.Json.Linq.JToken> lstHrefServices,ref Newtonsoft.Json.Linq.JArray lstServices)
        {
            try
            {
                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(InputString);
                for (int i = 0; i < headerCollection.Count; i++)
                {
                    string key = headerCollection.GetKey(i);
                    if (key == "Set-Cookie")
                    {
                        key = "Cookie";
                    }
                    else
                    {
                        continue;
                    }
                    string value = headerCollection.Get(i);
                    request2.Headers.Add(key, value);
                }
                request2.Method = "GET";
                request2.KeepAlive = true;
                request2.Accept = "*/*";
                request2.Headers.Add("Accept-Encoding", "gzip, deflate, sdch, br");
                request2.CookieContainer = cookieContainer;
                request2.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request2.Headers.Add("Accept-Language", "en-US");
                request2.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:29.0) Gecko/20100101 Firefox/29.0";
                request2.Headers.Add("Authorization", Signature);
                //request2.Headers.Add("Authorization", "OAuth realm=\"https://pos.shortcutssoftware.com/webapi/site/7966d291-1b12-dd11-a234-0050563fff01/services\", oauth_consumer_key=\"7BLI6f1oAGhfBBaUGp61\", oauth_token=\"uRXUDxGcEkzuuUi6X6Pa\", oauth_nonce=\"" + Nonce + "\", oauth_timestamp=\"" + TimeStamp + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_version=\"1.0\", oauth_signature=\"" + Uri.EscapeDataString(Signature) + "\"");

                Uri t2 = new Uri(InputString);

                WebResponse response2 = request2.GetResponse();
                StreamReader responseReader = new StreamReader(response2.GetResponseStream());
                string fullResponse = responseReader.ReadToEnd();
                response2.Close();

                string responseFromServer = fullResponse;
                dynamic obj = JsonConvert.DeserializeObject(responseFromServer);
                lstServices = obj.services;
                //foreach (Newtonsoft.Json.Linq.JObject objService in lstServices)
                //{
                //    foreach (System.Collections.Generic.KeyValuePair<string, Newtonsoft.Json.Linq.JToken> keyVal in objService)
                //    {
                //        if (keyVal.Key == "display_name")
                //        {
                //            lstValServices.Add(keyVal.Value);
                //        }
                //        if (keyVal.Key == "href")
                //        {
                //            lstHrefServices.Add(keyVal.Value);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {

            }
        }

        public static void FetchAllEmployeePricing(string InputString, string TimeStamp, string Nonce, string Signature, ref List<Newtonsoft.Json.Linq.JToken> lstValServices, ref List<Newtonsoft.Json.Linq.JToken> lstHrefServices, ref Newtonsoft.Json.Linq.JArray lstServices)
        {
            try
            {
                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(InputString);
                for (int i = 0; i < headerCollection.Count; i++)
                {
                    string key = headerCollection.GetKey(i);
                    if (key == "Set-Cookie")
                    {
                        key = "Cookie";
                    }
                    else
                    {
                        continue;
                    }
                    string value = headerCollection.Get(i);
                    request2.Headers.Add(key, value);
                }
                request2.Method = "GET";
                request2.KeepAlive = true;
                request2.Accept = "*/*";
                request2.Headers.Add("Accept-Encoding", "gzip, deflate, sdch, br");
                request2.CookieContainer = cookieContainer;
                request2.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request2.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                request2.Headers.Add("Origin", "https://www.ulta.com");
                request2.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:29.0) Gecko/20100101 Firefox/29.0";
                Signature = Signature.Replace("https://pos.shortcutssoftware.com/webapi/site/7966d291-1b12-dd11-a234-0050563fff01/services",InputString);
                request2.Headers.Add("Authorization", Signature);
                //request2.Headers.Add("Authorization", "OAuth realm=\"https://pos.shortcutssoftware.com/webapi/site/7966d291-1b12-dd11-a234-0050563fff01/services\", oauth_consumer_key=\"7BLI6f1oAGhfBBaUGp61\", oauth_token=\"uRXUDxGcEkzuuUi6X6Pa\", oauth_nonce=\"" + Nonce + "\", oauth_timestamp=\"" + TimeStamp + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_version=\"1.0\", oauth_signature=\"" + Uri.EscapeDataString(Signature) + "\"");

                Uri t2 = new Uri(InputString);

                WebResponse response2 = request2.GetResponse();
                StreamReader responseReader = new StreamReader(response2.GetResponseStream());
                string fullResponse = responseReader.ReadToEnd();
                response2.Close();

                string responseFromServer = fullResponse;
                dynamic obj = JsonConvert.DeserializeObject(responseFromServer);
                lstServices = obj.employees;
                //foreach (Newtonsoft.Json.Linq.JObject objService in lstServices)
                //{
                //    foreach (System.Collections.Generic.KeyValuePair<string, Newtonsoft.Json.Linq.JToken> keyVal in objService)
                //    {
                //        if (keyVal.Key == "display_name")
                //        {
                //            lstValServices.Add(keyVal.Value);
                //        }
                //        if (keyVal.Key == "href")
                //        {
                //            lstHrefServices.Add(keyVal.Value);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {

            }
        }

        public static string FetchAllEmployeeAppointments(string InputString, string TimeStamp, string Nonce, string Signature, ref List<Newtonsoft.Json.Linq.JToken> lstValServices, ref List<Newtonsoft.Json.Linq.JToken> lstHrefServices, ref Newtonsoft.Json.Linq.JArray lstServices,string PrevURL)
        {
            try
            {
                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(InputString);
                for (int i = 0; i < headerCollection.Count; i++)
                {
                    string key = headerCollection.GetKey(i);
                    if (key == "Set-Cookie")
                    {
                        key = "Cookie";
                    }
                    else
                    {
                        continue;
                    }
                    string value = headerCollection.Get(i);
                    request2.Headers.Add(key, value);
                }
                request2.Method = "POST";
                request2.ContentType = "application/json";
                request2.KeepAlive = true;
                request2.Accept = "*/*";
                request2.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                request2.CookieContainer = cookieContainer;
                request2.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request2.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                request2.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:29.0) Gecko/20100101 Firefox/29.0";
                request2.Headers.Add("Authorization", Signature);
                request2.Referer = "https://www.ulta.com/bookonline/availableTimes.html";
                using (var streamWriter = new StreamWriter(request2.GetRequestStream()))
                {
                    string json = "{\"date_time_filter\":[{\"days_of_week\":[\"Monday\",\"Tuesday\",\"Wednesday\",\"Thursday\",\"Friday\",\"Saturday\",\"Sunday\"],\"from_date\":\"2016-10-29\",\"to_date\":\"2016-11-11\",\"start_time\":\"00:00:00\",\"finish_time\":\"23:59:59\"}],\"maximum_availabilities_count\":500,\"requested_services\":[{\"gender_code\":\"unknown\",\"links\":[{\"rel\":\"site/service\",\"href\":\"" + PrevURL.Replace("/employee_pricing","") + "\"}]}]}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)request2.GetResponse();
                string result = string.Empty;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                dynamic obj = JsonConvert.DeserializeObject(result);
                lstServices = obj.available_appointments;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "true";
        }

        public static void FetchAvailableSlots(string InputString)
        {
            try
            {
                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(InputString);
                for (int i = 0; i < headerCollection.Count; i++)
                {
                    string key = headerCollection.GetKey(i);
                    if (key == "Set-Cookie")
                    {
                        key = "Cookie";
                    }
                    else
                    {
                        continue;
                    }
                    string value = headerCollection.Get(i);
                    request2.Headers.Add(key, value);
                }
                request2.Method = "GET";
                request2.KeepAlive = true;
                request2.Accept = "*/*";
                request2.Headers.Add("Accept-Encoding", "gzip, deflate, sdch, br");
                request2.CookieContainer = cookieContainer;
                request2.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request2.Headers.Add("Accept-Language", "en-US");
                request2.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:29.0) Gecko/20100101 Firefox/29.0";
                //request2.Headers.Add("Authorization", "OAuth realm=\"https://pos.shortcutssoftware.com/webapi/site/7966d291-1b12-dd11-a234-0050563fff01/services\", oauth_consumer_key=\"7BLI6f1oAGhfBBaUGp61\", oauth_token=\"uRXUDxGcEkzuuUi6X6Pa\", oauth_nonce=\"" + Nonce + "\", oauth_timestamp=\"" + TimeStamp + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_version=\"1.0\", oauth_signature=\"" + Uri.EscapeDataString(Signature) + "\"");

                Uri t2 = new Uri(InputString);

                WebResponse response2 = request2.GetResponse();
                StreamReader responseReader = new StreamReader(response2.GetResponseStream());
                string fullResponse = responseReader.ReadToEnd();
                response2.Close();

                string responseFromServer = fullResponse;
                //foreach (Newtonsoft.Json.Linq.JObject objService in lstServices)
                //{
                //    foreach (System.Collections.Generic.KeyValuePair<string, Newtonsoft.Json.Linq.JToken> keyVal in objService)
                //    {
                //        if (keyVal.Key == "display_name")
                //        {
                //            lstValServices.Add(keyVal.Value);
                //        }
                //        if (keyVal.Key == "href")
                //        {
                //            lstHrefServices.Add(keyVal.Value);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {

            }
        }
    }
}
