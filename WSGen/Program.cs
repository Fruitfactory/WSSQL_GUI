using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WSGen
{
    internal class Program
    {
        private const string UsingPart =
            "using System.Reflection;\nusing System.Runtime.CompilerServices;\nusing System.Runtime.InteropServices;\n\n";

        private const string AssemblyVersion = "[assembly: AssemblyVersion(\"{0}\")]";
        private const string AssemblyFileVersion = "[assembly: AssemblyFileVersion(\"{0}\")]";

        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("Source dir argument is absent.");
                return;
            }


            var build = Convert.ToInt16(Properties.Settings.Default.BuildCount);

            string source = args[0];
            string buildNumber = string.Format("{0}.{1}", Properties.Settings.Default.BuildNumber, build);
            string setupProject = string.Format("{0}{1}", source, Properties.Settings.Default.SetupProjectFile);
            GenerateVersionFile(source, buildNumber);
            UpdateSetupProject(setupProject, buildNumber);
            Properties.Settings.Default["BuildCount"] = Convert.ToInt32(++build);
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        static void GenerateVersionFile(string sourceDir, string buildNumber)
        {
            string filename = string.Format("{0}\\{1}", sourceDir, Properties.Settings.Default.VersionFilename);
            using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                writer.WriteLine(UsingPart);
                writer.WriteLine(AssemblyVersion, buildNumber);
                writer.WriteLine(AssemblyFileVersion, buildNumber);
            }
        }

        static void UpdateSetupProject(string setupProject, string buildNumber)
        {
            if (!File.Exists(setupProject))
                return;


            XDocument doc = XDocument.Load(setupProject);

            XElement program = doc.Root.FirstNode as XElement;
            if (program != null)
            {
                //XAttribute attr = program.Attribute("Id");
                //attr.Value = Guid.NewGuid().ToString().ToUpperInvariant();
                XAttribute attr = program.Attribute("Version");
                attr.Value = buildNumber;
            }

            doc.Save(setupProject);
        }

    }
}
