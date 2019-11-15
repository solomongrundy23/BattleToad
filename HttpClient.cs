using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BattleToad
{
    public class HttpClient
    {
        private string BaseURL;
        /// <summary>
        /// Создать экземпляр HttpClient
        /// </summary>
        /// <param name="BaseUrl">Базовый URL</param>
        HttpClient(string BaseUrl = "")
        {
            BaseURL = BaseUrl;
        }

        public void ChangeBaseUrl(string BaseUrl)
        {
            BaseURL = BaseUrl;
        }

        /// <summary>
        /// Ответ на запрос от HttpClient
        /// </summary>
        public class Response
        {
            /// <summary>
            /// Создает новый ответ
            /// </summary>
            /// <param name="code">Код ответ</param>
            /// <param name="text">Ответ в строке</param>
            /// <param name="error">Текст ошибки</param>
            public Response(HttpStatusCode code, string text, string error)
            {
                resp_statusCode = code;
                ResponseString = text;
                Error = error;
            }
            //Создает новый ответ только с ошибкой
            public Response(string error)
            {
                Error = error;
            }
            /// <summary>
            /// Код ответа
            /// </summary>
            public int StatusCode
            {
                get { return (int)resp_statusCode; }
            }
            /// <summary>
            /// Код ответа с расшифровкой
            /// </summary>
            public string StatusCodeString
            {
                get { return $"{((int)resp_statusCode).ToString()} {resp_statusCode}"; }
            }
            /// <summary>
            /// Текст ошибки запроса
            /// </summary>
            public readonly string Error;
            /// <summary>
            /// Текст ответа
            /// </summary>
            public readonly string ResponseString;
            private readonly HttpStatusCode resp_statusCode;
            /// <summary>
            /// Получить Код, Текст ошибки и Ответ в виде строки, например, для логирования
            /// </summary>
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
        /// <summary>
        /// Печеньки
        /// </summary>
        public CookieContainer mycookies = new CookieContainer();
        /// <summary>
        /// Отправить запрос GET
        /// </summary>
        /// <param name="url">Адрес</param>
        /// <returns></returns>
        public Response Get(string url)
        {
            return Send("Get", url);
        }
        /// <summary>
        /// Отправить запрос POST
        /// </summary>
        /// <param name="url">Адрес</param>
        /// <returns></returns>
        public Response Post(string url, string data)
        {
            return Send("Post", url, data);
        }
        /// <summary>
        /// Отправить запрос PUT
        /// </summary>
        /// <param name="url">Адрес</param>
        /// <returns></returns>
        public Response Put(string url, string data)
        {
            return Send("Put", url, data);
        }

        private Response Send(string method, string url, string data = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Headers.Add("charset", "utf-8");
                request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";
                request.CookieContainer = mycookies;
                request.Headers.Add("charset", "utf-8");
                if (data != null)
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