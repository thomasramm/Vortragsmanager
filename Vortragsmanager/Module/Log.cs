using System;
using System.IO;
using Vortragsmanager.DataModels;

namespace Vortragsmanager.Module
{
    public static class Log
    {
        public static LogLevel Level { get; set; }

        public static string File { get; set; }

        public static void Start()
        {
            Level = (LogLevel)Helper.Helper.GlobalSettings.LogLevel;
            var di = new DirectoryInfo(Helper.Helper.GlobalSettings.LogFolder);
            if (!di.Exists)
                Helper.Helper.GlobalSettings.LogFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            File = Helper.Helper.GlobalSettings.LogFolder.TrimEnd('\\')
                + $"\\vortragsmanager_{DateTime.Today:yyyy-MM-dd}.log";
        }

        public static void Start(LogLevel level)
        {
            Helper.Helper.GlobalSettings.LogLevel = (int)level;
            Helper.Helper.GlobalSettings.Save();
            Start();
        }

        public static void Loging(LogLevel level, string message)
        {
            Write(level, "Unknown", message);
        }

        public static void Loging(LogLevel level, string method, object message)
        {
            Write(level, method, message?.ToString());
        }

        public static void Error(string message)
        {
            Write(LogLevel.Fehler, "Unknown", message);
        }

        public static void Error(string method, string message)
        {
            Write(LogLevel.Fehler, method, message);
        }

        public static void Info(string method)
        {
            Write(LogLevel.Info, method, "");
        }

        public static void Info(string method, object message)
        {
            Write(LogLevel.Info, method, message?.ToString());
        }

        private static void Write(LogLevel level, string method, string message)
        {
            if (level <= Level)
                System.IO.File.AppendAllText(File, $"{DateTime.Now}\t{level}\t{method} => {message}" + Environment.NewLine);
        }
    }

    public enum LogLevel
    {
        KeinLog = 0,
        Fehler = 1,
        Info = 2,
    }
}