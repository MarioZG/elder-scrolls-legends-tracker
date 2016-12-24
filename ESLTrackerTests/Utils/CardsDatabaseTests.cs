using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.Utils.Tests
{
    [TestClass()]
    public class CardsDatabaseTests
    {
        [TestMethod()]
        public void FindCardByNameTest001_UnknownCard()
        {
            Card expected = Card.Unknown;
            Card actual = new CardsDatabase().FindCardByName("some randoe strng");

            Assert.AreEqual(expected, actual);
        }
    }
}