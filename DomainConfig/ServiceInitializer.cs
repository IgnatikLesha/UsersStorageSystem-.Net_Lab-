namespace DomainConfig
{
    using DAL.Configuration;
    using DAL.Infrastructure;
    using DAL.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;

    public static class ServiceInitializer
    {
        //private static ServiceHost serviceHost;
        public static MasterService GetMaster(IEnumerable<IUserService> serices)
        {
            return (MasterService)serices.Single(s => s is MasterService);
        }

        public static IEnumerable<SlaveService> GetSlaves(IEnumerable<IUserService> serices)
        {
            return serices.OfType<SlaveService>();
        }

        public static IEnumerable<IUserService> InitializeServices()
        {
            var serviceSection = ServiceRegisterConfigSection.GetConfig();
                        Dictionary<string, string> serviceConfigurations =
                            new Dictionary<string, string>(serviceSection.ServiceItems.Count);

            var servicesInfo = new List<ServiceConfigurationInfo>();

            for (int i = 0; i < serviceSection.ServiceItems.Count; i++)
                             {
                                 var serviceType = serviceSection.ServiceItems[i].ServiceType;
                                 var serviceName = serviceSection.ServiceItems[i].Path;
                                 serviceConfigurations[serviceName] = serviceType;
                                 IPAddress ipAddress;
                                 bool parsed = IPAddress.TryParse(serviceSection.ServiceItems[i].Ip, out ipAddress);
                                 IPEndPoint endPoint = null;
                                 
                                     endPoint = parsed?new IPEndPoint(IPAddress.Parse(serviceSection.ServiceItems[i].Ip),
                                         serviceSection.ServiceItems[i].Port):null;
                                     servicesInfo.Add(new ServiceConfigurationInfo
                                     {
                                         IpEndPoint = endPoint,
                                         Path = serviceName,
                                         ServiceType = serviceType
                                     });
                                 }
                                 
            IList<IUserService> services = new List<IUserService>();
                         foreach (var serviceConfiguration in serviceConfigurations)
                             {
                                 var domain = AppDomain.CreateDomain(serviceConfiguration.Key, null, null);
                                 var type = typeof(DomainServiceLoader);
                                 var loader = (DomainServiceLoader)domain.CreateInstanceAndUnwrap(Assembly.GetAssembly(type).FullName, type.FullName);
                if (serviceConfiguration.Key == "master")
                {
                    //ServiceConfigInfo info = new ServiceConfigInfo() { , port) };
                    var master = loader.LoadMaster();
                    master.Comunicator=new ServiceComunicator();
                    services.Add(master);
                        }
                else
                {
                    var service = loader.LoadSlave((MasterService)services.First());
                    services.Add(service);
                }
                                 
                            }
            for (int i = 0; i < servicesInfo.Count; i++)
            {
            services[i].AddConnectionInfo(servicesInfo[i]);
            }

            return services;

        }
        //private static void StartWcfService(IUserService service, string address)
        // { 
        //     serviceHost = new ServiceHost(service); 
        //     var behaviour = serviceHost.Description.Behaviors.Find<ServiceBehaviorAttribute>(); 
        //     behaviour.IncludeExceptionDetailInFaults = true; 
        //     behaviour.InstanceContextMode = InstanceContextMode.Single; 
        //     serviceHost.AddServiceEndpoint(typeof(IUserService), new NetTcpBinding(), $"net.tcp://{address}"); 
        //     serviceHost.Open(); 
        // }

    }
}
