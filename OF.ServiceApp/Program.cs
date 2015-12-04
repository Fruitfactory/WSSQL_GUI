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
        static void Main(string[] args)
        {
            IOFServiceBootstraper bootstraper = new OFServiceBootstraper();
            bootstraper.Initialize();
            bootstraper.Run();

            bootstraper.Exit();
        }
    }
}
