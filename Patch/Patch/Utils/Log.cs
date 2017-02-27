using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aselia.Patch.Utils
{
    public static class Log
    {
        public enum Type
        {
            None,
            Debug,
            Server,
            Conn,
        }
        private static List<string> logFileNames = new List<string>()
        {
            "",
            "debug.log",
            "server.log",
            "conn.log",
        };
   
        private static void LogException(string fmt, Exception ex)
        {
            Error(Type.Server, "Invalid log format or arguments: {0}\n{1}", fmt, ex);
        }
        private static void PrepareFormat(ref string fmt, ref string log)
        {
            while (fmt.StartsWith("\n"))
            {
                log += "\n";
                fmt = fmt.Remove(0, 1);
            }
        }
        private static void LogWrite(Type type, string text)
        {
            // Do not write connection logs to console
            if (type != Type.Conn)
            {
                Console.Write(text);
            }

            // Write to file only if we have a file name
            if (type != Type.None)
            {
                try
                {

                }
                catch { } // Ignore
            }

            try
            {
                string directory = Path.Combine("log", "patch");
                string fileName = DateTime.UtcNow.ToString("yyyy-MM-dd") + "-" + logFileNames[(int)type];

                Directory.CreateDirectory(directory);
                File.AppendAllText(Path.Combine(directory, fileName), text, Encoding.Unicode);
            }
            catch { } // Ignore
        }
        
        public static void Info(Type type, string fmt, params object[] args)
        {
            try
            {
                string log = string.Empty;
                PrepareFormat(ref fmt, ref log);
                log += string.Format("[{0:s}] [I] {1}\n", DateTime.UtcNow, string.Format(fmt, args));
                LogWrite(type, log);
            }
            catch (Exception ex)
            {
                LogException(fmt, ex);
            }
        }
        public static void Warning(Type type, string fmt, params object[] args)
        {
            string log = string.Empty;
            try
            {
                PrepareFormat(ref fmt, ref log);
                log += string.Format("[{0:s}] [W] {1}\n", DateTime.UtcNow, string.Format(fmt, args));
            }
            catch (Exception ex)
            {
                LogException(fmt, ex);
            }
        }
        public static void Error(Type type, string fmt, params object[] args)
        {
            string log = string.Empty;
            try
            {
                PrepareFormat(ref fmt, ref log);
                log += string.Format("[{0:s}] [E] {1}\n", DateTime.UtcNow, string.Format(fmt, args));
            }
            catch (Exception ex)
            {
                LogException(fmt, ex);
            }
        }
        public static void Debug(Type type, string fmt, params object[] args)
        {
            string log = string.Empty;
            try
            {
                PrepareFormat(ref fmt, ref log);
                log += string.Format("[{0:s}] [D] {1}\n", DateTime.UtcNow, string.Format(fmt, args));
            }
            catch (Exception ex)
            {
                LogException(fmt, ex);
            }
        }
    }
}
