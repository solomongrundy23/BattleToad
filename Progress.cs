using BattleToad.ConsoleAddons;
using BattleToad.Ext;
using System;

namespace BattleToad.Progress
{
    /// <summary>
    /// Текстовый прогрессбар
    /// </summary>
    public class Progress
    {
        private int max;
        private int Length => length ?? Console.WindowWidth;
        private int? length;
        private delegate string GetProgressTextDelegate(int value);
        private GetProgressTextDelegate GetProgressText;
        /// <summary>
        /// Создать прогрессбар
        /// </summary>
        /// <param name="Max"></param>
        /// <param name="Length"></param>
        public Progress(int Max, int theme = 0, int? Length = null)
        {
            if (Length < 10) throw new Exception($"Длина {Length} слишком мала, должна быть больше 5");
            max = Max;
            length = Length;
            GetProgressText = theme switch
            {
                0 => Theme1,
                1 => Theme2,
                2 => Theme3,
                3 => Theme4,
                4 => Theme5,
                5 => Theme6,
                6 => Theme7,
                7 => Theme8,
                _ => Theme1,
            };
        }
        public string GetProgress(int value) => GetProgressText(value);

        private string Theme1(int value)
        {
            if (value > max) value = max;
            return value == max ? "[OK]" : $"{value * 100 / max}%".Cut(Length).PadRight(Length, ' ');
        }
        private string Theme2(int value)
        {
            if (value > max) value = max;
            string result = "";
            int _max = value * (Length - 2) / max;
            return
                value ==
                max ? $"[OK]".PadRight(Length, ' ')
                :
                $"|{(result.PadLeft(_max, '=') + '>').PadRight(Length - 2, ' ')}|";
        }
        private string Theme3(int value)
        {
            if (value > max) value = max;
            string result = "";
            int _max = value * (Length - 2) / max;
            return
                value ==
                max ? $"[OK]".PadRight(Length, ' ')
                :
                $"[{(result.PadLeft(_max, '=') + '>').PadRight(Length - 2, '.')}]";
        }
        private string Theme4(int value)
        {
            if (value > max) value = max;
            string result = "";
            int _max = value * (Length - 2) / max;
            return
                value ==
                max ? $"[OK]".PadRight(Length, ' ')
                :
                $"|{(result.PadLeft(_max, '#')).PadRight(Length - 2, ' ')}|";
        }
        private string Theme5(int value)
        {
            if (value > max) value = max;
            string result = "";
            int _max = value * (Length - 2) / max;
            return
                value ==
                max ? $"[OK]".PadRight(Length, ' ')
                :
                $"[{(result.PadLeft(_max, '#')).PadRight(Length - 2, '.')}]";
        }
        private string Theme6(int value)
        {
            if (value > max) value = max;
            return value == max ?
                "[OK]".PadRight(max.ToString().Length * 2 + 1, ' ')
                :
                $"{value}/{max}".Cut(Length).PadRight(Length, ' ');
        }
        private string Theme7(int value)
        {
            if (value > max) value = max;
            string result = "";
            int _max = value * (Length - 2) / max;
            return
                value ==
                max ? $"[OK]".PadRight(Length, ' ')
                :
                $"[{(result.PadLeft(_max, '=') + '|').PadRight(Length - 2, '-')}]";
        }
        private string Theme8(int value)
        {
            if (value > max) value = max;
            string result = "";
            int _max = value * Length / max;
            result = result.PadLeft(_max, '|');
            return
                value ==
                max ? $"[OK]".PadRight(Length, ' ')
                :
                $"{result.PadRight(Length, '.')}";
        }

        /// <summary>
        /// Вывести прогрессбар в косноль
        /// </summary>
        /// <param name="value">значение</param>
        /// <param name="x">позиция x</param>
        /// <param name="y">позиция y</param>
        public void GetProgressToConsole(int value, int x, int y)
            => ConsoleEx.PrintAtPoint(GetProgress(value), x, y);
    }
}
