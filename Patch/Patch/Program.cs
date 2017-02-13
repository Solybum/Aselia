using System;
using System.IO;
using System.Reflection;

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

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);

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
