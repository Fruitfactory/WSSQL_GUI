using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OF.Core.Core.LimeLM;
using OF.Core.Helpers;

namespace OF.Install
{
    class Program
    {
        private const string JavaHomeVar = "JAVA_HOME";
        static void Main(string[] args)
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            using (var textWriter = File.CreateText(Path.Combine(folder, "install.txt")))
            {
                try
                {
                    string javaPath = OFRegistryHelper.Instance.GetJavaInstallationPath();
                    if (!string.IsNullOrEmpty(javaPath))
                    {

                        Environment.SetEnvironmentVariable(JavaHomeVar, javaPath, EnvironmentVariableTarget.Machine);
                    }
                    else
                    {
                        textWriter.WriteLine(string.Format("Java Runtime Path is empty!!!"));
                    }
                }
                catch (Exception ex)
                {
                    textWriter.WriteLine(ex.Message);
                }    
            }
        }
    }
}
