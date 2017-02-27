using System.IO;
using System.Net;
using Aselia.Patch.Properties;
using Aselia.Patch.Utils;

namespace Aselia.Patch
{
    public class Configuration
    {
        public IPAddress _ipAddress;
        public IPAddress _ipRedirect;
        
        public Configuration()
        {

        }

        public bool CheckConfiguration()
        {
            _ipAddress = Util.ParseIPAddress(Settings.Default.IPAddress);
            if (_ipAddress == null)
            {
                Log.Error(Log.Type.Server, "Error parsing IP address '{0}'", Settings.Default.IPAddress);
                return false;
            }
            _ipRedirect = Util.ParseIPAddress(Settings.Default.IPRedirect);
            if (_ipRedirect == null)
            {
                Log.Error(Log.Type.Server, "Error parsing IP address '{0}'", Settings.Default.IPRedirect);
                return false;
            }
            return true;
        }
        public void LogConfiguration()
        {
            Log.Info(Log.Type.Server, "Server Configuration");
#if DEBUG
            Log.Info(Log.Type.Server, "IP Address: -:{1}", _ipAddress.ToString(), Settings.Default.Port);
            Log.Info(Log.Type.Server, "IP Redirect: -", _ipRedirect.ToString());
#else
            Log.Info(Log.Type.Server, "IP Address: {0}:{1}", _ipAddress.ToString(), Settings.Default.Port);
            Log.Info(Log.Type.Server, "IP Redirect: {0}", _ipRedirect.ToString());
#endif
            Log.Info(Log.Type.Server, "Maximum upload speed: {0} KB/s", Settings.Default.MaxSpeed / 1024);
            Log.Info(Log.Type.Server, "Maximum client connections: {0}", Settings.Default.MaxClients);
            Log.Info(Log.Type.Server, "Updates path: {0}", Path.Combine(Directory.GetCurrentDirectory(), Settings.Default.UpdatesPath));
            Log.Info(Log.Type.Server, "Updates disabled: {0}", Settings.Default.DisableUpdates);
        }
    }
}
