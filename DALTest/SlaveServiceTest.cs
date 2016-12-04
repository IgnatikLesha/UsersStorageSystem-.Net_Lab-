using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Entities;
using DAL.Infrastructure;
using DAL.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest
{
    /// <summary>
    /// Test of slave responsibility
    /// </summary>
    [TestClass]
    public class SlaveServiceTest
    {
        /// <summary>
        /// Searching users
        /// </summary>
        [TestMethod]
        public void Find_FindUserByLastName_ResultTwoUsers()
        {
            UserRepository repo = new UserRepository();

            User user1 = new User { FirstName = "Lisa", LastName = "Rich" };
            User user2 = new User { FirstName = "Tim", LastName = "Rich" };
            User user3 = new User { FirstName = "Tim", LastName = "Rodny" };
            var res = new List<int> { repo.Create(user1), repo.Create(user2), repo.Create(user3) };
            UserService service = new UserService(repo);
            service.Comunicator = new ServiceComunicator();
            SlaveService slave = new SlaveService(service);
            var result2 = slave.SearchForUsers(u => u.LastName == "Rich");
            Assert.AreEqual(3, res.Count);
            var enumerable = result2 as User[] ?? result2.ToArray();
            Assert.AreEqual(2, enumerable.Count());
            Assert.AreEqual("Rich", enumerable.First().LastName);
            Assert.AreEqual("Rich", enumerable.Last().LastName);
        }

        /// <summary>
        /// Loading data
        /// </summary>
        [TestMethod]
        public void Load_LoadData()
        {
            UserRepository repo = new UserRepository();
            UserService service = new UserService(repo);
            service.Comunicator = new ServiceComunicator();
            SlaveService slave = new SlaveService(service);
            slave.Load();
            Assert.IsNotNull(repo.Users);
        }

        /// <summary>
        /// Failed operations : add users. Slave hasn't permissions
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void SlaveRepo_FailedAddUser()
        {
            UserRepository repository = new UserRepository();
            UserService service = new UserService(repository);
            service.Comunicator = new ServiceComunicator();
            SlaveService slave = new SlaveService(service);
            slave.AddUser(new User() { FirstName = "Lisa", LastName = "Rich" });
        }

        /// <summary>
        /// Failed operations : delete users. Slave hasn't permissions
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void SlaveRepo_FailedDeleteUser()
        {
            UserRepository repository = new UserRepository();
            User user = new User { FirstName = "Lisa", LastName = "Rich" };
            var res = repository.Create(user);
            UserService service = new UserService(repository);
            service.Comunicator = new ServiceComunicator();
            SlaveService slave = new SlaveService(service);
            slave.Delete(user);
        }
    }
}
