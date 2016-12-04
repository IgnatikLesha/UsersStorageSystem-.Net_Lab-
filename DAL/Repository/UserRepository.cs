namespace DAL.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Interfaces;
    using Infrastructure;
    using System.Configuration;


    [Serializable]
    public class UserRepository:IRepository<User>
    {
        public int LastId { get; set; }
        public List<User> Users { get; set; }

        public UserRepository()
        {
            LastId = Convert.ToInt32(ConfigurationManager.AppSettings["Id"]);
            Users = new List<User>();
        }

        public UserRepository(int lastId)
        {
            LastId = lastId;
            Users = new List<User>();
        }

        public int Create(User user)
        {
            if (!Users.Contains(user) && new UserValidation().Validate(user))
            {
                user.Id = IdIterator.GetNextId(LastId);
                ConfigurationManager.AppSettings["Id"] = LastId.ToString();
                Users.Add(user);
                return user.Id;
            }
            return -1;
        }

        public IEnumerable<User> Find(Func<User, bool> predicate)
        {
            return Users.Where(predicate).ToList();
        }

        public bool Delete(User user)
        {
            if (Users.Contains(user))
            {
                Users.Remove(user);
                return true;
            }
            return false;
        }
    }
}
