using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ESLTracker.Utils.IOWrappers;
using ESLTracker.DataModel;
using ESLTrackerTests;
using System.Xml;
using ESLTracker.Properties;
using ESLTracker.Services;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.General;
using ESLTracker.BusinessLogic.Cards;

namespace ESLTracker.Utils.Tests
{
    [TestClass()]
    public class FileManagerTests : BaseTest
    {
        Mock<IPathWrapper> pathWrapper = new Mock<IPathWrapper>();
        PathManager pathManager;
        Mock<IDirectoryWrapper> directoryWrapper = new Mock<IDirectoryWrapper>();
        Mock<IFileWrapper> fileWrapper = new Mock<IFileWrapper>();
        Mock<ICardsDatabaseFactory> cardsDatabaseFactory = new Mock<ICardsDatabaseFactory>();
        Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();

        [TestInitialize]
        public void TestInitialize()
        {
            pathManager = new PathManager(mockSettings.Object);
        }

        [TestMethod()]
        public void ManageBackupsTest001_OneBackup()
        {
            Mock<ISettings> settings = new Mock<ISettings>();
            settings.Setup(s => s.DataPath).Returns("c:\\some path\\f1\\f2\\");


            directoryWrapper
                .Setup(dw => dw.EnumerateFiles(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new string[] { "data123.xml", "data.xml" });

            string path = "./data.xml";
            CreateFileManager().ManageBackups(path);

            fileWrapper.Verify(fw => fw.Delete(It.IsAny<string>()), Times.Never);

        }

        [TestMethod()]
        public void ManageBackupsTest002_7BackupFiles()
        {
            mockSettings.Setup(s => s.DataPath).Returns("c:\\some path\\f1\\f2\\");

            directoryWrapper
                .Setup(dw => dw.EnumerateFiles(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new string[] { mockSettings.Object.DataPath + "data.xml",
                    mockSettings.Object.DataPath + "data1.xml",
                    mockSettings.Object.DataPath + "data2.xml",
                    mockSettings.Object.DataPath + "data3.xml",
                    mockSettings.Object.DataPath + "data4.xml",
                    mockSettings.Object.DataPath + "data5.xml",
                    mockSettings.Object.DataPath + "data6.xml",
                    mockSettings.Object.DataPath + "data7.xml"
                });

            string path = "./data.xml";

            CreateFileManager().ManageBackups(path);

            fileWrapper.Verify(fw => fw.Delete(It.IsAny<string>()), Times.Never);

        }

        [TestMethod()]
        public void ManageBackupsTest003_8BackupFiles()
        {
            mockSettings.Setup(s => s.DataPath).Returns("c:\\some path\\f1\\f2\\");

            directoryWrapper
                .Setup(dw => dw.EnumerateFiles(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new string[] { mockSettings.Object.DataPath + "data.xml",
                    mockSettings.Object.DataPath + "data4.xml",
                    mockSettings.Object.DataPath + "data3.xml",
                    mockSettings.Object.DataPath + "data6.xml",
                    mockSettings.Object.DataPath + "data5.xml",
                    mockSettings.Object.DataPath + "data8.xml",
                    mockSettings.Object.DataPath + "data2.xml",
                    mockSettings.Object.DataPath + "data1.xml",
                    mockSettings.Object.DataPath + "data7.xml"
                });

            string path = "./data.xml";

            CreateFileManager().ManageBackups(path);

            fileWrapper.Verify(fw => fw.Delete(It.IsAny<string>()), Times.Once);
            //ensure oldest backup has been deleted
            fileWrapper.Verify(fw => fw.Delete(It.Is<string>(s => s == mockSettings.Object.DataPath + "data1.xml")), Times.Once);

        }

        [TestMethod()]
        public void SaveDatabaseTest001_NonExitingPath()
        {
            CreateFileManager().SaveDatabase<Tracker>(TestContext.TestDeploymentDir + "./somerandomfolder/ss.xml", new Tracker());
        }

        [TestMethod()]
        public void ParseCurrentFileVersionTest001_AllOK()
        {
            var doc = new XmlDocument();
            doc.LoadXml(
                @" <Version>
                    <Build>2</Build>
                    <Major>1</Major>
                    <Minor>3</Minor>
                    <Revision>4</Revision>
                  </Version>");
            var versionNode = doc.DocumentElement;

            SerializableVersion expected = new SerializableVersion(new Version(1, 3, 2, 4));

            SerializableVersion result = FileManager.ParseCurrentFileVersion(versionNode);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ParseCurrentFileVersionTest002_IncorrectElementNameXML()
        {
            var doc = new XmlDocument();
            doc.LoadXml(
                @" <Version>
                    <Bld>2</Bld>
                    <Major>1</Major>
                    <Minor>3</Minor>
                    <Resion>4</Resion>
                  </Version>");
            var versionNode = doc.DocumentElement;

            SerializableVersion expected = null;

            SerializableVersion result = FileManager.ParseCurrentFileVersion(versionNode); ;

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ParseCurrentFileVersionTest003_NotIntInValue()
        {
            var doc = new XmlDocument();
            doc.LoadXml(
                @" <Version>
                    <Build>test non int!</Build>
                    <Major>1</Major>
                    <Minor>3</Minor>
                    <Revision>4</Revision>
                  </Version>");
            var versionNode = doc.DocumentElement;

            SerializableVersion expected = null;

            SerializableVersion result = FileManager.ParseCurrentFileVersion(versionNode);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ParseCurrentFileVersionTest004_NullPassed()
        {

            SerializableVersion expected = null;

            SerializableVersion result = FileManager.ParseCurrentFileVersion(null);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void UpdateCardsDBTest001()
        {
            string newFileContent = "{some json}";

            Mock<ICardsDatabase> cdb = new Mock<ICardsDatabase>();
            cdb.Setup( cd=> cd.Version).Returns(new Version(3, 4));

            cardsDatabaseFactory.Setup(cdf => cdf.GetCardsDatabase()).Returns(cdb.Object);

            FileManager fm = CreateFileManager();

            fm.UpdateCardsDB(newFileContent);

            fileWrapper.Verify(m => m.Move(".\\Resources\\cards.json", ".\\Resources\\cards_3.4.json"), Times.Once);
            fileWrapper.Verify(m => m.WriteAllText(".\\Resources\\cards.json", newFileContent), Times.Once);

            cardsDatabaseFactory.Verify(m => m.RealoadDB(), Times.Once);

        }

        [TestMethod()]
        public void UpdateCardsDBTest002_EnsureNewObjectIsReturned()
        {
            string newFileContent = "{some json}";

            Mock<ICardsDatabase> cardsDatabase = new Mock<ICardsDatabase>();

            cardsDatabaseFactory.Setup(cdf => cdf.GetCardsDatabase()).Returns(cardsDatabase.Object);

            cardsDatabaseFactory.Setup(cd => cd.RealoadDB()).Returns(cardsDatabase.Object);

            FileManager fm = CreateFileManager();

            ICardsDatabase actual = fm.UpdateCardsDB(newFileContent);

            Assert.AreSame(cardsDatabase.Object, actual);

        }

        private FileManager CreateFileManager()
        {
            return new FileManager(mockSettings.Object,
                            pathManager,
                            pathWrapper.Object,
                            directoryWrapper.Object,
                            fileWrapper.Object,
                            cardsDatabaseFactory.Object,
                            trackerFactory.Object);
        }
    }
}