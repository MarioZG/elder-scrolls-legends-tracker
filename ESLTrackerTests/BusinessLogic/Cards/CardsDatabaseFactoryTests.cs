using System;
using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.Utils;
using ESLTracker.Utils.IOWrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ESLTrackerTests.BusinessLogic.Cards
{
    [TestClass]
    public class CardsDatabaseFactoryTests : BaseTest
    {

        Mock<IFileWrapper> mockFileWrapper = new Mock<IFileWrapper>();
        Mock<IMessenger> mockMessenger = new Mock<IMessenger>();
        PathManager pathManager;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            pathManager = new PathManager(mockSettings.Object);
        }

        [TestMethod()]
        public void UpdateCardsDBTest001()
        {
            string newFileContent = "{some json}";

            CardsDatabaseFactory cardsDatabaseFactory = CreateCardsDatabaseFactory();

            cardsDatabaseFactory.UpdateCardsDB(newFileContent, new Version(3, 4));

            mockFileWrapper.Verify(m => m.Move(".\\Resources\\cards.json", ".\\Resources\\cards_3.4.json"), Times.Once);
            mockFileWrapper.Verify(m => m.WriteAllText(".\\Resources\\cards.json", newFileContent), Times.Once);

            mockFileWrapper.Verify( fw => fw.Exists(pathManager.CardsDatabasePath), Times.Once);

        }

        [TestMethod()]
        public void UpdateCardsDBTest001_DeleteAlreayExistingBackup()
        {
            string newFileContent = "{some json}";

            mockFileWrapper.Setup(fw => fw.Exists(It.IsAny<string>())).Returns(true);


            CardsDatabaseFactory cardsDatabaseFactory = CreateCardsDatabaseFactory();

            cardsDatabaseFactory.UpdateCardsDB(newFileContent, new Version(3, 4));

            mockFileWrapper.Verify(m => m.Move(".\\Resources\\cards.json", ".\\Resources\\cards_3.4.json"), Times.Once);
            mockFileWrapper.Verify(m => m.WriteAllText(".\\Resources\\cards.json", newFileContent), Times.Once);

            mockFileWrapper.Verify(fw => fw.Exists(".\\Resources\\cards_3.4.json"), Times.Once);
            mockFileWrapper.Verify(fw => fw.Delete(".\\Resources\\cards_3.4.json"), Times.Once);

        }

        private CardsDatabaseFactory CreateCardsDatabaseFactory()
        {
            return new CardsDatabaseFactory(
                mockFileWrapper.Object,
                pathManager,
                mockMessenger.Object);
        }
    }
}
