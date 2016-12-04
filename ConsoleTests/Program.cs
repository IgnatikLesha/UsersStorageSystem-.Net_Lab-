using DAL.Infrastructure;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting;
using DAL.Entities;
using DomainConfig;
using DAL.Interfaces;
using SocketClient;
using SocketServer;

namespace ConsoleTests
{
    /// <summary>
    /// Show service info
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args">this args</param>
        public static void Main(string[] args)
        {
            IList<IUserService> services = ServiceInitializer.InitializeServices().ToList();
            ShowServicesInfo(services);
            Console.ReadKey();
        }

        /// <summary>
        /// Show method
        /// </summary>
        /// <param name="services">All services</param>
        private static void ShowServicesInfo(IEnumerable<IUserService> services)
        {
            var servicesList = services.ToList();
            Console.WriteLine("SERVICES INFO: \n");
            for (int i = 0; i < servicesList.Count; i++)
            {
                var service = servicesList[i];
                Console.Write("Service {0} : type = ", i);
                if (service is MasterService)
                {
                    Console.Write(" Master; ");
                }
                else
                {
                    Console.Write(" Slave; ");
                }

                Console.Write("Current Domain: " + AppDomain.CurrentDomain.FriendlyName + "; ");

                Console.Write("IsProxy: " + RemotingServices.IsTransparentProxy(service) + "; ");

                Console.WriteLine();
            }
        }
    }
}