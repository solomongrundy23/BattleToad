using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleToad.List
{
    public static class Extensions
    {
        /// <summary>
        /// Перевести список в строку
        /// </summary>
        /// <typeparam name="T">Тип данных в списке</typeparam>
        /// <param name="list">Список</param>
        /// <param name="splitter">Разделитель</param>
        /// <returns>Строка с данными</returns>
        public static string ToString<T>(this List<T> list, string splitter = "\n") => string.Join(splitter, list.Select(x => x.ToString()));
        /// <summary>
        /// Перевести список в строку
        /// </summary>
        /// <param name="list">Список</param>
        /// <param name="splitter">Разделитель</param>
        /// <returns>Строка с данными</returns>
        public static string ToString(this List<string> list, string splitter = "\n") => string.Join(splitter, list);
        /// <summary>
        /// Преобразовать в очередь(Queue)
        /// </summary>
        /// <returns></returns>
        public static Queue<T> ToQueue<T>(this List<T> list)
        {
            var result = new Queue<T>();
            foreach (T obj in list)
            {
                result.Enqueue(obj);
            }
            return result;
        }
        /// <summary>
        /// Преобразовать в кучу(Stack)
        /// </summary>
        /// <returns></returns>
        public static Stack<T> ToStack<T>(this List<T> list)
        {
            var result = new Stack<T>();
            foreach (T obj in list)
            {
                result.Push(obj);
            }
            return result;
        }
        /// <summary>
        /// Оставить только уникальные значения
        /// </summary>
        public static void JustUnique<T>(this List<T> list)
        {
            IEnumerable<T> temp = list.Distinct();
            list.Clear();
            list.AddRange(temp);
        }
        /// <summary>
        /// Перемешать строки в случайном порядке
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            var temp = new List<T>();
            temp.AddRange(list);
            list.Clear();
            var rnd = new Random();
            while (temp.Count > 0)
            {
                int r = rnd.Next(temp.Count);
                list.Add(temp[r]);
                temp.RemoveAt(r);
            }
        }
        /// <summary>
        /// Получить и удалить строку по индексу
        /// </summary>
        /// <param name="index">индекс строки</param>
        /// <returns></returns>
        public static T Extract<T>(this List<T> list, int index)
        {
            T result = list[index];
            list.RemoveAt(index);
            return result;
        }
        /// <summary>
        /// Получить и удалить первую строку
        /// </summary>
        /// <returns></returns>
        public static T ExtractFirst<T>(this List<T> list) => list.Extract(0);
        /// <summary>
        /// Получить и удалить последнюю строку
        /// </summary>
        /// <returns></returns>
        public static T ExtractLast<T>(this List<T> list) => list.Extract(list.Count - 1);
        /// <summary>
        /// Получить случайную строку
        /// </summary>
        /// <returns></returns>
        public static T Random<T>(this List<T> list)
        {
            Random rnd = new Random();
            int x = rnd.Next(list.Count());
            return list[x];
        }
        /// <summary>
        /// Получить случайный элемент и удалить
        /// </summary>
        /// <returns></returns>
        public static T ExtractRandom<T>(this List<T> list)
        {
                Random rnd = new Random();
                return list.Extract(rnd.Next(list.Count()));
        }
    }
}
