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

namespace ESLTracker.Utils.Tests
{
    [TestClass()]
    public class WinAPITests
    {
        [TestMethod()]
        public void GetEslProcessTest_exceptionThrown()
        {
            WinAPI winApi = new WinAPI();
            Mock<IProcessWrapper> procWrap = new Mock<IProcessWrapper>();

            procWrap.Setup(pw => pw.GetProcesses()).Throws(new System.ComponentModel.Win32Exception(2147467259, "Access is denied"));

            Process p = winApi.GetEslProcess();

            Assert.IsNull(p);
        }
    }
}