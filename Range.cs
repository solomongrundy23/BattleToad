using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleToad.Ranges
{
    /// <summary>
    /// Класс диапозона, содержащий минимум и максимум
    /// </summary>
    public class Range<T> where T : unmanaged
    {
        /// <summary>
        /// Создает экземпляр Range
        /// </summary>
        public Range()
        {
        }
        /// <summary>
        /// Создает экземпляр Range
        /// </summary>
        /// <param name="min">значение минимума</param>
        /// <param name="max">значение максимума</param>
        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Значение минимума
        /// </summary>
        public T Min { get; set; }
        /// <summary>
        /// Значение максиума
        /// </summary>
        public T Max { get; set; }

        /// <summary>
        /// Получить строку формата $"{Min} - {Max}"
        /// </summary>
        /// <returns></returns>
        public new string ToString() => ToString(" - ");
        /// <summary>
        /// Получить строку формата $"{Min}{splitter}{Max}, где splitter - это разделитель
        /// </summary>
        /// <param name="splitter">Разделитель</param>
        /// <returns></returns>
        public virtual string ToString(string splitter)  => Min.Equals(Max) ? $"{Min}" : $"{Min}{splitter}{Max}";
    }
}
