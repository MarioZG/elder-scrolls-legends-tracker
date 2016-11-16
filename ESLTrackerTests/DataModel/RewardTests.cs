using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.DataModel.Tests
{
    [TestClass()]
    public class RewardTests
    {
        [TestMethod()]
        public void RewardTest001_CheckIfDatePopulated()
        {
            Reward r = new Reward();

            Assert.IsTrue(r.Date.Year > 1999);
        }
    }
}