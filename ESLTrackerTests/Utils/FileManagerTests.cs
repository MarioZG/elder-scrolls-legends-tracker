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

namespace ESLTracker.Utils.Tests
{
    [TestClass()]
    public class FileManagerTests : BaseTest
    {
        [TestMethod()]
        public void ManageBackupsTest001_OneBackup()
        {
            Mock<IPathWrapper> pathWrapper = new Mock<IPathWrapper>();
            Mock<IDirectoryWrapper> directoryWrapper = new Mock<IDirectoryWrapper>();
            Mock<IFileWrapper> fileWrapper = new Mock<IFileWrapper>();
            directoryWrapper
                .Setup(dw => dw.EnumerateFiles(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new string[] { "data123.xml", "data.xml" });

            string path = "./data.xml";

            FileManager.ManageBackups(path, pathWrapper.Object, directoryWrapper.Object, fileWrapper.Object);

            fileWrapper.Verify(fw => fw.Delete(It.IsAny<string>()), Times.Never);

        }

        [TestMethod()]
        public void ManageBackupsTest001_7BackupFiles()
        {
            Mock<IPathWrapper> pathWrapper = new Mock<IPathWrapper>();
            Mock<IDirectoryWrapper> directoryWrapper = new Mock<IDirectoryWrapper>();
            Mock<IFileWrapper> fileWrapper = new Mock<IFileWrapper>();
            directoryWrapper
                .Setup(dw => dw.EnumerateFiles(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new string[] { "data.xml",
                    "data1.xml",
                    "data2.xml",
                    "data3.xml",
                    "data4.xml",
                    "data5.xml",
                    "data6.xml",
                    "data7.xml"
                });

            string path = "./data.xml";

            FileManager.ManageBackups(path, pathWrapper.Object, directoryWrapper.Object, fileWrapper.Object);

            fileWrapper.Verify(fw => fw.Delete(It.IsAny<string>()), Times.Never);

        }

        [TestMethod()]
        public void ManageBackupsTest001_8BackupFiles()
        {
            Mock<IPathWrapper> pathWrapper = new Mock<IPathWrapper>();
            Mock<IDirectoryWrapper> directoryWrapper = new Mock<IDirectoryWrapper>();
            Mock<IFileWrapper> fileWrapper = new Mock<IFileWrapper>();
            directoryWrapper
                .Setup(dw => dw.EnumerateFiles(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new string[] { "data.xml",
                    "data8.xml",
                    "data7.xml",
                    "data6.xml",
                    "data5.xml",
                    "data4.xml",
                    "data3.xml",
                    "data2.xml",
                    "data1.xml"
                });

            string path = "./data.xml";

            FileManager.ManageBackups(path, pathWrapper.Object, directoryWrapper.Object, fileWrapper.Object);

            fileWrapper.Verify(fw => fw.Delete(It.IsAny<string>()), Times.Once);
            fileWrapper.Verify(fw => fw.Delete(It.Is<string>(s => s == "data8.xml")), Times.Once);

        }

        [TestMethod()]
        public void SaveDatabaseTest001_NonExitingPath()
        {
            FileManager.SaveDatabase<Tracker>(TestContext.TestDeploymentDir + "./somerandomfolder/ss.xml", new Tracker());
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
    }
}