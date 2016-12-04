using System;
using System.Collections.Generic;
using DAL.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest
{
    /// <summary>
    /// ID tests
    /// </summary>
    [TestClass]
    public class IdIteratorTest
    {
      /// <summary>
      /// Exception if previous id was -1
      /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetNextId_PrevIdMinusOne_ThrowAnException()
        {
            List<int> result = new List<int>();
            result.Add(IdIterator.GetNextId(-1));
        }
    }
}
