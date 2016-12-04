namespace SocketServer
{
    using DAL.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using DAL.Infrastructure;
    using DAL.Interfaces;
    using DomainConfig;

    class Program
    {
        public static void Main(string[] args)
        {
            IList<IUserService> services = ServiceInitializer.InitializeServices().ToList();
            IList<SlaveService> slaves = ServiceInitializer.GetSlaves(services).ToList();

            foreach (var slave in slaves)
            {
                var slaveThread = new Thread(() =>
                {
                    AsynchronousSocketListener.StartListening(slave.ServiceConfigurationInfo);
                });
                slaveThread.Start();
            }
            while (true)
            {
                var quit = Console.ReadKey();
                if (quit.Key == ConsoleKey.Escape)
                    break;
            }
        }
    }
}
