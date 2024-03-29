﻿using System;
using System.Collections.Generic;
using System.Linq;
using BattleToad.Extensions;
using BattleToad.Ranges;

namespace BattleToad.PhoneRange
{
    public class PhoneRanger : IDisposable
    {
        /// <summary>
        /// Определяет, будет ли производится автоматически сборка мусора
        /// </summary>
        public bool AutoGarbageCollector = true;
        /// <summary>
        /// Запуск GarbageCollector
        /// </summary>
        private void Garbage()
        {
            if (AutoGarbageCollector) GC.Collect(GC.MaxGeneration); 
        }
        /// <summary>
        /// Список диапозонов, при добавлении диапозонов вручную нужно обеспечивать праверку правильности заполнения диапозонами номера
        /// </summary>
        public List<Range<ulong>> Ranges = new List<Range<ulong>>();
        /// <summary>
        /// Получить список диапозонов из текста, содержащего телефонные номера
        /// </summary>
        /// <param name="text">текст</param>
        public void Parse(string text) => Parse(text.GetWords());
        /// <summary>
        /// Получить список диапозонов из массива строк, содержащих по одному телефонному номеру
        /// </summary>
        /// <param name="records">массив строк, содержащих по одному телефонному номеру</param>
        public void Parse(string[] records)
        {
            try
            {
                if (Ranges.Count > 0)
                {
                    Ranges.Clear();
                    Garbage();
                }
                var nums = records
                    .Select(x => RusPhoneNumbers.GetPhoneNumber(x, "7"))
                    .Where(x => x != null)
                    .Select(x => ulong.Parse(x))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray();
                if (nums.Length == 0) return;
                var range = new Range<ulong>(nums[0], nums[0]);
                for (int i = 1; i < nums.Count(); i++)
                {
                    if (range.Max + 1 == nums[i])
                    {
                        range.Max = nums[i];
                    }
                    else
                    {
                        Ranges.Add(range);
                        range = new Range<ulong>(nums[i], nums[i]);
                    }
                }
                if (Ranges.Count == 0)
                    Ranges.Add(range);
                else
                    if (Ranges.Last().Max != range.Max)
                    Ranges.Add(range);
            }
            catch
            {
                throw new Exception("Не удалось произвести парсинг данных, проверьте вводимые данные");
            }
            finally
            {
                Garbage();
            }
        }
        /// <summary>
        /// Сортировка списка диапозонов по минимумам, а затем по максимумам
        /// </summary>
        public void Sort()
        {
            Ranges = Ranges.OrderBy(x => x.Min).ThenBy(x => x.Max).ToList();
            Garbage();
        }
        /// <summary>
        /// Обединить близкие диапозоны
        /// </summary>
        public void Merge()
        {
            if (Ranges.Count() == 0) return;
            try
            {
                var Temp = Ranges.OrderBy(x => x.Min).ThenBy(x => x.Max).ToList();
                Ranges.Clear();
                var range = Temp[0];
                for (int i = 1; i < Temp.Count(); i++)
                {
                    if (Temp[i].Min > range.Min && Temp[i].Max < range.Max) continue;
                    if (
                        (range.Max + 1 == Temp[i].Min) ||
                        (Temp[i].Min > range.Min && Temp[i].Max >= range.Max)
                       )
                    {
                        range.Max = Temp[i].Max;
                    }
                    else
                    {
                        Ranges.Add(range);
                        range = Temp[i];
                    }
                }
                if (Temp.Count == 0)
                    Ranges.Add(range);
                else
                    if (Ranges.Last().Max != range.Max)
                    Ranges.Add(range);
            }
            finally
            {
                Garbage();
            }
        }
        /// <summary>
        /// Разбить диапозоны на список
        /// </summary>
        /// <param name="Merge">Объединение диапозонов</param>
        /// <returns></returns>
        public List<string> BreakingRange(bool Merge = false)
        {
            try
            {
                var result = new List<string>();
                foreach (var range in Ranges)
                {
                    if (range != null)
                        if (Merge)
                            result.AddRange(DissolutionCompress(range));
                        else
                            result.AddRange(Dissolution(range));
                }
                return result;
            }
            finally
            {
                Garbage();
            }
        }

        private string Cut(Range<ulong> range)
        {
            try
            {
                string sa = range.Min.ToString();
                string sb = range.Max.ToString();
                if (sa == sb) return sa;
                while (sa.Last() == '0' && sb.Last() == '9')
                {
                    sa = sa.Substring(0, sa.Length - 1);
                    sb = sb.Substring(0, sb.Length - 1);
                }
                if (sa == sb) return sa.PadRight(11, '.');
                return null;
            }
            catch
            {
                throw new Exception("Не верные данные");
            }
        }

        private List<string> Dissolution(Range<ulong> range)
        {
            var result = new List<string>();

            string Cutted = Cut(range);
            if (Cutted != null)
            {
                result.Add(Cutted);
            }
            else
            {
                result.AddRange(
                    ToMasks(range.Min, range.Max)
                    .Select(x => x.ToString()).ToArray());
            }
            CompressDots(ref result);
            return result;
        }

        private List<string> DissolutionCompress(Range<ulong> range)
        {
            var result = new List<string>();
            string Cutted = Cut(range);
            if (Cutted != null)
            {
                result.Add($"^{Cutted}$");
            }
            else
            {
                string[] list =
                    ToMasks(range.Min, range.Max)
                    .Select(x => x.ToString()).ToArray();
                string old = list[0].Substring(0, list[0].Length - 1);
                int A = int.Parse(list[0].Substring(list[0].Length - 1, 1));
                int B = 0;
                int C = A;
                bool recorded = false;
                for (int i = 1; i < list.Length; i++)
                {
                    if (list[i] != "")
                    {
                        B = int.Parse(list[i].Substring(list[i].Length - 1, 1));
                        if ((C + 1 != B) | (old.Length + 1 != list[i].Length))
                        {
                            string temp_s = A == C ? old + $"{A}" : old + $"[{A}-{C}]";
                            temp_s += AddDots(ref old);
                            result.Add($"^{temp_s}$");
                            A = B;
                            C = B;
                            old = list[i].Substring(0, list[i].Length - 1);
                        }
                        else
                        {
                            C = B;
                            old = list[i].Substring(0, list[i].Length - 1);
                        }
                    }
                }
                if (!recorded)
                {
                    string temp_s = A == C ? old + $"{A}" : old + $"[{A}-{C}]";
                    temp_s += AddDots(ref old);
                    result.Add("^" + temp_s + "$");
                }
                result = result.Select(x => x.Replace("[0-9]", ".")).ToList();
            }
            CompressDots(ref result);
            return result;
        }

        private string GetDots(int i)
        {
            string result = "";
            while (result.Length < i) i += '.';
            return result;
        }
        private string AddDots(ref string old) => GetDots(10 - old.Length);
        private void CompressDots(ref List<string> data)
        {
            for (int i = 10; i >= 0; i--)
                for (int j = 0; j < data.Count; i++)
                    data[j] = data[j].Replace(GetDots(i), ".{" + i.ToString() + "}");
        }

        private IEnumerable<ulong> ToMasks(ulong start, ulong end)
        {
            while (start <= end)
            {
                ulong mask = 0, pow = 0;
                for (int i = 0; ; i++)
                {
                    pow = (uint)Math.Pow(10, 7 - i);
                    mask = start / pow;
                    if (mask * pow + pow - 1 <= end && mask * pow >= start)
                    {
                        yield return mask;
                        break;
                    }
                }
                start = (mask + 1) * pow;
            }
        }

        ~PhoneRanger()
        {
           Dispose();
        }

        public void Dispose()
        {
            if (Ranges != null) Ranges = null;
            Garbage();
            GC.SuppressFinalize(this);
        }
    }
}