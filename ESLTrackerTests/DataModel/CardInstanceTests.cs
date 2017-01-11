using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTrackerTests;
using System.Reflection;

namespace ESLTracker.DataModel.Tests
{
    [TestClass]
    public class CardInstanceTests : BaseTest
    {
        [TestMethod]
        public void CloneTest001()
        {
            CardInstance ci = new CardInstance();
            PopulateObject(ci, StartProp);

            CardInstance clone = ci.Clone() as CardInstance;

            foreach (PropertyInfo p in typeof(CardInstance).GetProperties())
            {
                if (p.CanWrite)
                {
                    TestContext.WriteLine("Checking prop:{0}.{1};{2}", p.Name, p.GetValue(ci), p.GetValue(clone));

                    if ((p.Name == nameof(CardInstance.Card))//skip do not clone as this will never change anyway! (read from DB)
                        || (p.Name == nameof(CardInstance.BorderBrush))) //brush wont be modified
                    {
                        
                        continue;
                    }
                    Assert.IsFalse(Object.ReferenceEquals(p.GetValue(ci), p.GetValue(clone))); 
                }
            }



        }
    }
}