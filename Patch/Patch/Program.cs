using System;
using System.IO;
using System.Reflection;

namespace Aselia.Patch
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolver);
            
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

        static Assembly AssemblyResolver(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(@"..\lib\", new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath))
            {
                return null;
            }
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
    }
}
 