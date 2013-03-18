using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace iLabStatusService
{
    static class iLabStatus
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun = null;

            // More than one user Service may run within the same process. To add
            // another service to this process, change the following line to
            // create a second service object. For example,
            //
            //   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
            //
            if(args.Length == 0)
                ServicesToRun = new ServiceBase[] { new iLabStatusService() };
             else if (args.Length == 1)
                ServicesToRun = new ServiceBase[] { new iLabStatusService(args[1]) };
            else if (args.Length > 1)
                ServicesToRun = new ServiceBase[] { new iLabStatusService(args[1],args[2]) };

            ServiceBase.Run(ServicesToRun);
        }
    }
}