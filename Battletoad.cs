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

namespace BattleToad.Ext
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
        public static Strings operator +(Strings a, Strings b)
        {
            Strings result = new Strings();
            result.AddRange(a);
            result.AddRange(b);
            return result;
        }
        public static Strings operator +(Strings a, string[] b)
        {
            Strings result = new Strings();
            result.AddRange(a);
            result.AddRange(b);
            return result;
        }
        public static Strings operator +(Strings a, List<string> b)
        {
            Strings result = new Strings();
            result.AddRange(a);
            result.AddRange(b);
            return result;
        }
        public static Strings operator +(Strings a, string b)
        {
            Strings result = new Strings();
            result.AddRange(a);
            result.Add(b);
            return result;
        }
        public static Strings operator -(Strings a, Strings b)
        {
            Strings result = new Strings();
            result = a.Where(x => !b.Contains(x)).ToArray().ToStrings();
            return result;
        }
        public static Strings operator -(Strings a, string[] b)
        {
            Strings result = new Strings();
            result = a.Where(x => !b.Contains(x)).ToArray().ToStrings();
            return result;
        }
        public static Strings operator -(Strings a, List<string> b)
        {
            Strings result = new Strings();
            result = a.Where(x => !b.Contains(x)).ToArray().ToStrings();
            return result;
        }
        public static Strings operator -(Strings a, string b)
        {
            Strings result = new Strings();
            result.AddRange(a);
            result.Remove(b);
            return result;
        }
    }
    /// <summary>
    /// Цивилизованная многопоточная очередь
    /// </summary>
    public class CivilizedQueue : ConcurrentQueue<string> { }

    public static class Extensions
    {
        //Strings
        /// <summary>
        /// Перевести в Strings
        /// </summary>
        /// <param name="YourList"></param>
        /// <returns></returns>
        public static Strings ToStrings(this string[] YourList)
        {
            Strings result = new Strings();
            result.AddRange(YourList);
            return result;
        }
        /// <summary>
        /// Перевести в Strings
        /// </summary>
        /// <param name="YourList"></param>
        /// <returns></returns>
        public static Strings ToStrings(this List<string> YourList)
            => YourList.ToArray().ToStrings();
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
        /// <summary>
        /// Записать данные класса в лог(класс Log)
        /// </summary>
        /// <typeparam name="T">тип</typeparam>
        /// <param name="obj">объект</param>
        /// <param name="log">экземпляр класса Log</param>
        /// <param name="title">название записи</param>
        /// <param name="private_data">отображать приватные данные</param>
        /// <param name="data_type">отображать тип данных</param>
        public static void LogClass<T>(this T obj, Log log, string title = "", bool private_data = false, bool data_type = false)
        {
            log.WriteClass(obj, title, private_data, data_type);
        }
    }

    /// <summary>
    /// Класс для логирования
    /// </summary>
    public class Log : IDisposable
    {
        private bool disposed = false;
        /// <summary>
        /// Убить экземпляр класса, а что он тебе сделал?
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
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
            if (!disposed)
            {
                if (disposing)
                {
                    WritterThread.Abort();
                }
                disposed = true;
            }
        }
        ~Log()
        {
            Dispose(false);
        }
        private readonly string FileName;
        private Thread WritterThread;
        private readonly ConcurrentQueue<string> LogList = new ConcurrentQueue<string>();
        /// <summary>
        /// Создать логирование
        /// </summary>
        /// <param name="LogFile">путь к файлу лога</param>
        public Log(string LogFile = "log.txt")
        {
            FileName = LogFile;
            WritterThread = new Thread(Writter)
            {
                IsBackground = true
            };
            WritterThread.Start();
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
        private string LogString(string log, string title = "")
            => $"{Addons.GetNow()}: {title}{Environment.NewLine}{log}{Environment.NewLine}{Environment.NewLine}";
        /// <summary>
        /// Записать строку в лог
        /// </summary>
        /// <param name="log">данные для лога</param>
        ///<param name="title">название записи для лога</param>
        public void Write(string log, string title = "")
        {
            WriteLog(LogString(log));
        }
        /// <summary>
        /// Записать массив строк в лог
        /// </summary>
        /// <param name="log">массив данных для лога</param>
        ///<param name="title">название записи для лога</param>
        public void Write(string[] log, string title = "")
        {
            Write(log.ToText(), title);
        }
        /// <summary>
        /// Записать список строк в лог
        /// </summary>
        /// <param name="log">список данных для лога</param>
        ///<param name="title">название записи для лога</param>
        public void Write(List<string> log, string title = "")
        {
            Write(log.ToText());
        }
        /// <summary>
        /// Записать значение класса
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="obj">объект</param>
        public void WriteClass<T>(T obj, string title = "", bool private_data = false, bool data_type = false)
        {
            Write(obj.PrintClassValues(private_data, data_type).ToText(), title);
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
        private static HashAlgorithm GetAlgorithm(Type type)
        {
            switch (type)
            {
                case Type.MD5: return new MD5CryptoServiceProvider();
                case Type.SHA256: return new SHA256CryptoServiceProvider();
                case Type.SHA512: return new SHA512CryptoServiceProvider();
                case Type.SHA1: return new SHA1CryptoServiceProvider();
                case Type.SHA384: return new SHA384CryptoServiceProvider();
                default: return new MD5CryptoServiceProvider();
            }
        }
        public static string GetHash(byte[] bytes, Type type = Type.MD5)
        {
            using (HashAlgorithm algorithm = GetAlgorithm(type))
            {
                bytes = algorithm.ComputeHash(bytes);
                string result = BitConverter.ToString(bytes).Replace("-", string.Empty);
                algorithm.Dispose();
                return result;
            }
        }
        /// <summary>
        /// Получить ХЭШ из строки
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="type">алгоритм</param>
        /// <returns></returns>
        public static string GetHashFromString(string text, Type type = Type.MD5)
            => GetHash(Encoding.Unicode.GetBytes(text), type);
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
        /// <param name="type">тип ХЭШа</param>
        /// <returns></returns>
        public static string ComputeFileChecksum(string path, Type type = Type.MD5)
        {
            using (FileStream fs = File.OpenRead(path))
            using (HashAlgorithm algorithm = GetAlgorithm(type))
            {
                byte[] checkSum = algorithm.ComputeHash(fs);
                string result = BitConverter.ToString(checkSum).Replace("-", string.Empty);
                return result;
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
            List<string> result = new List<string>();
            BindingFlags flags = ShowPrivate ?
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                :
                BindingFlags.Public | BindingFlags.Instance;
            try
            {
                PropertyInfo[] pi = obj.GetType().GetProperties(flags);
                string[] properties = ShowTypes ?
                   pi.Select(x => $"{x.PropertyType.Name}: {x.Name}=\"{x.GetValue(obj)}\"").ToArray()
                   :
                   pi.Select(x => $"{x.Name}=\"{x.GetValue(obj)}\"").ToArray();
                result.AddRange(properties);
            }
            catch { }
            try
            {
                FieldInfo[] fi = obj.GetType().GetFields(flags);
                string[] fields = ShowTypes ?
                   fi.Select(x => $"{x.FieldType.Name}: {x.Name}=\"{x.GetValue(obj)}\"").ToArray()
                   :
                   fi.Select(x => $"{x.Name}=\"{x.GetValue(obj)}\"").ToArray();
                result.AddRange(fields);
            }
            catch { }
            return result.ToArray();
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
        public static int GetByteFromHexString(string hexString) => int.Parse(hexString, NumberStyles.HexNumber);
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
        /// <summary>
        /// Возвращает значение True, если переменная имеет значение, отличное от Null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>Еrue, если переменная имеет значение, отличное от Null</returns>
        public static bool NotNull<T>(this T obj) => obj != null;
        /// <summary>
        /// Возвращает значение True, если переменная имеет значение, равное Null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>true, если переменная имеет значение, равное Null</returns>
        public static bool Null<T>(this T obj) => obj == null;
        /// <summary>
        /// Возвращает True, если строка пустая
        /// </summary>
        /// <param name="text"></param>
        /// <returns>True, если строка пустая или Null</returns>
        public static bool IsEmpty(this string text) => text == "";
        /// <summary>
        /// Возвращает True, если строка не пустая
        /// </summary>
        /// <param name="text"></param>
        /// <returns>True, если строка пустая или Null</returns>
        public static bool IsNotEmpty(this string text) => text != "";
        /// <summary>
        /// Возвращает True, если переменная равна 0
        /// </summary>
        /// <param name="number"></param>
        /// <returns>True, если переменная равна 0</returns>
        public static bool IsZero(this int number) => number == 0;
        /// <summary>
        /// Возвращает True, если переменная равна 0
        /// </summary>
        /// <param name="number"></param>
        /// <returns>True, если переменная равна 0</returns>
        public static bool IsZero(this long number) => number == 0;
        /// <summary>
        /// Возвращает True, если переменная равна 0
        /// </summary>
        /// <param name="number"></param>
        /// <returns>True, если переменная равна 0</returns>
        public static bool IsZero(this byte number) => number == 0;
        /// <summary>
        /// Больше
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool More(this int a, int b) => a > b;
        /// <summary>
        /// Больше
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool More(this long a, long b) => a > b;
        /// <summary>
        /// Больше
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool More(this byte a, long b) => a > b;
        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Less(this int a, int b) => a < b;
        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Less(this long a, long b) => a < b;
        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Less(this byte a, long b) => a < b;
        /// <summary>
        /// Длиннее
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool StringLengthMore(this string a, int b) => a.Length > b;
        /// <summary>
        /// Короче
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool StringLengthLess(this string a, int b) => a.Length < b;
        /// <summary>
        /// Длиннее, чем строка
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool StringLengthMore(this string a, string b) => a.Length > b.Length;
        /// <summary>
        /// Короче, чем строка
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool StringLengthLess(this string a, string b) => a.Length < b.Length;
        /// <summary>
        /// Возвращает True, если число не четное
        /// </summary>
        /// <param name="number"></param>
        /// <returns>number % 2 == 1</returns>
        public static bool IsOdd(this int number) => number % 2 == 1;
        /// <summary>
        /// Возвращает True, если число четное
        /// </summary>
        /// <param name="number"></param>
        /// <returns>number % 2 == 0</returns>
        public static bool IsEven(this int number) => number % 2 == 0;
    }

    /// <summary>
    /// Имя файла
    /// </summary>
    public class FileName
    {
        /// <summary>
        /// Создать экземпляр имени класса
        /// </summary>
        /// <param name="file_name"></param>
        public FileName(string file_name)
        {
            Name = System.IO.Path.GetFileNameWithoutExtension(file_name);
            Extension = System.IO.Path.GetExtension(file_name);
            Path = System.IO.Path.GetDirectoryName(file_name);
        }
        /// <summary>
        /// Имя файла без расширения
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Расширение файла
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Полный путь к файлу
        /// </summary>
        public string Full => Path == "" ? $"{Name}{Extension}" : $"{Path}\\{Name}{Extension}";
        /// <summary>
        /// Имя файла с расширением
        /// </summary>
        public string NameWithExtension => $"{Name}{Extension}";
    }
    /// <summary>
    /// Перевести int в числительные строки, к примеру 1ый, 2ой, 103ий
    /// </summary>
    public static class OneToFirst
    {
        /// <summary>
        /// Перевести int в числительные строки на русском языке
        /// </summary>
        /// <param name="number">число</param>
        /// <returns></returns>
        public static string OneToFirstRus(this int number)
        {
            string text = number.ToString();
            switch (text.LastOrDefault())
            {
                case '0': return text == "0" ? $"{text}ой" : $"{text}ый";
                case '1': return $"{text}ый";
                case '2': return $"{text}ой";
                case '3': return $"{text}ий";
                case '4': return $"{text}ый";
                case '5': return $"{text}ый";
                case '6': return $"{text}ой";
                case '7': return $"{text}ой";
                case '8': return $"{text}ой";
                case '9': return $"{text}ый";
                default: return text;
            }
        }
        /// <summary>
        /// Перевести int в числительные строки на английском языке
        /// </summary>
        /// <param name="number">число</param>
        /// <returns></returns>
        public static string OneToFirstEng(this int number)
        {
            string text = number.ToString();
            if (text.Length == 2 && text.StartsWith("1")) return $"{text}th";
            switch (text.LastOrDefault())
            {
                case '0': return text == "0" ? text : $"{text}th";
                case '1': return $"{text}st";
                case '2': return $"{text}nd";
                case '3': return $"{text}rd";
                default: return $"{text}th";
            }
        }
    }
}