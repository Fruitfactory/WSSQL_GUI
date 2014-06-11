using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WSUI.Core.Core.LimeLM;
using WSUI.Core.Enums;
using WSUI.Core.Logger;

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
                    var result = TurboLimeActivate.Instance.Deactivate(true);
                    WSSqlLogger.Instance.LogError("Deactivating result : {0}", result);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                WSSqlLogger.Instance.LogError("Deactivating: {0}", ex.Message);
            }
            
        }
    }
}
