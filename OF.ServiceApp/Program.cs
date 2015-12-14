using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OF.ServiceApp.Bootstraper;
using OF.ServiceApp.Interfaces;

namespace OF.ServiceApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            IOFServiceBootstraper bootstraper = new OFServiceBootstraper();

            if (bootstraper.IsApplicationAlreadyWorking())
            {
                Console.WriteLine("Another copy is running...");
                return;
            }

            bootstraper.Initialize();
            bootstraper.Run();

            bootstraper.Exit();
        }
    }
}
