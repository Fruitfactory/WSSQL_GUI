using System;
using System.IO;
using System.Linq;
using OF.Core.Core.LimeLM;
using OF.Core.Enums;

namespace OF.Unistall
{
    internal class Program
    {
        private const string ParamName = "unistall";

        private static void Main(string[] args)
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
            try
            {
                if (args == null || !args.Any())
                    return;
                if (args.Count() > 1)
                    return;
                string param = args[0];
                if (param != ParamName)
                    return;
                //if (TurboLimeActivate.Instance.State == ActivationState.Activated)
                //{
                    TurboLimeActivate.Instance.Deactivate(true);
                //}
                Console.Out.WriteLine("Done...");
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }
    }
}