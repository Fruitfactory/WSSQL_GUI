﻿using System;
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


            var revision = (int)(DateTime.UtcNow - new DateTime(2013, 8, 20)).TotalDays;

            string source = args[0];
            string buildNumber = string.Format("{0}.{1}.{2}.{3}", Properties.Settings.Default.Major, Properties.Settings.Default.Minor,Properties.Settings.Default.Build, revision);
            string setupProject = string.Format("{0}{1}", source, Properties.Settings.Default.SetupProjectFile);
            string bootstrapperProject = string.Format("{0}{1}", source, Properties.Settings.Default.BootstrapperProject);
            GenerateVersionFile(source, buildNumber);
            UpdateSetupProjects(setupProject, buildNumber);
            UpdateSetupProjects(bootstrapperProject,buildNumber);
            Properties.Settings.Default["Revision"] = Convert.ToInt32(revision);
            Properties.Settings.Default["BuildNumber"] = buildNumber;
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

        static void UpdateSetupProjects(string setupProject, string buildNumber)
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
