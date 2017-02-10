using System;
using System.IO;
using System.Text;

namespace Aselia.Patch
{
    public class Log
    {
        public enum Level
        {
            Info,
            Warning,
            Error,
            Debug,
            None,
        }
        public enum Type
        {
            None,
            Server,
            Conn,
            Debug,
        }
        public static void Write(Level level, Type type, string fmt, params object[] args)
        {
            // This thing could probably be done once at startup if we are going to rotate logs per month...
            string dir = Path.Combine("log", "patch");
            string date = DateTime.UtcNow.ToString("yyyy-MM");

            string log = string.Empty;
            try
            {
                while (fmt.StartsWith("\n"))
                {
                    log += "\n";
                    fmt = fmt.Remove(0, 1);
                }
                switch (level)
                {
                    case Level.Info:
                    log += string.Format("[{0:s}] [I] {1}\n", DateTime.UtcNow, string.Format(fmt, args));
                    break;
                    case Level.Warning:
                    log += string.Format("[{0:s}] [W] {1}\n", DateTime.UtcNow, string.Format(fmt, args));
                    break;
                    case Level.Error:
                    log += string.Format("[{0:s}] [E] {1}\n", DateTime.UtcNow, string.Format(fmt, args));
                    break;
                    case Level.Debug:
                    log += string.Format("[{0:s}] [D] {1}\n", DateTime.UtcNow, string.Format(fmt, args));
                    break;
                    default:
                    log += string.Format("[{0:s}] {1}\n", DateTime.UtcNow, string.Format(fmt, args));
                    break;
                }
            }
            catch
            {
                Write(Level.Error, Type.Server, "Invalid log format or arguments: {0}", fmt);
                return;
            }

            try
            {
                if (type != Type.Conn)
                {
                    Console.Write(log);
                }

                Directory.CreateDirectory(dir);
                switch (type)
                {
                    case Type.None:
                    {
                    }
                    break;
                    case Type.Server:
                    {
                        File.AppendAllText(Path.Combine(dir, date + "-patch.log"), log, Encoding.Unicode);
                    }
                    break;
                    case Type.Conn:
                    {
                        File.AppendAllText(Path.Combine(dir, date + "-conn.log"), log, Encoding.Unicode);
                    }
                    break;
                    case Type.Debug:
                    {
                        File.AppendAllText(Path.Combine(dir, date + "-debug.log"), log, Encoding.Unicode);
                    }
                    break;
                    default:
                    break;
                }
            }
            catch { } // Ignore
        }
    }
}
