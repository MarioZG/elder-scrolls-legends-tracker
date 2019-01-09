using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ESLTracker.Utils.IOWrappers;
using TESLTracker.DataModel;
using ESLTrackerTests;
using System.Xml;
using ESLTracker.Properties;
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
        public override void TestInitialize()
        {
            base.TestInitialize();

            pathManager = new PathManager(mockSettings.Object);
            pathWrapper.Setup(pw => pw.ChangeExtension(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string p, string e) => { return p; });
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

     

    

        private FileSaver CreateFileManager()
        {
            return new FileSaver(
                            pathManager,
                            pathWrapper.Object,
                            directoryWrapper.Object,
                            fileWrapper.Object);
        }
    }
}