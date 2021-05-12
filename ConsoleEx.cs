using System;
using System.Collections.Generic;

namespace BattleToad.ConsoleAddons
{
    public static class ConsoleEx
    {
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
        public static void Input(string label, out int data, int min = int.MinValue, int max = int.MaxValue, bool messages = true)
        {
            while (true)
            {
                Console.Write(label);
                if (int.TryParse(Console.ReadLine(), out data))
                {
                    if (min < data && max > data)
                        return;
                    else
                        if (messages) Console.WriteLine("Число вне диапозона");
                }
                else
                    if (messages) Console.WriteLine("Неверен формат данных");
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
        /// Console.WriteLine с указанием позиции
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="x">x</param>
        public static void PrintAtPoint(string text, int x)
        {
            PrintAtPoint(text, x, Console.CursorTop);
        }
        /// <summary>
        /// Console.WriteLine с указанием позиции
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        public static void PrintAtPoint(string text, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.WriteLine(text);
        }
    }
}
