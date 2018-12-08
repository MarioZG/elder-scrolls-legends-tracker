using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.BusinessLogic.Packs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ESLTracker.Utils;
using ESLTrackerTests.Builders;
using ESLTracker.DataModel;
using ESLTracker.Utils.IOWrappers;

namespace ESLTracker.BusinessLogic.Packs.Tests
{
    [TestClass()]
    public class PacksDataCSVExporterTests
    {
        private Mock<IWinDialogs> mockWinDialogs = new Mock<IWinDialogs>();
        private Mock<IFileWrapper> mockFileWrapper = new Mock<IFileWrapper>();

        
        [TestMethod()]
        public async Task ExportToCSVFileTest_SaveDialogCancelled()
        {
            string dialogReturnValue = null;
            mockWinDialogs
                .Setup(iwd => iwd.SaveFileDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(dialogReturnValue);

            var packs = new List<Pack> {
                new PackBuilder().Build()
            };

            PacksDataCSVExporter exporter = CreateExporterObject();

            var actual = await exporter.ExportToCSVFile(packs);

            Assert.IsTrue(String.IsNullOrEmpty(actual));

        }

        [TestMethod()]
        public async Task ExportToCSVFileTest_ExportToFile()
        {
            string dialogReturnValue = "selectedFile";
            mockWinDialogs
                .Setup(iwd => iwd.SaveFileDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(dialogReturnValue);

            var packs = new List<Pack> {
                new PackBuilder().Build()
            };

            PacksDataCSVExporter exporter = CreateExporterObject();

            var actual = await exporter.ExportToCSVFile(packs);

            Assert.IsFalse(String.IsNullOrEmpty(actual));
            mockFileWrapper.Verify(mfw => mfw.WriteAllText(dialogReturnValue, It.IsAny<string>()));
        }

        private PacksDataCSVExporter CreateExporterObject()
        {
            return  new PacksDataCSVExporter(
                mockWinDialogs.Object, 
                mockFileWrapper.Object);
        }
    }
}