using System;
using System.Collections.Generic;
using DAL.Entities;
using DAL.Infrastructure;
using DAL.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest
{
    /// <summary>
    /// Tests of main service responsibility.
    /// </summary>
    [TestClass]
    public class UserServiceTest
    {
        /// <summary>
        /// Adding users
        /// </summary>
        [TestMethod]
        public void AddUser_AddThreeUsers_ReturnArrayWithThreeUsers()
        {
            UserRepository repository = new UserRepository();
            UserService service = new UserService(repository);
            User user1 = new User { FirstName = "Lisa", LastName = "Rich" };
            User user2 = new User { FirstName = "Nick", LastName = "Name" };
            User user3 = new User { FirstName = "Tim", LastName = "Rodny" };

            var result = new List<int> { service.AddUser(user1), service.AddUser(user2), service.AddUser(user3) };

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        /// <summary>
        /// Adding an exist user
        /// </summary>
        [TestMethod]
        public void AddUser_AddAnExistUser_ReturnMinusOne()
        {
            UserRepository repository = new UserRepository();
            UserService service = new UserService(repository);
            User user1 = new User { FirstName = "Lisa", LastName = "Rich" };
            User user2 = new User { FirstName = "Lisa", LastName = "Rich" };
            List<int> result = new List<int> { service.AddUser(user1), service.AddUser(user2) };

            Assert.IsNotNull(result);
            Assert.AreEqual(-1, result[1]);
        }

        /// <summary>
        /// Deleting user
        /// </summary>
        [TestMethod]
        public void DeleteUser_DeleteOneExistUser()
        {
            UserRepository repository = new UserRepository();
            UserService service = new UserService(repository);
            User user1 = new User { FirstName = "Lisa", LastName = "Rich" };
            User user2 = new User { FirstName = "Lisa", LastName = "Rich" };
            List<int> result = new List<int> { service.AddUser(user1) };
            service.Delete(user2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, repository.Users.Count);
        }

        /// <summary>
        /// Saving and loading users
        /// </summary>
        [TestMethod]
        public void SaveAndLoad_Users()
        {
            var repo = new UserRepository();
            repo.Create(new User
            {
                FirstName = "Lisa",
                LastName = "Rich"
            });
            repo.Create(new User
            {
                FirstName = "Nick",
                LastName = "Name"
            });
            repo.Create(new User
            {
                FirstName = "Tim",
                LastName = "Rodny"
            });
            var service = new UserService(repo);
            service.Save();

            // service.Load();

            Assert.AreEqual(3, repo.Users.Count);
        }
    }
}
