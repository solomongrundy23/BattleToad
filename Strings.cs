using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BattleToad.Ext;

namespace BattleToad.Strings
{
    public static class StringsExt
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
        /// <summary>
        /// Перевести в Strings
        /// </summary>
        /// <param name="YourIEnumerable"></param>
        /// <returns></returns>
        public static Strings ToStrings(this IEnumerable<string> YourIEnumerable)
            => YourIEnumerable.ToArray().ToStrings();
    }
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
        /// Создать экземпляр Strings
        /// </summary>
        /// <param name="FromArray">Добавить массив строк при создании</param>
        public Strings(string[] FromArray)
        {
            this.AddRange(FromArray);
        }
        /// <summary>
        /// Создать экземпляр Strings, загрузив из файла
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        public Strings(string FileName, bool Empty_Strings)
            => FromFile(FileName, Empty_Strings);
        /// <summary>
        /// Получить список массивов строк разделив строки символом-разделителем
        /// </summary>
        /// <param name="splitter">разделитель</param>
        /// <returns>список массивов разделенных строк</returns>
        public List<string[]> SplitLines(char splitter = '\t')
            => this.Select(x => x.Split(splitter)).ToList();
        /// <summary>
        /// Получить список массивов строк разделив строки массивом строк-разделителей
        /// </summary>
        /// <param name="splitters">массив строк разделителей</param>
        /// <returns>список массивов разделенных строк</returns>
        public List<string[]> SplitLines(string[] splitters, StringSplitOptions options)
            => this.Select(x => x.Split(splitters, options)).ToList();
        /// <summary>
        /// Удалить пустые строки
        /// </summary>
        public void RemoveEmpty()
        {
            this.Remove("");
        }
        /// <summary>
        /// Сортировка Strings без учетка длины
        /// </summary>
        /// <param name="Desc">В обратном порядке</param>
        public void Sort(bool Desc)
        {
            if (Desc)
            {
                Sort();
                Reverse();
            }
            else
            {
                Sort();
            }
        }
        /// <summary>
        /// Сортировка Strings с учетом длины 
        /// </summary>
        /// <param name="Desc">В обратном порядке</param>
        public void SortByStringRules(bool Desc = false)
        {
            Comparer<string> comparer;
            if (Desc)
                comparer = new StringComparerDesc();
            else
                comparer = new StringComparer();
            Sort(comparer);
        }
        private class StringComparer : Comparer<string>
        {
            public override int Compare(string x, string y)
            {
                if (x == y) return 0;
                if (x.Length == y.Length)
                {
                    if (x == "") return 0;
                    int i = x.Length;
                    do
                    {
                        i--;
                        if (x[i] > y[i]) return 1;
                        else
                            if (x[i] < y[i]) return -1;
                    }
                    while (i > 0);
                    return 0;
                }
                else
                {
                    if (x.Length > y.Length)
                        return 1;
                    else
                        return -1;
                }
            }
        }
        private class StringComparerDesc : Comparer<string>
        {
            public override int Compare(string x, string y)
            {
                if (x == y) return 0;
                if (x.Length == y.Length)
                {
                    if (x == "") return 0;
                    int i = x.Length;
                    do
                    {
                        i--;
                        if (x[i] > y[i]) return -1;
                        else
                            if (x[i] < y[i]) return 1;
                    }
                    while (i > 0);
                    return 0;
                }
                else
                {
                    if (x.Length > y.Length)
                        return -1;
                    else
                        return 1;
                }
            }
        }
        public override string ToString() => this.ToText();
        /// <summary>
        /// Преобразовать в очередь(Queue)
        /// </summary>
        /// <returns></returns>
        public Queue<string> ToQueue()
        {
            var result = new Queue<string>();
            foreach (string str in this)
            {
                result.Enqueue(str);
            }
            return result;
        }
        /// <summary>
        /// Преобразовать в кучу(Stack)
        /// </summary>
        /// <returns></returns>
        public Stack<string> ToStack()
        {
            var result = new Stack<string>();
            foreach (string str in this)
            {
                result.Push(str);
            }
            return result;
        }
        /// <summary>
        /// Объединить в один String
        /// </summary>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public string Join(string splitter) => string.Join(splitter, this);
        /// <summary>
        /// Оставить только уникальные значения
        /// </summary>
        public void JustUnique()
        {
            IEnumerable<string> temp = this.Distinct();
            this.Clear();
            this.AddRange(temp);
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
        /// Получить строки удовлетворяющих регулярному выражению
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public string[] GetLinesFromRegex(string matcher)
            => this.Where(x => x.IsRegexMatch(matcher)).ToArray();
        /// <summary>
        /// Получить первую строку удовлетворяющих регулярному выражению
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public string GetLineFromRegex(string matcher)
            => this.Where(x => x.IsRegexMatch(matcher)).FirstOrDefault();
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
        /// <summary>
        /// Перемешать строки в случайном порядке
        /// </summary>
        public void Shuffle()
        {
            var temp = new List<string>();
            temp.AddRange(this);
            this.Clear();
            var rnd = new Random();
            while (temp.Count > 0)
            {
                int r = rnd.Next(temp.Count);
                this.Add(temp[r]);
                temp.RemoveAt(r);
            }
        }
        /// <summary>
        /// Получить и удалить строку по индексу
        /// </summary>
        /// <param name="index">индекс строки</param>
        /// <returns></returns>
        public string Extract(int index)
        {
            try
            {
                string result = this[index];
                this.RemoveAt(index);
                return result;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Получить и удалить первую строку
        /// </summary>
        /// <returns></returns>
        public string ExtractFirst() => Extract(0);
        /// <summary>
        /// Получить и удалить последнюю строку
        /// </summary>
        /// <returns></returns>
        public string ExtractLast() => Extract(this.Count - 1);
        /// <summary>
        /// Получить случайную строку
        /// </summary>
        /// <returns></returns>
        public string RandomString()
        {
            Random rnd = new Random();
            int x = rnd.Next(this.Count());
            return this[x];
        }
        /// <summary>
        /// Получить случайную строку и удалить
        /// </summary>
        /// <returns></returns>
        public string ExtractRandom()
        {
            int? x = null;
            try
            {
                Random rnd = new Random();
                x = rnd.Next(this.Count());
                return this[x.Value];
            }
            finally
            {
                if (x != null) this.RemoveAt(x.Value);
            }
        }

        public static Strings operator +(Strings a, IEnumerable<string> b)
        {
            Strings result = new Strings();
            result.AddRange(a);
            result.AddRange(b);
            return result;
        }
        public static Strings operator -(Strings a, IEnumerable<string> b)
        {
            Strings result = new Strings();
            result = a.Where(x => !b.Contains(x)).ToArray().ToStrings();
            return result;
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
            var result = a.Where(x => !b.Contains(x)).ToArray().ToStrings();
            return result;
        }
        public static Strings operator -(Strings a, string[] b)
        {
            var result = a.Where(x => !b.Contains(x)).ToArray().ToStrings();
            return result;
        }
        public static Strings operator -(Strings a, List<string> b)
        {
            var result = a.Where(x => !b.Contains(x)).ToArray().ToStrings();
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
}
