using System;
using System.Collections.Generic;
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
            string source = args[0];
            string buildNumber = Properties.Settings.Default.BuildNumber;
            string setupProject = string.Format("{0}{1}", source, Properties.Settings.Default.SetupProjectFile);
            GenerateVersionFile(source,buildNumber);
            UpdateSetupProject(setupProject,buildNumber);
        }

        static void GenerateVersionFile(string sourceDir,string buildNumber)
        {
            string filename = string.Format("{0}\\{1}",sourceDir,Properties.Settings.Default.VersionFilename);
            using (StreamWriter writer = new StreamWriter(filename,false,Encoding.UTF8))
            {
                writer.WriteLine(UsingPart);
                writer.WriteLine(AssemblyVersion,buildNumber);
                writer.WriteLine(AssemblyFileVersion, buildNumber);
            }
        }
        
        static void UpdateSetupProject(string setupProject, string buildNumber)
        {
            #region [for visuall setup project]

            //string content = string.Empty;
            //using (StreamReader reader = new StreamReader(setupProject))
            //{
            //    content = reader.ReadToEnd();
            //}
            //if (string.IsNullOrEmpty(content))
            //{
            //    Console.WriteLine("Content of setup project is empty.");
            //    return;
            //}
            //content = Regex.Replace(content, Properties.Settings.Default.ProductVersion,
            //                        string.Format(Properties.Settings.Default.ProductVersionTemplate, buildNumber),
            //                        RegexOptions.IgnoreCase);
            //string guid = "{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}";
            //content = Regex.Replace(content, Properties.Settings.Default.ProductCode,
            //                        string.Format(Properties.Settings.Default.ProductCodeTemplate, guid),
            //                        RegexOptions.IgnoreCase);
            //using (StreamWriter writer = new StreamWriter(setupProject, false, Encoding.UTF8))
            //{
            //    writer.Write(content);
            //}

            #endregion

            if(!File.Exists(setupProject))
                return;


            XDocument doc = XDocument.Load(setupProject);

            XElement program = doc.Root.FirstNode as XElement;
            if (program != null)
            {
                XAttribute attr = program.Attribute("Id");
                attr.Value = Guid.NewGuid().ToString().ToUpperInvariant();
                attr = program.Attribute("Version");
                attr.Value = buildNumber;
            }

            doc.Save(setupProject);
        }
    }
}
