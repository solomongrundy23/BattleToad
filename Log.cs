using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BattleToad.Ext;

namespace BattleToad.Log
{
    /// <summary>
    /// Абстрактный класс для логирования
    /// </summary>
    public class Logging: IDisposable
    {
        public Logging(string log_filename = "log.txt") => LogStream = new Log(log_filename);
        public Logging(Log log) => LogStream = log;
        public Log LogStream;
        public virtual void ToLog(string title = "", bool private_data = false, bool data_type = false) 
            => this.LogClass(LogStream, title, private_data, data_type);
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    LogStream.Dispose();
                }
                disposedValue = true;
            }
        }
        ~Logging()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

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
        /// <summary>
        /// Записать событие в лог
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void EventToLog(object sender, EventArgs args)
            => Write($"Event:\r\nSender:{sender.PrintClassValues().ToText()}\r\nArgs:\r\n{args.PrintClassValues().ToText()}");
        private bool disposed = false;
        /// <summary>
        /// Убить экземпляр класса, а что он тебе сделал?
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (WritterThread.IsAlive) WritterThread.Abort();
            while (LogList.Count() > 0)
            {
                if (LogList.TryDequeue(out string log_string))
                {
                    try
                    {
                        File.AppendAllText(FileName, log_string);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Ошибка логирования - {ex.Message}");
                    }
                }
            }
            if (!disposed)
            {
                if (disposing)
                {
                    WritterThread.Abort();
                }
                disposed = true;
            }
        }
        ~Log()
        {
            Dispose(false);
        }
        private readonly string FileName;
        private Thread WritterThread;
        private readonly ConcurrentQueue<string> LogList = new ConcurrentQueue<string>();
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
        private void WriteLog(string log) => LogList.Enqueue(log);
        private void Writter()
        {
            while (true)
            {
                if (LogList.TryDequeue(out string log_string))
                {
                    try
                    {
                        File.AppendAllText(FileName, log_string);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Ошибка логирования - {ex.Message}");
                    }
                }
            }
        }
        private string LogString(string log, string title = "")
            => $"{Addons.GetNow()}: {title}{Environment.NewLine}{log}{Environment.NewLine}{Environment.NewLine}";
        /// <summary>
        /// Записать строку в лог
        /// </summary>
        /// <param name="log">данные для лога</param>
        ///<param name="title">название записи для лога</param>
        public void Write(string log, string title = "")
        {
            WriteLog(LogString(log));
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
