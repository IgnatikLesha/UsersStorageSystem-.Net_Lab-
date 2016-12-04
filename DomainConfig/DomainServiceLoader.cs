namespace DomainConfig
{
    using DAL.Infrastructure;
    using System;

    public class DomainServiceLoader:MarshalByRefObject
    {
        public MasterService LoadMaster()
        {
            return new MasterService();
        }
        public SlaveService LoadSlave(MasterService service)
        {
            return new SlaveService(service);
        }
    }
}
