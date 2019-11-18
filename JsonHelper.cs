using System.Web.Script.Serialization;

namespace JSONHelper
{
    /// <summary>
    /// Помощник для работы с JSON
    /// </summary>
    internal static class JSONHelper
    {
        /// <summary>
        /// Перевести в JSON
        /// </summary>>
        /// <param name="obj">класс</param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj) => JSONHelper.Serialize(obj);
        /// <summary>
        /// В класс из JSON
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="json">JSON</param>
        /// <returns></returns>
        public static T FromJson<T>(this string json) => JSONHelper.Deserialize<T>(json);
        /// <summary>
        /// Класс в JSON
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="obj">объект</param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
        /// <summary>
        /// Из JSON в класс
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="json">JSON</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }
    }
}
