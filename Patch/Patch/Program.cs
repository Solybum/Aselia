using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using Aselia.Patch.Properties;

namespace Aselia.Patch
{
    class Program
    {
        /// <summary>
        /// Custom assembly resolver so we can store all the libraries 
        /// in a single folder while still being able to separate 
        /// each server on it's own directory.
        /// This method currently does not work in Mono
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {   
            string assemblyPath = Path.Combine("..", "lib", new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath))
            {
                return null;
            }
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        public static bool SetupSettingsForMono()
        {
            if (Type.GetType("Mono.Runtime") == null)
            {
                // Not Mono
                return true;
            }

            if (ConfigurationManager.AppSettings[nameof(Settings.Default.IPAddress)]                == null ||
                ConfigurationManager.AppSettings[nameof(Settings.Default.IPRedirect)]               == null ||
                ConfigurationManager.AppSettings[nameof(Settings.Default.Port)]                     == null ||
                ConfigurationManager.AppSettings[nameof(Settings.Default.MaxSpeed)]                 == null ||
                ConfigurationManager.AppSettings[nameof(Settings.Default.MaxClients)]               == null ||
                ConfigurationManager.AppSettings[nameof(Settings.Default.MaxConcurrentConnections)] == null ||
                ConfigurationManager.AppSettings[nameof(Settings.Default.DisableUpdates)]           == null ||
                ConfigurationManager.AppSettings[nameof(Settings.Default.UpdatesPath)]              == null ||
                ConfigurationManager.AppSettings[nameof(Settings.Default.MOTD)]                     == null)
            {
                Console.WriteLine("One or more settings were not found\n" +
                    "Please fix that before running the server with mono");
                return false;
            }

            Settings.Default.IPAddress                = ConfigurationManager.AppSettings[nameof(Settings.Default.IPAddress)];
            Settings.Default.IPRedirect               = ConfigurationManager.AppSettings[nameof(Settings.Default.IPRedirect)];
            Settings.Default.Port                     = int.Parse(ConfigurationManager.AppSettings[nameof(Settings.Default.Port)]);
            Settings.Default.MaxSpeed                 = int.Parse(ConfigurationManager.AppSettings[nameof(Settings.Default.MaxSpeed)]);
            Settings.Default.MaxClients               = int.Parse(ConfigurationManager.AppSettings[nameof(Settings.Default.MaxClients)]);
            Settings.Default.MaxConcurrentConnections = int.Parse(ConfigurationManager.AppSettings[nameof(Settings.Default.MaxConcurrentConnections)]);
            Settings.Default.DisableUpdates           = bool.Parse(ConfigurationManager.AppSettings[nameof(Settings.Default.DisableUpdates)]);
            Settings.Default.UpdatesPath              = ConfigurationManager.AppSettings[nameof(Settings.Default.UpdatesPath)];
            Settings.Default.MOTD                     = ConfigurationManager.AppSettings[nameof(Settings.Default.MOTD)];
            return true;
        }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
            if (!SetupSettingsForMono())
            {
                return;
            }

            string cmdtitle = string.Format("{0} v{1}.{2}",
                ((AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0]).Title,
                Assembly.GetExecutingAssembly().GetName().Version.Major,
                Assembly.GetExecutingAssembly().GetName().Version.Minor);

            Console.Title = cmdtitle;
            Console.WriteLine(cmdtitle + "\n");

            Server s = new Server();
            s.Start();
#if DEBUG
            Console.Read();
#endif
        }
    }
}
 