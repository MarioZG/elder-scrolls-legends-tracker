using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.DataModel.Tests
{
    [TestClass]
    [DeploymentItem("./Resources/cards.json", "./Resources/")]
    public class PackTests
    {
        [TestMethod]
        public void PackTest001_SerialiseAndDeserialise()
        {
            Pack pack = new Pack();
            pack.Cards.Add(new CardInstance());
            pack.Cards.Add(new CardInstance());
            pack.Cards.Add(new CardInstance());


            Assert.AreEqual(3, pack.Cards.Count);

            string xml = Utils.SerializationHelper.SerializeXML<Pack>(pack);

            Pack pack2 = Utils.SerializationHelper.DeserializeXML<Pack>(xml);

            Assert.AreEqual(3, pack2.Cards.Count);

        }
    }
}