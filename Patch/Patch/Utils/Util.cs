using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Aselia.Patch.Utils
{
    public static class Util
    {
        public static long Time()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static IPAddress ParseIPAddress(string value)
        {
            if (string.Compare(value, "any", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return IPAddress.Any;
            }
            else if (string.Compare(value, "localhost", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return IPAddress.Loopback;
            }
            else if (string.Compare(value, "auto", StringComparison.OrdinalIgnoreCase) == 0)
            {
                try
                {
                    IPHostEntry iphe;
                    iphe = Dns.GetHostEntry(Dns.GetHostName());
                    return iphe.AddressList[0];
                }
                catch
                {

                }
            }
            else if (Regex.IsMatch(value, "^([0-9]{1,3}\\.){3}[0-9]{1,3}$"))
            {
                try
                {
                    return IPAddress.Parse(value);
                }
                catch { }
            }
            else
            {
                try
                {
                    IPHostEntry iphe;
                    iphe = Dns.GetHostEntry(value);
                    return iphe.AddressList[0];
                }
                catch { }
            }
            return null;
        }
        public static string StringRemoveNull(string text)
        {
            if (text == null)
            {
                return null;
            }

            int pos = text.IndexOf('\0', 0);
            if (pos > 0)
            {
                text = text.Substring(0, pos);
            }
            return text.Replace("\0", string.Empty);
        }
    }
}
