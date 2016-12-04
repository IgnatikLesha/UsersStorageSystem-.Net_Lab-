namespace DAL.Configuration
{
    using System.Configuration;

    public class ServiceRegisterConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Services")]
        public ServiceCollection ServiceItems
        {
            get { return ((ServiceCollection)(base["Services"])); }
        }

        public static ServiceRegisterConfigSection GetConfig()
        {
            return (ServiceRegisterConfigSection)ConfigurationManager.GetSection("ServiceRegister") ?? new ServiceRegisterConfigSection();
        }

    }
}
