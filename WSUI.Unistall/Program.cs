using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WSUI.Core.Core.LimeLM;
using WSUI.Core.Enums;

namespace WSUI.Unistall
{
    class Program
    {
        private const string ParamName = "unistall";

        static void Main(string[] args)
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
                if (TurboLimeActivate.Instance.State == ActivationState.Activated)
                {
                    TurboLimeActivate.Instance.Deactivate(true);
                }
            }
            catch
            {
            }
            
        }
    }
}
