
namespace DAL.Configuration
{
    using System;
    using System.Net;


    [Serializable]
    public class ServiceConfigurationInfo
    {
        public IPEndPoint IpEndPoint { get; set; }
        public string ServiceType { get; set; }
        public string Path { get; set; }
    }
}
