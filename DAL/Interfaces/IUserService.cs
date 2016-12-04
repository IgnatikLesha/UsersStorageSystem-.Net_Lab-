namespace DAL.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Entities;
    using System.ServiceModel;

    public interface IUserService
    {
        int AddUser(User user);

        IEnumerable<User> SearchForUsers(Func<User, bool> predicate);

        bool Delete(User user);

        void Load();

        void Save();

        void AddConnectionInfo(ServiceConfigurationInfo info);
    }
}
