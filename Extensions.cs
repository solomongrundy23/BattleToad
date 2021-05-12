using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BattleToad.Extensions
{
    /// <summary>
    /// Расширения для различных классов
    /// </summary>
    public static class Extensions
    {        /// <summary>
             /// Вывести в консоль значение переменной
             /// </summary>
             /// <typeparam name="T"></typeparam>
             /// <param name="obj"></param>
        public static void ConsoleWrite<T>(this T obj) => Console.Write(obj);
        /// <summary>
        /// Вывести в консоль значение переменной с переходом на новую строку
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void ConsoleWriteLine<T>(this T obj) => Console.WriteLine(obj);

        public static string RemoveText(this string str, string text) => str.Replace(text, "");
        public static string RemoveTextStartEnd(this string str, string text) => str.RemoveTextStart(text).RemoveTextEnd(text);
        public static string RemoveTextStart(this string str, string text) => str.StartsWith(text) ? str.Substring(text.Length) : text;
        public static string RemoveTextEnd(this string str, string text) => str.EndsWith(text) ? str.Substring(0, str.Length - text.Length) : text;
        /// <summary>
        /// Возвращается выравненную строку до определенной длины
        /// </summary>
        /// <param name="str">исходная  строка</param>
        /// <param name="length">желаемая длина</param>
        /// <returns></returns>
        public static string ToStringAndPad(this int num, int length, char ch = ' ') => num.ToString().Pad(length, ' ');
        /// <summary>
        /// Возвращается выравненную указанным символом строку до определенной длины
        /// </summary>
        /// <param name="str">исходная  строка</param>
        /// <param name="length">желаемая длина</param>
        /// <returns></returns>
        public static string Pad(this string str, int length, char ch = ' ')
        {
            if (length - str.Length <= 0) return str;
            return str.PadLeft(str.Length + (length - str.Length) / 2, ch).PadRight(length, ch);
        }
        /// <summary>
        /// Получить строку повторение данной строки
        /// </summary>
        /// <param name="str">строка</param>
        /// <param name="repeat">количество повторений</param>
        /// <returns>новая строка</returns>
        public static string Repeat(this string str, int repeat) => Addons.StringRepeat(str, repeat);
        /// <summary>
        /// Распарсить дату и время
        /// </summary>
        /// <param name="str">строка</param>
        /// <returns>DateTime, null - при неуспешной операции</returns>
        public static DateTime? DateTimeParse(this string str) => Addons.DateTimeParse(str);
        /// <summary>
        /// Получить n символов с конца строки
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="length">количество символов с конца</param>
        /// <returns></returns>
        public static string SubStringFromEnd(this string text, int length) => SubStringFromEnd(text, length);
        /// <summary>
        /// Добавить в начало и конец строки строки
        /// </summary>
        /// <param name="text">исходный текст</param>
        /// <param name="start">текст в начало</param>
        /// <param name="end">текст в конец</param>
        /// <returns></returns>
        public static string Wrap(this string text, string start, string end)
            => $"{start}{text}{end}";
        //String
        /// <summary>
        /// Быстро и безболезнено перевести в int
        /// </summary>
        /// <param name="text"></param>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static int ToIntOrDefault(this string text, int default_value = int.MinValue)
           => Addons.ToIntOrDeafault(text, default_value);
        /// <summary>
        /// Быстро перевести в int, если не удается, то вернёт Null
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int? ToInt(this string text, int default_value = int.MinValue)
           => Addons.ToInt(text, default_value);
        /// <summary>
        /// Получить Base64 из строки
        /// </summary>
        /// <param name="text">текст</param>
        /// <returns></returns>
        public static string GetBase64String(this string text)
            => GetBase64String(text, Encoding.Default);
        /// <summary>
        /// Получить Base64 из строки
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="encoding">Кодировка</param>
        /// <returns></returns>
        public static string GetBase64String(this string text, Encoding encoding)
            => encoding.GetBytes(text).GetBase64String();
        /// <summary>
        /// Быстро и безболезнено перевести в long
        /// </summary>
        /// <param name="text"></param>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static long ToLong(this string text, long default_value = long.MinValue)
            => Addons.ToLong(text, default_value);
        /// <summary>
        /// Перевести кириллицу в транслит
        /// </summary>
        /// <param name="text">строка</param>
        /// <returns></returns>
        public static string ToTraslite(this string text) => Translitiration.ConvertToLatin(text);
        /// <summary>
        /// Разбивает текст на массив строк
        /// </summary>
        /// <param name="text">Строка</param>
        /// <param name="withEmpty">Пропустить пустые строки</param>
        /// <returns></returns>
        public static string[] GetLines(this string text, bool withEmpty = true)
            => Addons.GetLines(text, withEmpty);
        /// <summary>
        /// Разбить текст на слова
        /// Любой не буквенный символ - разделитель
        /// </summary>
        /// <param name="text">Строка</param>
        /// <returns></returns>
        public static string[] GetWords(this string text)
            => Regex.Split(text, @"[^(\w)|\-)]").Where(x => x != string.Empty).ToArray();
        /// <summary>
        /// Совпадает ли с регулярым выражением
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="matcher">регулярное выражение</param>
        /// <returns></returns>
        public static bool IsRegexMatch(this string text, string matcher) => Regex.IsMatch(text, matcher);
        /// <summary>
        /// Получить только цифры из строки
        /// </summary>
        /// <param name="text">Строка</param>
        /// <returns></returns>
        public static string GetJustNums(this string text)
            => Regex.Replace(text, @"[^\d]", "");
        public static string GetJustLetters(this string text)
            => Regex.Replace(text, @"([^\w])|\d", "");
        /// <summary>
        /// Обрезать строку до определенного количества символов
        /// </summary>
        /// <param name="text">Строк</param>
        /// <param name="count">Количество символов</param>
        /// <param name="end">что поставить в конце, по умолчанию многоточие</param>
        /// <returns></returns>
        /// <summary>
        /// Перевести в String
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="splitter">символ для стыковки</param>
        /// <returns></returns>
        public static string ToText<T>(this T[] array, string splitter = "\r\n")
               => string.Join(splitter, array);
        public static string Cut(this string text, int count, string end = "...")
            => text.Length > count ? text.Substring(0, count) + end : text;
        /// <summary>
        /// Является ли строка IP-адресом(IPv4)
        /// </summary>
        /// <param name="text">строка</param>
        /// <returns></returns>
        public static bool IsIP(this string text)
            => Addons.IsIP(text);
        /// <summary>
        /// Получить хэш от строки, Type указывает на тип хэша по умолчанию Hash.Type.MD5
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="Type">тип хэша</param>
        /// <returns></returns>
        public static string GetHashString(this string text, Hash.Type Type = Hash.Type.MD5)
            => Hash.GetHash(Encoding.Unicode.GetBytes(text), Type);
        /// <summary>
        /// Найди значения по регулярному выражения
        /// </summary>
        /// <param name="DATA">строка</param>
        /// <param name="TAG">регулярное выражение</param>
        /// <param name="registr">регистрозависимость</param>
        /// <param name="singleline">считать весь текст единой строкой</param>
        /// <param name="group">вывести группу</param>
        /// <returns></returns>
        public static List<string> FindByRegex(this string DATA, string TAG, bool registr = false,
                                               bool singleline = false, int group = 0)
            => Addons.FindByRegex(DATA, TAG, registr, singleline, group);
        /// <summary>
        /// Сравнить со строкой
        /// </summary>
        /// <param name="string_data">Строка</param>
        /// <param name="text">Строка для сравнения</param>
        /// <returns></returns>
        public static bool EqualString(this string this_string, string other_string)
            => string.Equals(this_string, other_string, StringComparison.Ordinal);
        /// <summary>
        /// Сравнить со строкой, игнорируя регистр
        /// </summary>
        /// <param name="string_data">Строка</param>
        /// <param name="text">Строка для сравнения</param>
        /// <returns></returns>
        public static bool EqualStringIgnoreCase(this string this_string, string other_string)
        => string.Equals(this_string, other_string, StringComparison.OrdinalIgnoreCase);
        /// <summary>
        /// Получить последние count символов из строки
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="count">количество символов</param>
        /// <returns></returns>
        public static string FromTheEnd(this string text, int count)
        {
            return
            count >= text.Length || text.Length == 0
            ?
            text
            :
            text.Substring(text.Length - count);
        }
        //String[] and List<string>
        public static string ToText(this List<string> text, string splitter = "\r\n")
            => string.Join(splitter, text);
        /// <summary>
        /// Получить только те строки, которые содержат подстроку
        /// </summary>
        /// <param name="lines">массив строк</param>
        /// <param name="filter">подстрока</param>
        /// <returns></returns>
        public static string[] FilterContains(this string[] lines, string filter)
            => lines.Where(x => Regex.IsMatch(x, filter)).ToArray();
        /// <summary>
        /// Получить только те строки, которые содержат подстроку
        /// </summary>
        /// <param name="lines">список строк</param>
        /// <param name="filter">подстрока</param>
        /// <returns></returns>
        public static string[] FilterContains(this List<string> lines, string filter)
            => lines.Where(x => Regex.IsMatch(x, filter)).ToArray();
        /// <summary>
        /// Получить только те строки, которые удолетворяют регулярному выражению
        /// </summary>
        /// <param name="lines">массив строк</param>
        /// <param name="filter">регулярное выражение</param>
        /// <returns></returns>
        public static string[] FilterRegex(this string[] lines, string filter)
            => lines.Where(x => Regex.IsMatch(x, filter)).ToArray();
        /// <summary>
        /// Получить только те строки, которые удолетворяют регулярному выражению
        /// </summary>
        /// <param name="lines">список строк</param>
        /// <param name="filter">регулярное выражение</param>
        /// <returns></returns>
        public static string[] FilterRegex(this List<string> lines, string filter)
            => lines.Where(x => Regex.IsMatch(x, filter)).ToArray();
        //Dictionary
        /// <summary>
        /// Перевести в String
        /// </summary>
        /// <param name="dictionary">словарь</param>
        /// <param name="KeyValueSplitter">символ(ы) для стыковки</param>
        /// <returns></returns>
        public static string ToText<K, V>(this Dictionary<K, V> dictionary,
            string KeyValueSplitter = "\t")
            => string.Join(Environment.NewLine,
                dictionary.Select(x => $"{x.Key}{KeyValueSplitter}{x.Value}").ToArray());
        //<T>
        /// <summary>
        /// Вывести значение
        /// </summary>
        /// <param name="value">пересенная</param>
        /// <param name="key">указать имя переменной</param>
        /// <returns></returns>
        public static string PrintValue<T>(this T value, string key = "value",
            bool multi_line_value = false)
            =>
            multi_line_value ?
            $"{key}=\"{Environment.NewLine}{value}{Environment.NewLine}\""
            :
            $"{key}=\"{value}\"";
        /// <summary>
        /// Вывести значение c типом переменной
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string PrintValueWithType<T>(this T value, string key = "value")
            => $"{typeof(T)}: {key}=\"{value}\"";
        /// <summary>
        /// Вывести переменные класса
        /// </summary>
        /// <param name="obj">класс</param>
        /// <param name="ShowPrivate">отображать приватные</param>
        /// <param name="ShowTypes">отображать тип переменных</param>
        /// <returns></returns>
        public static string[] PrintClassValues<T>(this T obj, bool Sort = false, bool ShowPrivate = false,
            bool ShowTypes = false)
            => Addons.PrintValuesInClass<T>(obj, Sort, ShowPrivate, ShowTypes);
        /// <summary>
        /// Получить Base64 из массива байт
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="encoding">Кодировка</param>
        /// <returns></returns>
        public static string GetBase64String(this byte[] data)
            => Convert.ToBase64String(data);
    }
}
