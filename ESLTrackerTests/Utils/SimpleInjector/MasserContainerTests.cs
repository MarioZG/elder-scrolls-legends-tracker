using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.Utils.SimpleInjector
{
    [TestClass]
    public class MasserContainerTests : BaseTest
    {

        [TestMethod]
        [Ignore]
        public void VerifyDiConfiguration()
        {
            try
            {
                var container = new MasserContainer();
              //  container.Verify();
                    TestContext.WriteLine(MasserContainer.Container.GetRegistration(typeof(MainWindowViewModel)).VisualizeObjectGraph());

            }
            catch (InvalidOperationException ex)
            {
            //    if (ex.InnerException.GetType().FullName == "SimpleInjector.Internals.CyclicDependencyException")
                {
                    //    SimpleInjector.ActivationException ae = (SimpleInjector.ActivationException)ex.InnerException;
                  //  SimpleInjector.InstanceProducer failedProducer = ex.InnerException.GetType().GetProperty("OriginatingProducer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ex.InnerException) as SimpleInjector.InstanceProducer;
                //    var failedDependecies = MasserContainer.Container.GetRegistration(failedProducer.Registration.ImplementationType).VisualizeObjectGraph();
                    //    HandleUnhandledException(new Exception("Dependecies issue: " + failedDependecies), "Startup");
                    //    throw new Exception("Dependecies issue: " + failedDependecies);
                //    TestContext.WriteLine(failedDependecies);
                }
                
            }
        }

    }
}
