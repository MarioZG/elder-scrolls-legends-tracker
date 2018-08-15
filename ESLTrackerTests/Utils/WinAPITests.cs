using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Moq;
using ESLTracker.Utils.DiagnosticsWrappers;

namespace ESLTrackerTests.Utils
{
    [TestClass()]
    public class WinAPITests: BaseTest
    {
        [TestMethod()]
        public void GetEslProcessTest_exceptionThrown()
        {
            Mock<IProcessWrapper> procWrap = new Mock<IProcessWrapper>();
            procWrap.Setup(pw => pw.GetProcessesByName(It.IsAny<string>())).Throws(new System.ComponentModel.Win32Exception(2147467259, "Access is denied"));

            WinAPI winApi = new WinAPI(mockLogger.Object, procWrap.Object);
            Process p = winApi.GetEslProcess();

            Assert.IsNull(p);
        }
    }
}