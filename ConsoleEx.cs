using System;
using BattleToad.Ext;

namespace BattleToad.ConsoleEx
{
    public static class ConsoleEx
    {
        public static void Input(string label, out string data)
        {
            Console.Write(label);
            data = Console.ReadLine();
        }
        public static void RepeatRepeat(int repeat, string text = "")
        {
            for (int i = 0; i < repeat; i++) Console.WriteLine(text);
        }
        public static void Print(string text = "")
        {
            Console.WriteLine(text);
        }
        public static void PrintColor(string text, ConsoleColor front, ConsoleColor back)
        {
            Console.ForegroundColor = front;
            Console.BackgroundColor = back;
            Console.WriteLine(text);
        }
        public static void PrintAtPoint(string text, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.WriteLine(text);
        }
        /// <summary>
        /// Текстовый прогрессбар
        /// </summary>
        public class Progress
        {
            private int max;
            private int length = 20;
            private delegate string GetProgressTextDelegate(int value);
            private GetProgressTextDelegate GetProgressText;
            /// <summary>
            /// Создать прогрессбар
            /// </summary>
            /// <param name="Max"></param>
            /// <param name="Length"></param>
            public Progress(int Max, int theme = 0, int Length = 20)
            {
                if (length < 10) throw new Exception($"Длина {length} слишком мала, должна быть больше 5");
                max = Max;
                length = Length;
                switch (theme)
                {
                    case 0: GetProgressText = Theme1; break;
                    case 1: GetProgressText = Theme2; break;
                    case 2: GetProgressText = Theme3; break;
                    case 3: GetProgressText = Theme4; break;
                    case 4: GetProgressText = Theme5; break;
                    case 5: GetProgressText = Theme6; break;
                    case 6: GetProgressText = Theme7; break;
                    default: GetProgressText = Theme1; break;
                }
            }
            public string GetProgress(int value) => GetProgressText(value);

            private string Theme1(int value)
            {
                if (value > max) value = max;
                return value == max ? "[OK]" : $"{value * 100 / max}%";
            }
            private string Theme2(int value)
            {
                if (value > max) value = max;
                string result = "";
                int _max = value * (length - 3) / max;
                return
                    value ==
                    max ? $"[OK]".AddToEndWhileLengthNotValid(' ', length)
                    :
                    $"|{(result.AddToStartWhileLengthNotValid('=', _max) + '>').AddToEndWhileLengthNotValid(' ', length - 3)}|";
            }
            private string Theme3(int value)
            {
                if (value > max) value = max;
                string result = "";
                int _max = value * (length - 3) / max;
                return
                    value ==
                    max ? $"[OK]".AddToEndWhileLengthNotValid(' ', length)
                    :
                    $"[{(result.AddToStartWhileLengthNotValid('=', _max) + '>').AddToEndWhileLengthNotValid('.', length - 3)}]";
            }
            private string Theme4(int value)
            {
                if (value > max) value = max;
                string result = "";
                int _max = value * (length - 2) / max;
                return
                    value ==
                    max ? $"[OK]".AddToEndWhileLengthNotValid(' ', length)
                    :
                    $"|{(result.AddToStartWhileLengthNotValid('#', _max)).AddToEndWhileLengthNotValid(' ', length - 3)}|";
            }
            private string Theme5(int value)
            {
                if (value > max) value = max;
                string result = "";
                int _max = value * (length - 2) / max;
                return
                    value ==
                    max ? $"[OK]".AddToEndWhileLengthNotValid(' ', length)
                    :
                    $"[{(result.AddToStartWhileLengthNotValid('#', _max)).AddToEndWhileLengthNotValid('.', length - 3)}]";
            }
            private string Theme6(int value)
            {
                if (value > max) value = max;
                return value == max ? "[OK]".AddToEndWhileLengthNotValid(' ', max.ToString().Length * 2 + 1) : $"{value}/{max}";
            }
            private string Theme7(int value)
            {
                if (value > max) value = max;
                string result = "";
                int _max = value * (length - 3) / max;
                return
                    value ==
                    max ? $"[OK]".AddToEndWhileLengthNotValid(' ', length)
                    :
                    $"[{(result.AddToStartWhileLengthNotValid('=', _max) + '|').AddToEndWhileLengthNotValid('-', length - 3)}]";
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
}
