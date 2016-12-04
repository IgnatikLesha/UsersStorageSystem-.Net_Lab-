namespace DAL.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Interfaces;
    using Repository;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Xml.Serialization;
    using DAL.Configuration;
    using System.Threading;

    [Serializable]
    public class SlaveService: MarshalByRefObject,IUserService
    {
        public UserRepository UserRepository { get; private set; }
    
        private static int countOfSlaves ;

        public ServiceConfigurationInfo ServiceConfigurationInfo { get; set; }

        private ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();


        public SlaveService(){ }
        public SlaveService(MasterService srvc)
        {
            UserRepository = srvc.UserRepository;
            var items = ServiceRegisterConfigSection.GetConfig().ServiceItems;
            int sk = 0;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ServiceType == "Slave")
                    sk++;
            }

            if (countOfSlaves >= sk)
            {
                throw new ArgumentException("There is no way to create more than 4 instances of Slave class");
            }

            countOfSlaves++;
            srvc.Comunicator.Message+= SlaveListener;
        }

        public void SlaveListener(Object sender, ActionEventArgs eventArgs)
        {
        }

        public int AddUser(User user)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> SearchForUsers(Func<User, bool> predicate)
        {
            rwl.EnterReadLock();
            try
            {
                return UserRepository.Find(predicate);
            }
            finally
            {
                rwl.ExitReadLock();
            }
        }

        public bool Delete(User user)
        {
            throw new NotImplementedException();
        }


        public void Load()
        {
            rwl.EnterReadLock();
            try
            {
                var loader = new XmlSerializer(typeof (List<User>));
                string file;
                try
                {
                    file = ConfigurationManager.AppSettings["xmlfile"];
                }
                catch (ConfigurationException e)
                {
                    throw;
                }

                using (var fileStr = new FileStream(file, FileMode.OpenOrCreate))
                {
                    UserRepository.Users = (List<User>) loader.Deserialize(fileStr);
                    UserRepository.LastId = UserRepository.Users.Last().Id;
                }
            }
            finally
            {
                rwl.ExitReadLock();
            }
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void AddConnectionInfo(ServiceConfigurationInfo info)
        {
            ServiceConfigurationInfo = info;
        }
    }
}
