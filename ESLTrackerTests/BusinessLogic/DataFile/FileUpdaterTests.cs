using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ESLTrackerTests.BusinessLogic.DataFile
{
    [TestClass]
    public class FileUpdaterTests : BaseTest
    {
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

            FileUpdater fileUpdater = CreateFileUpdater();

            SerializableVersion result = fileUpdater.ParseCurrentFileVersion(versionNode);

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


            FileUpdater fileUpdater = CreateFileUpdater();

            SerializableVersion result = fileUpdater.ParseCurrentFileVersion(versionNode);

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

            FileUpdater fileUpdater = CreateFileUpdater();

            SerializableVersion result = fileUpdater.ParseCurrentFileVersion(versionNode);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void ParseCurrentFileVersionTest004_NullPassed()
        {

            SerializableVersion expected = null;

            FileUpdater fileUpdater = CreateFileUpdater();

            SerializableVersion result = fileUpdater.ParseCurrentFileVersion(null);

            Assert.AreEqual(expected, result);
        }


        private FileUpdater CreateFileUpdater()
        {
            return new FileUpdater(new PathManager(mockSettings.Object));
        }
    }
}
