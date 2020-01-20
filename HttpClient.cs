using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BattleToad.FastHttpClient
{
    public class FastHttpClient
    {
        public ICredentials Credentials;
        public bool PreAuthenticate = false;
        public WebHeaderCollection Headers = new WebHeaderCollection();
        public string BaseURL;
        /// <summary>
        /// Создать экземпляр HttpClient
        /// </summary>
        /// <param name="BaseUrl">Базовый URL</param>
        public FastHttpClient(string BaseUrl = "")
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
                    if (ResponseString != null) stringBuilder.Append($"Ответ: {ResponseString}");
                    return stringBuilder.ToString();
                }
            }
        }
        /// <summary>
        /// Печеньки
        /// </summary>
        public CookieContainer mycookies = new CookieContainer();
        /// <summary>
        /// Отправить HTTP-запрос
        /// </summary>
        /// <param name="method">метод</param>
        /// <param name="url">адрес</param>
        /// <param name="data">данные</param>
        /// <returns></returns>
        public Response Send(string method, string url,
            string ContentType = ContentTypes.plain,
            string Accept = "", string data = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + url);
                request.Method = method.ToUpper();
                request.Headers = Headers;
                request.ContentType = ContentType;
                request.Accept = Accept;
                request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";
                request.CookieContainer = mycookies;
                request.Headers.Add("charset", "utf-8");
                if (Credentials != null) request.Credentials = Credentials;
                request.PreAuthenticate = PreAuthenticate;
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

        /// <summary>
        /// Отправить HTTP-запрос асинхронно
        /// </summary>
        /// <param name="method">метод</param>
        /// <param name="url">адрес</param>
        /// <param name="data">данные</param>
        /// <returns></returns>
        public async Task<Response> SendAsync(string method, string url,
            string ContentType = ContentTypes.plain,
            string Accept = "", string data = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + url);
                request.Method = method.ToUpper();
                request.Headers = Headers;
                request.ContentType = ContentType;
                request.Accept = Accept;
                request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";
                request.CookieContainer = mycookies;
                if (data != null)
                {
                    byte[] b_data = Encoding.UTF8.GetBytes(data);
                    request.ContentLength = b_data.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        await stream.WriteAsync(b_data, 0, b_data.Length);
                    }
                }
                try
                {
                    HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    return new Response(response.StatusCode, await reader.ReadToEndAsync(), null);
                }
                catch (WebException EX)
                {
                    HttpWebResponse response = EX.Response as HttpWebResponse;
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    return new Response(response.StatusCode, await reader.ReadToEndAsync(), EX.Message);
                }
            }
            catch (Exception EX)
            {
                return new Response(EX.Message);
            }
        }

        /// <summary>
        /// Список Content-Types с Википедии
        /// </summary>
        public static class ContentTypes
        {
            public const string atom_xml = "application/atom+xml";
            public const string EDIFACT = "application/EDIFACT";
            public const string EDI_X12 = "application/EDI-X12";
            public const string font_woff = "application/font-woff";
            public const string gzip = "application/gzip";
            public const string application_javascript = "application/javascript";
            public const string json = "application/json";
            public const string msword = "application/msword";
            public const string octet_stream = "application/octet-stream";
            public const string application_ogg = "application/ogg";
            public const string pdf = "application/pdf";
            public const string postscript = "application/postscript";
            public const string soap_xml = "application/soap+xml";
            public const string vnd_google_earth_kml_xml = "application/vnd.google-earth.kml+xml";
            public const string vnd_mozilla_xul_xml = "application/vnd.mozilla.xul+xml";
            public const string vnd_ms_excel = "application/vnd.ms-excel";
            public const string vnd_ms_powerpoint = "application/vnd.ms-powerpoint";
            public const string vnd_oasis_opendocument_graphics = "application/vnd.oasis.opendocument.graphics";
            public const string vnd_oasis_opendocument_presentation = "application/vnd.oasis.opendocument.presentation";
            public const string vnd_oasis_opendocument_spreadsheet = "application/vnd.oasis.opendocument.spreadsheet";
            public const string vnd_oasis_opendocument_text = "application/vnd.oasis.opendocument.text";
            public const string vnd_openxmlformats_officedocument_presentationml_presentation = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            public const string vnd_openxmlformats_officedocument_spreadsheetml_sheet = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            public const string vnd_openxmlformats_officedocument_wordprocessingml_document = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            public const string x_bittorrent = "application/x-bittorrent";
            public const string x_dvi = "application/x-dvi";
            public const string x_font_ttf = "application/x-font-ttf";
            public const string xhtml_xml = "application/xhtml+xml";
            public const string x_javascript = "application/x-javascript";
            public const string x_latex = "application/x-latex";
            public const string xml = "application/xml";
            public const string xml_dtd = "application/xml-dtd";
            public const string xop_xml = "application/xop+xml";
            public const string x_pkcs12 = "application/x-pkcs12";
            public const string x_pkcs7_certificates = "application/x-pkcs7-certificates";
            public const string x_pkcs7_certreqresp = "application/x-pkcs7-certreqresp";
            public const string x_pkcs7_mime = "application/x-pkcs7-mime";
            public const string x_pkcs7_signature = "application/x-pkcs7-signature";
            public const string x_rar_compressed = "application/x-rar-compressed";
            public const string x_shockwave_flash = "application/x-shockwave-flash";
            public const string x_stuffit = "application/x-stuffit";
            public const string x_tar = "application/x-tar";
            public const string x_tex = "application/x-tex";
            public const string x_www_form_urlencoded = "application/x-www-form-urlencoded";
            public const string zip = "application/zip";
            public const string aac = "audio/aac";
            public const string basic = "audio/basic";
            public const string L24 = "audio/L24";
            public const string audio_mp4 = "audio/mp4";
            public const string audio_mpeg = "audio/mpeg";
            public const string audio_ogg = "audio/ogg";
            public const string vnd_rn_realaudio = "audio/vnd.rn-realaudio";
            public const string vnd_wave = "audio/vnd.wave";
            public const string vorbis = "audio/vorbis";
            public const string audio_webm = "audio/webm";
            public const string x_ms_wax = "audio/x-ms-wax";
            public const string x_ms_wma = "audio/x-ms-wma";
            public const string gif = "image/gif";
            public const string jpeg = "image/jpeg";
            public const string pjpeg = "image/pjpeg";
            public const string png = "image/png";
            public const string svg_xml = "image/svg_xml";
            public const string tiff = "image/tiff";
            public const string vnd_microsoft_icon = "image/vnd_microsoft_icon";
            public const string vnd_wap_wbmp = "image/vnd_wap_wbmp";
            public const string webp = "image/webp";
            public const string http = "message/http";
            public const string imdn_xml = "message/imdn+xml";
            public const string partial = "message/partial";
            public const string rfc822 = "message/rfc822";
            public const string example = "model/example";
            public const string iges = "model/iges";
            public const string mesh = "model/mesh";
            public const string vrml = "model/vrml";
            public const string x3d_binary = "model/x3d_binary";
            public const string x3d_vrml = "model/x3d_vrml";
            public const string x3d_xml = "model/x3d_xml";
            public const string mixed = "multipart/mixed";
            public const string alternative = "multipart/alternative";
            public const string related = "multipart/related";
            public const string form_data = "multipart/form_data";
            public const string signed = "multipart/signed";
            public const string encrypted = "multipart/encrypted";
            public const string cmd = "text/cmd";
            public const string css = "text/css";
            public const string csv = "text/csv";
            public const string html = "text/html";
            public const string text_javascript = "text/javascript";
            public const string plain = "text/plain";
            public const string php = "text/php";
            public const string text_xml = "text/xml";
            public const string markdown = "text/markdown";
            public const string cache_manifest = "text/cache_manifest";
            public const string x_jquery_tmpl = "text/x_jquery_tmpl";
            public const string video_mpeg = "video/videompeg";
            public const string video_mp4 = "video/videomp4";
            public const string ogg = "video/videoogg";
            public const string quicktime = "video/videoquicktime";
            public const string video_webm = "video/videowebm";
            public const string x_ms_wmv = "video/videox-ms-wmv";
            public const string x_flv = "video/videox-flv";
            public const string video_3gpp = "video/video3gpp";
            public const string video_3gpp2 = "video/video3gpp2";
        }
    }
}