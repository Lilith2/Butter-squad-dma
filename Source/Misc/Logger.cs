using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace squad_dma.Source.Misc
{
    public static class Logger
    {
        private static readonly bool IsDebugMode;
        private static readonly bool IsConsoleEnabled;

        static Logger()
        {
#if DEBUG
            IsDebugMode = true;
            IsConsoleEnabled = true;
#else
            IsDebugMode = false;
            IsConsoleEnabled = false;
#endif
        }

        [Conditional("DEBUG")]
        public static void Debug(string message, [CallerMemberName] string caller = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (!IsDebugMode) return;
            WriteLog("DEBUG", message);
        }

        public static void Info(string message, [CallerMemberName] string caller = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            WriteLog("INFO", message);
        }

        public static void Error(string message, [CallerMemberName] string caller = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            WriteLog("ERROR", message);
        }

        private static void WriteLog(string level, string message)
        {
            if (!IsConsoleEnabled) return;
            
            var logMessage = $"[{level}] {message}";
            Console.WriteLine(logMessage);
            System.Diagnostics.Debug.WriteLine(logMessage);
        }
    }
} 