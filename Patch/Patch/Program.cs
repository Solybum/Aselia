using System;
using System.Reflection;

namespace Aselia.Patch
{
    class Program
    {
        static void Main(string[] args)
        {
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
 