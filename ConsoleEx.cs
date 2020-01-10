using BattleToad.Ext;
using System;
using System.Collections.Generic;

namespace BattleToad.ConsoleAddons
{
    public static class ConsoleEx
    {
        /// <summary>
        /// Вывести в консоль значение переменной
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void WriteInConsole<T>(this T obj) => Console.Write(obj);
        /// <summary>
        /// Вывести в консоль значение переменной с переходом на новую строку
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void WriteLineInConsole<T>(this T obj) => Console.WriteLine(obj);
        /// <summary>
        /// Вывести текст и считать переменную из консоли
        /// </summary>
        /// <param name="label">текст</param>
        /// <param name="data">переменная</param>
        public static void Input(string label, out string data)
        {
            Console.Write(label);
            data = Console.ReadLine();
        }
        /// <summary>
        /// Вывести текст и считать переменную из консоли
        /// </summary>
        /// <param name="label">текст</param>
        /// <param name="data">переменная</param>
        public static void Input(string label, out int data, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(label);
                if (int.TryParse(Console.ReadLine(), out data))
                {
                    if (min < data && max > data)
                        return;
                    else
                        Console.WriteLine("Число вне диапозона");
                }
                else
                    Console.WriteLine("Неверен формат данных");
            }
        }
        /// <summary>
        /// Вывести текст в цикле
        /// </summary>
        /// <param name="repeat">количество повторов</param>
        /// <param name="text">выводимый текст</param>
        public static void RepeatPrint(int repeat, string text = "")
        {
            for (int i = 0; i < repeat; i++) Console.WriteLine(text);
        }
        /// <summary>
        /// Вывести цвет определенным цветом
        /// </summary>
        /// <param name="text"></param>
        /// <param name="front"></param>
        /// <param name="back"></param>
        public static void PrintColor(string text, ConsoleColor? front = null, ConsoleColor? back = null)
        {
            ConsoleColor oldFore = Console.ForegroundColor;
            ConsoleColor oldBack = Console.BackgroundColor;
            if (front.HasValue) Console.ForegroundColor = front.Value;
            if (back.HasValue) Console.BackgroundColor = back.Value;
            Console.WriteLine(text);
            Console.ForegroundColor = oldFore;
            Console.BackgroundColor = oldBack;
        }
        /// <summary>
        /// Упрощение Console.WriteLine
        /// </summary>
        /// <param name="text">текст</param>
        public static void Print(string text = "") => Console.WriteLine(text);
        /// <summary>
        /// Упрощение Console.WriteLine
        /// </summary>
        /// <param name="text">текст</param>
        public static void Print<T>(T text) => Console.WriteLine(text);
        /// <summary>
        /// Упрощение Console.WriteLine
        /// </summary>
        /// <param name="text">текст</param>
        public static void Print(string[] text) => 
            Console.WriteLine(string.Join(Environment.NewLine, text));
        /// <summary>
        /// Упрощение Console.WriteLine
        /// </summary>
        /// <param name="text">текст</param>
        public static void Print(List<string> text) =>
            Console.WriteLine(string.Join(Environment.NewLine, text));
        /// <summary>
        /// PrintColor c зелёным цветом фона
        /// </summary>
        /// <param name="text">текст</param>
        public static void PrintMessage(string text) => PrintColor(text, ConsoleColor.White, ConsoleColor.DarkGreen);
        /// <summary>
        /// PrintColor c жёлтым цветом фона и белым текстом
        /// </summary>
        /// <param name="text">текст</param>
        public static void PrintWarning(string text) => PrintColor(text, ConsoleColor.White, ConsoleColor.DarkYellow);
        /// <summary>
        /// PrintColor c красным цветом фона и белым текстом
        /// </summary>
        /// <param name="text">текст</param>
        public static void PrintError(string text) => PrintColor(text, ConsoleColor.White, ConsoleColor.DarkRed);
        /// <summary>
        /// Console.WriteLine с указанием позиции и белым текстом
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
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
                    max ? $"[OK]".PadRight(length, ' ')
                    :
                    $"|{(result.PadLeft(_max, '=') + '>').PadRight(length - 3, ' ')}|";
            }
            private string Theme3(int value)
            {
                if (value > max) value = max;
                string result = "";
                int _max = value * (length - 3) / max;
                return
                    value ==
                    max ? $"[OK]".PadRight(length, ' ')
                    :
                    $"[{(result.PadLeft(_max, '=') + '>').PadRight(length - 3, '.')}]";
            }
            private string Theme4(int value)
            {
                if (value > max) value = max;
                string result = "";
                int _max = value * (length - 2) / max;
                return
                    value ==
                    max ? $"[OK]".PadRight(length, ' ')
                    :
                    $"|{(result.PadLeft(_max, '#')).PadRight(length - 3, ' ')}|";
            }
            private string Theme5(int value)
            {
                if (value > max) value = max;
                string result = "";
                int _max = value * (length - 2) / max;
                return
                    value ==
                    max ? $"[OK]".PadRight(length, ' ')
                    :
                    $"[{(result.PadLeft(_max, '#')).PadRight(length - 3, '.')}]";
            }
            private string Theme6(int value)
            {
                if (value > max) value = max;
                return value == max ? 
                    "[OK]".PadRight(max.ToString().Length * 2 + 1, ' ')
                    :
                    $"{value}/{max}";
            }
            private string Theme7(int value)
            {
                if (value > max) value = max;
                string result = "";
                int _max = value * (length - 3) / max;
                return
                    value ==
                    max ? $"[OK]".PadRight(length, ' ')
                    :
                    $"[{(result.PadLeft(_max, '=') + '|').PadRight(length - 3, '-')}]";
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
