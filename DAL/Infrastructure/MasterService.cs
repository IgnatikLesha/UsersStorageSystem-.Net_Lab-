namespace DAL.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Xml.Serialization;
    using Configuration;
    using Interfaces;
    using Entities;
    using Repository;
    using System.Xml;
    using System.Xml.Linq;

    [Serializable]
    public class MasterService : MarshalByRefObject, IUserService
    {
        public UserRepository UserRepository { get; private set; }
        public ServiceConfigurationInfo ServiceConfigurationInfo { get; set; }
        public ServiceComunicator Comunicator { get; set; }

        private ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();


        public MasterService()
        {
            UserRepository = new UserRepository();
        }

        public MasterService(IRepository<User> rep)
        {
            UserRepository = (UserRepository)rep;
        }


        public int AddUser(User user)
        {
            rwl.EnterWriteLock();
            try
            {
                OnMessage(new ActionEventArgs(){Message = "User added/created"});
                return UserRepository.Create(user);
            }
            finally
            {
                rwl.ExitWriteLock();
            }
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
            rwl.EnterWriteLock();
            try
            {
                OnMessage(new ActionEventArgs{Message = "User deleted"});
                return UserRepository.Delete(user);
            }
            finally
            {
                rwl.ExitWriteLock();
            }

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
            rwl.EnterWriteLock();
            try
            {
                var saver = new XmlSerializer(typeof (List<User>));
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
                    saver.Serialize(fileStr, UserRepository.Users);
                }
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }
    
        private void OnMessage(ActionEventArgs e)
        {
            if(Comunicator!=null) Comunicator.Send(e);
        }

        public void AddConnectionInfo(ServiceConfigurationInfo info)
        {
            ServiceConfigurationInfo = info;
        }
    }
}
