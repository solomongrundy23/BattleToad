using BattleToad.Ext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BattleToad.CSV
{
    public class CSV : IDisposable
    {
        /// <summary>
        /// Включать ли сборку мусора
        /// </summary>
        public bool AutoGarbageRemove;
        private void RemoveGarbage()
        {
            if (AutoGarbageRemove) GC.Collect(GC.MaxGeneration);
        }
        /// <summary>
        /// Создать экземплер
        /// </summary>
        /// <param name="split_char">Разделитель</param>
        /// <param name="autoGarbageRemove">Включать ли сборку мусора</param>
        /// <param name="quotes">Одинарные или двойные кавычки, null - нет</param>
        /// <param name="null_Text">Замена пустых значений</param>
        public CSV(char split_char = '\t', bool autoGarbageRemove = false, 
                   char? quotes = null, string null_Text = null)
        {
            AutoGarbageRemove = autoGarbageRemove;
            Quotes = quotes;
            SplitChar = split_char;
            Null_Text = null_Text;
        }
        /// <summary>
        /// Знак кавычек, если нет, то null
        /// </summary>
        public char? Quotes;
        /// <summary>
        /// Разделитель колонок
        /// </summary>
        public char SplitChar;
        public string Null_Text;
        /// <summary>
        /// Заголовок CSV
        /// </summary>
        public string[] Header;
        /// <summary>
        /// Загрузить CSV-файл
        /// </summary>
        /// <param name="file_name">имя файла</param>
        /// <param name="first_line_is_header">первая строка, это заголовок</param>
        public async Task LoadAsync(string file_name, bool first_line_is_header = false)
            => await Task.Run(() => Load(file_name, first_line_is_header));
        /// <summary>
        /// Сохранить данные CSV в файл
        /// </summary>
        /// <param name="file_name">имя файла</param>
        /// <param name="write_header">записывать ли заголовок, если он не Null</param>
        public async Task SaveAsync(string file_name, bool write_header = true)
            => await Task.Run(() => Save(file_name, write_header));
        /// <summary>
        /// Загрузить CSV-файл
        /// </summary>
        /// <param name="file_name">имя файла</param>
        /// <param name="first_line_is_header">первая строка, это заголовок</param>
        public void Load(string file_name, bool first_line_is_header = false)
        {
            try
            {
                List<string[]> result = new List<string[]>();
                string[] file_data = File.ReadAllText(file_name).GetLines(false);
                foreach (string file_string in file_data)
                {
                    string[] records = ParseLine(file_string);
                    if (records != null) result.Add(records);
                }
                if (first_line_is_header)
                {
                    if (result.Count == 0)
                    {
                        throw new Exception($"Не могу найти заголовок");
                    }
                    else
                    {
                        Header = result[0];
                        result.RemoveAt(0);
                    }
                }
                Data = result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки CSV-файла. {ex.Message}");
            }
            finally
            {
                RemoveGarbage();
            }
        }
        /// <summary>
        /// Сохранить данные CSV в файл
        /// </summary>
        /// <param name="file_name">имя файла</param>
        /// <param name="write_header">записывать ли заголовок, если он не Null</param>
        public void Save(string file_name, bool write_header = true)
        {
            try
            {
                List<string> result = new List<string>();
                if (Header != null && write_header) result.Add(UnTrimmerRecord(Header));
                foreach (string[] records in Data)
                {
                    result.Add(UnTrimmerRecord(records));
                }
                File.WriteAllLines(file_name, result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка  CSV-файла. {ex.Message}");
            }
            finally
            {
                RemoveGarbage();
            }
        }
        /// <summary>
        /// Данные CSV, хранятся как список массивов строк
        /// </summary>
        public List<string[]> Data = new List<string[]>();
        private string[] ParseLine(string line)
        {
            return line.Split(SplitChar).Select(x => Trimmer(x)).ToArray();
        }
        private string Trimmer(string record)
        {
            if (record == Null_Text) return null;
            return Quotes == null ? record : record.Trim(Quotes.Value);
        }
        private string UnTrimmer(string record)
        {
            if (record == null) return Null_Text;
            return Quotes == null ? record : $"{Quotes}{record}{Quotes}";
        }
        private string UnTrimmerRecord(string[] records) 
            => string.Join($"{SplitChar}", records.Select(x => UnTrimmer(x)).ToArray());

        /// <summary>
        /// Запустить сборку мусора GC.Collect()
        /// </summary>
        public void GarbageCollect()
        {
            GC.Collect();
        }
        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public void Dispose()
        {
            Data = null;
            RemoveGarbage();
            GC.WaitForPendingFinalizers();
        }
    }
}
