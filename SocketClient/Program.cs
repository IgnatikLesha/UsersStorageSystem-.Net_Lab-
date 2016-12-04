namespace SocketClient
{
using DAL.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DAL.Entities;
using DAL.Infrastructure;
using DAL.Interfaces;
using DomainConfig;

    class Program
    {
        public static void Main(string[] args)
        {

            IList<IUserService> services = ServiceInitializer.InitializeServices().ToList();
            IList<SlaveService> slaves = ServiceInitializer.GetSlaves(services).ToList();
            MasterService master = ServiceInitializer.GetMaster(services);
            List<ServiceConfigurationInfo> infos = new List<ServiceConfigurationInfo>();
            foreach (var server in slaves)
            {
                infos.Add(server.ServiceConfigurationInfo);
            }
            AsynchronousClient client = new AsynchronousClient(infos);
            client.StartClient();
            RunMaster(master, client);
            while (true)
            {
                var quit = Console.ReadKey();
                if (quit.Key == ConsoleKey.Escape)
                    break;
            }
            master.Save();

        }
        private static void RunMaster(MasterService master, AsynchronousClient client)
        {
            Random rand = new Random();
            ThreadStart masterSearch = () =>
            {
                while (true)
                {
                    var serachresult = master.SearchForUsers(u => u.FirstName != null);
                    Console.Write("Master search results: ");
                    foreach (var result in serachresult)
                        Console.Write(result.FirstName + " " + result.LastName);
                    Console.WriteLine();
                   
                    Thread.Sleep(rand.Next(1000, 5000));
                }
            };

            ThreadStart masterAddDelete = () =>
            {
                var users = new List<User>
                {
                    new User { FirstName = "Alick", LastName = "Nero"},
                    new User { FirstName = "Alice", LastName = "Cooper"}
                };
               
                User userToDelete = null;
                while (true)
                {
                    foreach (var user in users)
                    {
                        int addChance = rand.Next(0, 3);
                        if (addChance == 0)
                        {
                            master.AddUser(user);
                            //client.Message = master.Comunicator.GetMessage();
                            //client.StartClient();
                            client.Send(master.Comunicator.GetMessage());
                            //client.sendDone.WaitOne();
                            //client.Recieve();
                        }

                        Thread.Sleep(rand.Next(1000, 5000));
                        if (userToDelete != null)
                        {
                            int deleteChance = rand.Next(0, 3);
                            if (deleteChance == 0)
                                master.Delete(userToDelete);
                           
                            client.Send(master.Comunicator.GetMessage());
                           
                        }

                        userToDelete = user;
                        Thread.Sleep(rand.Next(1000, 5000));
                        Console.WriteLine();
                    }
                }
            };

            Thread masterSearchThread = new Thread(masterSearch) { IsBackground = true };
            Thread masterAddThread = new Thread(masterAddDelete) { IsBackground = true };
            masterAddThread.Start();
            masterSearchThread.Start();
        }
   
    }
}
