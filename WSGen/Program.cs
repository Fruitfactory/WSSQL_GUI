using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace WSGen
{
    class Program
    {
        private const string BuildNumberFilename = "wsui.number";


        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Source dir argument is absent.");
                return;
            }
            string source = args[0];
            string workingDir = Assembly.GetAssembly(typeof (Program)).Location;
            workingDir = workingDir.Substring(0, workingDir.LastIndexOf('/'));
            string filePath = workingDir + BuildNumberFilename;


        }



    }
}
