using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Patch
{
    [DataContract]
    public class Configuration
    {
        public IPAddress _ipAddress;
        public IPAddress _ipRedirect;
        public IPAddress _ipLogin;

        [DataMember] public string ipAddress;
        [DataMember] public string ipRedirect;
        [DataMember] public int port;
        [DataMember] public int maxSpeed;
        [DataMember] public int maxClients;
        [DataMember] public int maxConcurrentConnections;
        [DataMember] public bool noUpdates;
        [DataMember] public string updatesPath;
        [DataMember] public string motd;
        
        public Configuration()
        {

        }

        public bool CheckConfiguration()
        {
            _ipAddress = Utils.ParseIPAddress(ipAddress);
            if (_ipAddress == null)
            {
                Log.Write(Log.Level.Error, Log.Type.Server, "Error parsing IP address '{0}'", ipAddress);
                return false;
            }
            _ipRedirect = Utils.ParseIPAddress(ipRedirect);
            if (_ipRedirect == null)
            {
                Log.Write(Log.Level.Error, Log.Type.Server, "Error parsing IP address '{0}'", ipRedirect);
                return false;
            }

            maxSpeed *= 1024;

            return true;
        }
        public void LogConfiguration()
        {
            Log.Write(Log.Level.Info, Log.Type.None, "Server Configuration");
#if DEBUG
            Log.Write(Log.Level.Info, Log.Type.None, "IP Address: -:{1}", _ipAddress.ToString(), port);
            Log.Write(Log.Level.Info, Log.Type.None, "IP Redirect: -", _ipRedirect.ToString());
#else
            Log.Write(Log.Level.Info, Log.Type.None, "IP Address: {0}:{1}", _ipAddress.ToString(), port);
            Log.Write(Log.Level.Info, Log.Type.None, "IP Redirect: {0}", _ipRedirect.ToString());
#endif
            Log.Write(Log.Level.Info, Log.Type.None, "Maximum upload speed: {0}", maxSpeed / 1024);
            Log.Write(Log.Level.Info, Log.Type.None, "Maximum client connections: {0}", maxClients);
            Log.Write(Log.Level.Info, Log.Type.None, "Updates path: {0}", Path.Combine(Directory.GetCurrentDirectory(), updatesPath));
            Log.Write(Log.Level.Info, Log.Type.None, "Updates disabled: {0}", noUpdates);
        }
    }
}
