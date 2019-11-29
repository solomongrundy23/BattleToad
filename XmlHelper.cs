using System.IO;
using System.Xml.Serialization;

namespace BattleToad.XMLHelper
{
    /// <summary>
    /// Помощник для работы с XML
    /// </summary>
    internal static class XMLHelper
    {
        /// <summary>
        /// Перевести в XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXML<T>(this T obj) => Serialize(obj);
        /// <summary>
        /// В класс из XML
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="xml">XML</param>
        /// <returns></returns>
        public static T FromXML<T>(this string xml) => Deserialize<T>(xml);
        /// <summary>
        /// Класс в XML
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="obj">объект</param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, obj);
                return textWriter.ToString();
            }
        }
        /// <summary>
        /// Из XML в класс
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader textReader = new StringReader(xml))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }
    }
}
