using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NumberTimeShow
{
    public static class GET
    {
        private static string BaseURL = @"https://www.kody.su/api/v2.1/search.xml?q={0}&key={1}";

        public class Response
        {
            internal Response(HttpStatusCode code, string text, string error)
            {
                resp_statusCode = code;
                ResponseString = text;
                Error = error;
            }
            internal Response(string error)
            {
                Error = error;
            }
            public int StatusCode
            {
                get { return (int)resp_statusCode; }
            }
            public string StatusCodeString
            {
                get { return $"{((int)resp_statusCode).ToString()} {resp_statusCode}"; }
            }
            public readonly string Error;
            public readonly string ResponseString;
            private readonly HttpStatusCode resp_statusCode;
            public string GetString
            {
                get
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    if (StatusCode != 0) stringBuilder.AppendLine($"Код: {StatusCodeString}");
                    if (Error != null) stringBuilder.AppendLine($"Ошибка: {Error}");
                    if (ResponseString != null) stringBuilder.AppendLine($"Ответ: {ResponseString}");
                    return stringBuilder.ToString();
                }
            }
        }

        private static CookieContainer mycookies = new CookieContainer();

        public static Response GetData(string number, string apikey)
        {
            return Get(string.Format(BaseURL, number, apikey));
        }

        private static Response Get(string url)
        {
            return Send(Method.GET, url);
        }

        private static Response Post(string url, string data)
        {
            return Send(Method.POST, url, data);
        }

        private static Response Put(string url, string data)
        {
            return Send(Method.PUT, url, data);
        }

        private enum Method { POST, PUT, GET };

        private static Response Send(Method method, string url, string data = "")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                switch (method)
                {
                    case Method.GET: request.Method = "Get"; break;
                    case Method.POST: request.Method = "Post"; break;
                    case Method.PUT: request.Method = "Put"; break;
                }
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Headers.Add("charset", "utf-8");
                request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";
                request.CookieContainer = mycookies;
                request.Headers.Add("charset", "utf-8");
                if (method != Method.GET)
                {
                    byte[] b_data = Encoding.UTF8.GetBytes(data);
                    request.ContentLength = b_data.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(b_data, 0, b_data.Length);
                    }
                }
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    return new Response(response.StatusCode, reader.ReadToEnd(), null);
                }
                catch (WebException EX)
                {
                    HttpWebResponse response = EX.Response as HttpWebResponse;
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    return new Response(response.StatusCode, reader.ReadToEnd(), EX.Message);
                }
            }
            catch (Exception EX)
            {
                return new Response(EX.Message);
            }
        }
    }
}