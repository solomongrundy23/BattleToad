using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BattleToad.WinFormsExt
{
    class WinFormsExt
    {
        /// <summary>
        /// Вызывает MessageBox ошибки
        /// </summary>
        /// <param name="text">текст сообщение</param>
        /// <param name="title">текст названия</param>
        public static void MessageError(string text, string title = "Ошибка")
            => MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        /// <summary>
        /// Вызывает MessageBox предупреждения
        /// </summary>
        /// <param name="text">текст сообщение</param>
        /// <param name="title">текст названия</param>
        public static void MessageWarning(string text, string title = "Внимание")
                    => MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        /// <summary>
        /// Вызывает MessageBox информации
        /// </summary>
        /// <param name="text">текст сообщение</param>
        /// <param name="title">текст названия</param>
        public static void MessageInfo(string text, string title = "Сообщение")
                    => MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        /// <summary>
        /// Вызывает MessageBox подтверждения
        /// </summary>
        /// <param name="text">текст сообщение</param>
        /// <param name="title">текст названия</param>
        /// <returns>Результат диалога</returns>
        public static void MessageConfirmation(string text, string title = "Подтверждение")
                    => MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        /// <summary>
        /// Вызывает MessageBox подтверждения с ошибкой
        /// </summary>
        /// <param name="text">текст сообщение</param>
        /// <param name="title">текст названия</param>
        /// <returns>Результат диалога</returns>
        public static DialogResult MessageConfirmationWarning(string text, string title = "Внимание")
            => MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        /// <summary>
        /// Получить размер отображаемого текста
        /// </summary>
        /// <param name="form">Форма, на которой расположен элемент</param>
        /// <param name="text">Текст</param>
        /// <param name="font">Шрифт текста</param>
        /// <returns>Структура с размерами текста</returns>
        public static SizeF GetTextSize(Form form, string text, Font font)
        {
            using (Graphics g = Graphics.FromHwnd(form.Handle))
            {
                return g.MeasureString(text, font);
            }
        }

        public Color GetBack(Bitmap bmp, int alpha = 100)
        {
            Bitmap temp = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, 0, 0, 1, 1);
                g.Dispose();
            }
            return temp.GetPixel(0, 0);
        }

        public static Color ColorFromAhsb(int a, float h, float s, float b)
        {
            try
            {
                if (0 > a || 255 < a)
                {
                    throw new Exception("a");
                }
                if (0f > h || 360f < h)
                {
                    throw new Exception("h");
                }
                if (0f > s || 1f < s)
                {
                    throw new Exception("s");
                }
                if (0f > b || 1f < b)
                {
                    throw new Exception("b");
                }

                if (0 == s)
                {
                    return Color.FromArgb(a, Convert.ToInt32(b * 255),
                      Convert.ToInt32(b * 255), Convert.ToInt32(b * 255));
                }

                float fMax, fMid, fMin;
                int iSextant, iMax, iMid, iMin;

                if (0.5 < b)
                {
                    fMax = b - (b * s) + s;
                    fMin = b + (b * s) - s;
                }
                else
                {
                    fMax = b + (b * s);
                    fMin = b - (b * s);
                }

                iSextant = (int)Math.Floor(h / 60f);
                if (300f <= h)
                {
                    h -= 360f;
                }
                h /= 60f;
                h -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
                if (0 == iSextant % 2)
                {
                    fMid = h * (fMax - fMin) + fMin;
                }
                else
                {
                    fMid = fMin - h * (fMax - fMin);
                }

                iMax = Convert.ToInt32(fMax * 255);
                iMid = Convert.ToInt32(fMid * 255);
                iMin = Convert.ToInt32(fMin * 255);

                switch (iSextant)
                {
                    case 1:
                        return Color.FromArgb(a, iMid, iMax, iMin);
                    case 2:
                        return Color.FromArgb(a, iMin, iMax, iMid);
                    case 3:
                        return Color.FromArgb(a, iMin, iMid, iMax);
                    case 4:
                        return Color.FromArgb(a, iMid, iMin, iMax);
                    case 5:
                        return Color.FromArgb(a, iMax, iMin, iMid);
                    default:
                        return Color.FromArgb(a, iMax, iMid, iMin);
                }
            }
            catch
            {
                return Color.Black;
            }
        }

    }

}
