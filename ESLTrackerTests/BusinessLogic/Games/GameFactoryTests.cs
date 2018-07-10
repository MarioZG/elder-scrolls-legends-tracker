using System;
using ESLTracker.BusinessLogic.Games;
using ESLTracker.DataModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ESLTrackerTests.BusinessLogic.Games
{
    [TestClass]
    public class GameFactoryTests : BaseTest
    {
        [TestMethod]
        public void CreateGame001_EnsureDateIsSet()
        {
            DateTime expectedDate = new DateTime(2018, 7, 6, 12, 34, 56);

            mockDatetimeProvider.SetupGet(dtp => dtp.DateTimeNow).Returns(expectedDate);

            GameFactory gamefactory = CreateGameFactory();

            Game game = gamefactory.CreateGame();

            Assert.AreEqual(expectedDate, game.Date);
        }

        private GameFactory CreateGameFactory()
        {
            return new GameFactory(mockDatetimeProvider.Object);
        }
    }
}
