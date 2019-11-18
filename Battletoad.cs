using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace BattleToad
{
    /// <summary>
    /// Упрощение доступа к List<string>
    /// </summary>
    public class Strings : List<string>
    {
        /// <summary>
        /// Создать экземпляр Strings
        /// </summary>
        public Strings()
        {
        }
        /// <summary>
        /// Создать экземпляр Strings, загрузив из файла
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        public Strings(string FileName)
        {
            FromFile(FileName);
        }
        /// <summary>
        /// Загрузить из файла
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="EmptyStrings">Добавлять ли пустые строки</param>
        public void FromFile(string FileName, bool EmptyStrings = true)
        {
            try
            {
                string[] FileData = File.ReadAllLines(FileName);
                this.Clear();
                if (EmptyStrings)
                    this.AddRange(FileData);
                else
                    this.AddRange(FileData.Where(x => x != ""));
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки файла: {ex.Message}");
            }
        }
        /// <summary>
        /// Сохранить строки в файл
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        public void ToFile(string FileName)
        {
            try
            {
                File.WriteAllLines(FileName, this);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения файла: {ex.Message}");
            }
        }
        /// <summary>
        /// Дописать строки в конец файла
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        public void ToFile_Append(string FileName)
        {
            try
            {
                File.AppendAllLines(FileName, this);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения файла: {ex.Message}");
            }
        }
    }
    /// <summary>
    /// Цивилизованная многопоточная очередь
    /// </summary>
    public class CivilizedQueue : ConcurrentQueue<string> { }

    public static class Extensions
    {
        //String
        /// <summary>
        /// Быстро и безболезнено перевести в int
        /// </summary>
        /// <param name="text"></param>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static int ToInt(this string text, int default_value = int.MinValue)
           => Addons.ToInt(text, default_value);
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
            => text.Split(
                new[] { "\r\n", "\r", "\n" },
                withEmpty ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries
               );
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
        /// Проверить строку, если это IP-адрес, то выдать версию. 
        /// Результат: "ipv4", "ipv6" и null
        /// </summary>
        /// <param name="text">строка</param>
        /// <returns>"ipv4", "ipv6" и null</returns>
        public static string GetIPVersion(this string text)
            => Addons.IsIP_Ext(text);
        /// <summary>
        /// Получить хэш от строки, Type указывает на тип хэша по умолчанию Hash.Type.MD5
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="Type">тип хэша</param>
        /// <returns></returns>
        public static string GetHashString(this string text, Hash.Type Type = Hash.Type.MD5)
            => Hash.GetHash(Encoding.UTF8.GetBytes(text), Type);
        /// <summary>
        /// Добавлять в начало строки символ, пока строка меньше указанной длины
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="addChar">символ для добавления</param>
        /// <param name="length">желаемая длина</param>
        /// <returns></returns>
        public static string AddToStartWhileLengthNotValid(this string text, char addChar, int length)
        {
            while (text.Length < length)
                text = addChar + text;
            return text;
        }
        /// <summary>
        /// Добавлять в конец строки символ, пока строка меньше указанной длины
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="addChar">символ для добавления</param>
        /// <param name="length">желаемая длина</param>
        /// <returns></returns>
        public static string AddToEndWhileLengthNotValid(this string text, char addChar, int length)
        {
            while (text.Length < length)
                text = text + addChar;
            return text;
        }
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
        /// <summary>
        /// Перевести в String
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="splitter">символ для стыковки</param>
        /// <returns></returns>
        public static string ToText(this string[] text, string splitter = "\r\n")
               => string.Join(splitter, text);
        /// <summary>
        /// Перевести в String
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="splitter">символ для стыковки</param>
        /// <returns></returns>
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
        public static string ToText<K, V>(this Dictionary<K, V> dictionary, string KeyValueSplitter = "\t")
            => string.Join(Environment.NewLine, dictionary.Select(x => $"{x.Key}{KeyValueSplitter}{x.Value}").ToArray());

        //<T>
        /// <summary>
        /// Вывести значение
        /// </summary>
        /// <param name="value">пересенная</param>
        /// <param name="key">указать имя переменной</param>
        /// <returns></returns>
        public static string PrintValue<T>(this T value, string key = "value")
            => $"{key}=\"{value}\"";
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
        public static string[] PrintClassValues<T>(this T obj, bool ShowPrivate = false, bool ShowTypes = false)
            => Addons.PrintValuesInClass<T>(obj, ShowPrivate, ShowTypes);
    }

    /// <summary>
    /// Класс для логирования
    /// </summary>
    public class Log
    {
        private readonly string FileName;
        private Thread WritterThread;
        private readonly ConcurrentQueue<string> LogList = new ConcurrentQueue<string>();
        /// <summary>
        /// Создать логирование
        /// </summary>
        /// <param name="LogFile">путь к файлу лога</param>
        public Log(string LogFile)
        {
            FileName = LogFile;
            WritterThread = new Thread(Writter)
            {
                IsBackground = true
            };
            WritterThread.Start();
        }
        ~Log()
        {
            if (WritterThread.IsAlive) WritterThread.Abort();
            while (LogList.Count > 0)
            {
                if (LogList.TryDequeue(out string log_string))
                {
                    try
                    {
                        File.AppendAllText(FileName, log_string);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Ошибка логирования - {ex.Message}");
                    }
                }
            }
        }
        private void WriteLog(string log) => LogList.Enqueue(log);
        private void Writter()
        {
            while (true)
            {
                if (LogList.TryDequeue(out string log_string))
                {
                    try
                    {
                        File.AppendAllText(FileName, log_string);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Ошибка логирования - {ex.Message}");
                    }
                }
            }
        }
        private string LogString(string log)
            => $"{Addons.GetNow()}:{Environment.NewLine}{log}{Environment.NewLine}{Environment.NewLine}";
        /// <summary>
        /// Записать строку в лог
        /// </summary>
        /// <param name="log">данные для лога</param>
        public void Write(string log)
        {
            WriteLog(LogString(log));
        }
        /// <summary>
        /// Записать массив строк в лог
        /// </summary>
        /// <param name="log">массив данных для лога</param>
        public void Write(string[] log)
        {
            Write(log.ToText());
        }
        /// <summary>
        /// Записать список строк в лог
        /// </summary>
        /// <param name="log">список данных для лога</param>
        public void Write(List<string> log)
        {
            Write(log.ToText());
        }
        /// <summary>
        /// Записать значение класса
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="obj">объект</param>
        public void WriteClass<T>(T obj)
        {
            obj.PrintClassValues(true, true);
        }
    }

    /// <summary>
    /// Класс для упрощения работы с хэшированием
    /// </summary>
    public static class Hash
    {
        /// <summary>
        /// Алгоритм хэширования
        /// </summary>
        public enum Type { MD5, SHA256, SHA512, SHA1, SHA384 };
        /// <summary>
        /// Получить ХЭШ из массивая байт
        /// </summary>
        /// <param name="bytes">массив байт</param>
        /// <param name="type">алгоритм</param>
        /// <returns></returns>
        public static string GetHash(byte[] bytes, Type type = Type.MD5)
        {
            HashAlgorithm algorithm;
            switch (type)
            {
                case Type.MD5: algorithm = new MD5CryptoServiceProvider(); break;
                case Type.SHA256: algorithm = new SHA256CryptoServiceProvider(); break;
                case Type.SHA512: algorithm = new SHA512CryptoServiceProvider(); break;
                case Type.SHA1: algorithm = new SHA1CryptoServiceProvider(); break;
                case Type.SHA384: algorithm = new SHA384CryptoServiceProvider(); break;
                default: algorithm = new MD5CryptoServiceProvider(); break;
            }
            bytes = algorithm.ComputeHash(bytes);
            var s = new StringBuilder();
            foreach (var b in bytes)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            algorithm.Dispose();
            return s.ToString();
        }
        /// <summary>
        /// Получить ХЭШ из строки
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="type">алгоритм</param>
        /// <returns></returns>
        public static string GetHashFromString(string text, Type type = Type.MD5)
            => GetHash(Encoding.UTF8.GetBytes(text), type);
        /// <summary>
        /// Получить ХЭШ от времени сейчас
        /// </summary>
        /// <param name="type">алгоритм</param>
        /// <returns></returns>
        public static string GetHashFromDateTime(Type type = Type.MD5)
            => GetHashFromString(Addons.GetNow(), type);
        /// <summary>
        /// Получить ХЭШ файла
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <returns></returns>
        public static string ComputeFileChecksum(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                SHA512 sha512 = new SHA512CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                byte[] bytes = sha512.ComputeHash(fileData);
                var s = new StringBuilder();
                foreach (var b in bytes)
                {
                    s.Append(b.ToString("x2").ToLower());
                }
                return s.ToString();
            }
        }
        /// <summary>
        /// Получить ХЭШ файла ассинхронно
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <returns></returns>
        public static async Task<string> ComputeFileChecksumAsync(string path)
        {
            Task<string> Getter = new Task<string>(() => ComputeFileChecksum(path));
            Getter.Start();
            return await Getter;
        }
    }

    /// <summary>
    /// Дополнительный функционал
    /// </summary>
    public static class Addons
    {
        /// <summary>
        /// Быстро и безболезнено перевести в int
        /// </summary>
        /// <param name="text"></param>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static int ToInt(string text, int default_value = int.MinValue)
            => int.TryParse(text, out int x) ? x : default_value;
        /// <summary>
        /// Быстро и безболезнено перевести в long
        /// </summary>
        /// <param name="text"></param>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static long ToLong(string text, long default_value = long.MinValue) 
            => long.TryParse(text, out long x) ? x : default_value;
        /// <summary>
        /// Вывести переменные класса
        /// </summary>
        /// <param name="obj">класс</param>
        /// <param name="ShowPrivate">отображать приватные</param>
        /// <param name="ShowTypes">отображать тип переменных</param>
        /// <returns></returns>
        public static string[] PrintValuesInClass<T>(T obj, bool ShowPrivate = false, bool ShowTypes = false)
        {
            BindingFlags flags = ShowPrivate ?
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                :
                BindingFlags.Public | BindingFlags.Instance;
            FieldInfo[] fi = obj.GetType().GetFields(flags);
            string[] result = ShowTypes ?
               fi.Select(x => $"{x.Name}=\"{x.GetValue(obj)}\"").ToArray()
               :
               fi.Select(x => $"{x.FieldType}: {x.Name}=\"{x.GetValue(obj)}\"").ToArray();
            return result;
        }
        /// <summary>
        /// Запущена ли программа от Администратора
        /// </summary>
        /// <returns>True, если администратор</returns>
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (identity != null)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }
        /// <summary>
        /// Получить время в региональном формате
        /// </summary>
        /// <param name="Culture">регион</param>
        /// <returns></returns>
        public static string GetNow(string Culture = "Ru-ru")
            => DateTime.Now.ToString(new CultureInfo(Culture));
        /// <summary>
        /// Найди значения по регулярному выражения
        /// </summary>
        /// <param name="DATA">строка</param>
        /// <param name="TAG">регулярное выражение</param>
        /// <param name="registr">регистрозависимость</param>
        /// <param name="singleline">считать весь текст единой строкой</param>
        /// <param name="group">вывести группу</param>
        /// <returns></returns>
        public static List<string> FindByRegex(string DATA, string TAG, bool registr = false,
                                               bool singleline = false, int group = 0)
        {
            RegexOptions ro = singleline ? RegexOptions.Singleline : RegexOptions.Multiline;
            ro = !registr ? ro | RegexOptions.IgnoreCase : ro;
            MatchCollection re = Regex.Matches(DATA, TAG, ro);
            List<string> result = new List<string>();
            foreach (Match rem in re)
                result.Add(rem.Groups[group].ToString());
            return result;
        }
        /// <summary>
        /// Является ли строка IP-адресом(IPv4)
        /// </summary>
        /// <param name="text">строка</param>
        /// <returns></returns>
        public static bool IsIP(string text)
            => Regex.IsMatch(text, "^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
        /// <summary>
        /// Проверить строку, если это IP-адрес, то выдать версию. 
        /// Результат: "ipv4", "ipv6" и null
        /// </summary>
        /// <param name="value">строка</param>
        /// <returns>"ipv4", "ipv6" и null</returns>
        public static string IsIP_Ext(string value)
        {
            IPAddress address;
            if (IPAddress.TryParse(value, out address))
            {
                switch (address.AddressFamily)
                {
                    case System.Net.Sockets.AddressFamily.InterNetwork:
                        return "ipv4";
                    case System.Net.Sockets.AddressFamily.InterNetworkV6:
                        return "ipv6";
                    default:
                        return null;
                }
            }
            else
                return null;
        }
    }

    /// <summary>
    /// В транслит
    /// </summary>
    internal static class Translitiration
    {
        private static readonly Dictionary<char, string> ConvertedLetters = new Dictionary<char, string>
            {
                {'а', "a"},
                {'б', "b"},
                {'в', "v"},
                {'г', "g"},
                {'д', "d"},
                {'е', "e"},
                {'ё', "yo"},
                {'ж', "zh"},
                {'з', "z"},
                {'и', "i"},
                {'й', "j"},
                {'к', "k"},
                {'л', "l"},
                {'м', "m"},
                {'н', "n"},
                {'о', "o"},
                {'п', "p"},
                {'р', "r"},
                {'с', "s"},
                {'т', "t"},
                {'у', "u"},
                {'ф', "f"},
                {'х', "h"},
                {'ц', "c"},
                {'ч', "ch"},
                {'ш', "sh"},
                {'щ', "sch"},
                {'ъ', "j"},
                {'ы', "i"},
                {'ь', "j"},
                {'э', "e"},
                {'ю', "yu"},
                {'я', "ya"},
                {'А', "A"},
                {'Б', "B"},
                {'В', "V"},
                {'Г', "G"},
                {'Д', "D"},
                {'Е', "E"},
                {'Ё', "Yo"},
                {'Ж', "Zh"},
                {'З', "Z"},
                {'И', "I"},
                {'Й', "J"},
                {'К', "K"},
                {'Л', "L"},
                {'М', "M"},
                {'Н', "N"},
                {'О', "O"},
                {'П', "P"},
                {'Р', "R"},
                {'С', "S"},
                {'Т', "T"},
                {'У', "U"},
                {'Ф', "F"},
                {'Х', "H"},
                {'Ц', "C"},
                {'Ч', "Ch"},
                {'Ш', "Sh"},
                {'Щ', "Sch"},
                {'Ъ', "J"},
                {'Ы', "I"},
                {'Ь', "J"},
                {'Э', "E"},
                {'Ю', "Yu"},
                {'Я', "Ya"}
            };
        internal static string ConvertToLatin(string source)
        {
            var result = new StringBuilder();
            foreach (var letter in source)
            {
                if (ConvertedLetters.ContainsKey(letter))
                    result.Append(ConvertedLetters[letter]);
                else
                    result.Append(letter);
            }
            return result.ToString();
        }

    }

    /// <summary>
    /// Проверка и получение телефонных номеров РФ
    /// </summary>
    public static class RusPhoneNumbers
    {
        public static string GetPhoneNumber(string text, string prefix = "")
        {
            List<char> cha = text.Where(x => IsNum(x)).ToList();
            if (cha.Count < 10) return null;
            if (cha.Count > 11) return null;
            if (cha.Count == 11 && (!(cha[0] == '7' || cha[0] == '8'))) return null;
            if (cha.Count == 11) cha.RemoveAt(0);
            return $"{prefix}{string.Join("", cha)}";
        }
        public static bool IsPhoneNumber(string number)
        {
            number = Regex.Replace(number, @"[\(\)-]", "");
            return
                (
                Regex.IsMatch(number, @"^((\+7)|7|8)\d{10}$")
                ||
                Regex.IsMatch(number, @"^((\+7)|7|8)\(\d\d\d\)\d\d\d\d\d\d\d$")
                ||
                Regex.IsMatch(number, @"^((\+7)|7|8)\(\d\d\d\)\d\d\d-\d\d-\d\d$")
                );
        }
        public static string GetPhoneNumbersIgnoreOneSpace(string text, string prefix = "")
            => GetPhoneNumbersIgnoreSpaces(text.Replace("  ", "\t"), prefix);
        public static string GetPhoneNumbersIgnoreSpaces(string text, string prefix = "")
            => GetPhoneNumbers(text.Replace(" ", ""), prefix);
        public static string GetPhoneNumbers(string text, string prefix = "")
        {
            text = Regex.Replace(text, @"[\(\)-]", "");
            string[] texts = Regex.Split(text, @"[\W+]");
            texts = texts.Select(x => GetPhoneNumber(x, prefix)).Where(x => x != null).ToArray();
            return string.Join(Environment.NewLine, texts);
        }
        private static bool IsNum(char ch) => ch >= '0' && ch <= '9';
    }
    /// <summary>
    /// Проверка условий
    /// </summary>
    public static class Check
    {
        public static bool NotNull<T>(this T obj) => obj != null;
        public static bool Null<T>(this T obj) => obj == null;
        public static bool StringIsNullOrEmpty(this string text) => text == null || text == "";
        public static bool IsZero(this int item) =>  item == 0;
        public static bool IsZero(this long item) => item == 0;
        public static bool IsZero(this byte item) => item == 0;
        public static bool More(this int a, int b) => a > b;
        public static bool More(this long a, long b) => a > b;
        public static bool More(this byte a, long b) => a > b;
        public static bool Less(this int a, int b) => a < b;
        public static bool Less(this long a, long b) => a < b;
        public static bool Less(this byte a, long b) => a < b;
        public static bool StringLengthMore(this string a, string b) => a.Length > b.Length;
        public static bool StringLengthLess(this string a, string b) => a.Length < b.Length;
    }
}
