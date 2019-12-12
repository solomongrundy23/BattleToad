using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BattleToad.Ext;

namespace BattleToad.Log
{
    /// <summary>
    /// Расширения для логирования
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Записать в лог строку
        /// </summary>
        /// <param name="str">строка</param>
        /// <param name="log">экземпляр класса Log</param>
        /// <param name="title">название записи</param>
        public static void Log(this string str, Log log, string title = "")
            => log.Write(str, title);
        /// <summary>
        /// Записать данные класса в лог(класс Log)
        /// </summary>
        /// <typeparam name="T">тип</typeparam>
        /// <param name="obj">объект</param>
        /// <param name="log">экземпляр класса Log</param>
        /// <param name="title">название записи</param>
        /// <param name="private_data">отображать приватные данные</param>
        /// <param name="data_type">отображать тип данных</param>
        public static void LogClass<T>(this T obj, Log log, string title = "", bool private_data = false, bool data_type = false)
        {
            log.WriteClass(obj, title, private_data, data_type);
        }
    }
    /// <summary>
    /// Класс для логирования
    /// </summary>
    public class Log : IDisposable
    {
        public bool Enabled = true;
        private static Log instance;
        public static Log GetInstance
        {
            get
            {
                if (instance.Null()) instance = new Log();
                return instance;
            }
        }
        private struct Record
        {
            public Record(string time, string title, string data)
            {
                Time = time;
                Title = title;
                Data = data;
            }
            public string Time;
            public string Title;
            public string Data;
            public string String
            {
                get 
                {
                    var result = new StringBuilder();
                    result.Append(Time);
                    if (Title != "") result.Append(Environment.NewLine + Title);
                    result.AppendLine(":");
                    string[] data_recs = Data.GetLines();
                    result.AppendLine("--BEGIN--");
                    result.Append(Addons.Decor(Data, 128, "         "));
                    result.AppendLine("---END---");
                    result.AppendLine();
                    return result.ToString();
                }
            }
        }
        /// <summary>
        /// Записать событие в лог
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void EventToLog(object sender, EventArgs args)
            => Write($"Event:\r\nSender:{sender.PrintClassValues().ToText()}\r\nArgs:\r\n{args.PrintClassValues().ToText()}");
        public bool Disposed = false;
        /// <summary>
        /// Убить экземпляр класса, а что он тебе сделал?
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected async void Dispose(bool disposing)
        {
            instance = null;
            if (WritterThread.IsAlive) WritterThread.Abort();
            {
                await Task.Run(() =>
                {
                    while (LogList.Count() > 0)
                    {
                        if (LogList.TryDequeue(out Record log_string))
                        {
                            try
                            {
                                File.AppendAllText(FileName, log_string.String);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"Ошибка логирования - {ex.Message}");
                            }
                        }
                    }
                });
            }
            if (!Disposed)
            {
                if (disposing)
                {
                    WritterThread.Abort();
                }
                Disposed = true;
            }
        }
        ~Log()
        {
            Dispose(false);
        }
        private readonly string FileName;
        private readonly Thread WritterThread;
        private readonly ConcurrentQueue<Record> LogList = new ConcurrentQueue<Record>();
        /// <summary>
        /// Создать логирование
        /// </summary>
        /// <param name="LogFile">путь к файлу лога</param>
        public Log(string LogFile = "log.txt")
        {
            FileName = LogFile;
            WritterThread = new Thread(Writter)
            {
                IsBackground = true
            };
            WritterThread.Start();
        }
        private void WriteLog(Record log) => LogList.Enqueue(log);
        private void Writter()
        {
            while (true)
            {
                if (LogList.TryDequeue(out Record log_string))
                {
                    try
                    {
                        File.AppendAllText(FileName, log_string.String);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Ошибка логирования - {ex.Message}");
                    }
                }
            }
        }
        /// <summary>
        /// Записать строку в лог
        /// </summary>
        /// <param name="log">данные для лога</param>
        ///<param name="title">название записи для лога</param>
        public void Write(string log, string title = "")
        {
            if (Enabled)
            WriteLog(new Record() 
            { 
                Time = Addons.GetNow(), 
                Title = title, 
                Data = log}
            );
        }
        /// <summary>
        /// Записать массив строк в лог
        /// </summary>
        /// <param name="log">массив данных для лога</param>
        ///<param name="title">название записи для лога</param>
        public void Write(string[] log, string title = "")
        {
            Write(log.ToText(), title);
        }
        /// <summary>
        /// Записать список строк в лог
        /// </summary>
        /// <param name="log">список данных для лога</param>
        ///<param name="title">название записи для лога</param>
        public void Write(List<string> log, string title = "")
        {
            Write(log.ToText());
        }
        /// <summary>
        /// Записать значение класса
        /// </summary>
        /// <typeparam name="T">класс</typeparam>
        /// <param name="obj">объект</param>
        public void WriteClass<T>(T obj, string title = "", bool private_data = false, bool data_type = false)
        {
            Write(obj.PrintClassValues(private_data, data_type).ToText(), title);
        }
    }
}
