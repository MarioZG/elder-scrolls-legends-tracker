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
    public class GameTests
    {
        [TestMethod()]
        public void GameTest001_CheckIfDatePopulated()
        {
            Game g = new Game();

            Assert.IsTrue(g.Date.Year > 1999);
        }
    }
}