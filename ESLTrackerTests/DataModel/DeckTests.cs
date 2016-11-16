using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace ESLTracker.DataModel.Tests
{
    [TestClass()]
    public class DeckTests
    {


        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod()]
        public void DeckTest001_GuidPreservedDurinDeserialisation()
        {
            Deck d = new Deck();
            Guid original = d.DeckId;

            StringBuilder tempString = new StringBuilder();
            using (TextWriter writer = new StringWriter(tempString))
            {
                var xml = new XmlSerializer(typeof(Deck));
                xml.Serialize(writer, d);
            }

            Deck deckDeserialise;

            using (TextReader reader = new StringReader(tempString.ToString()))
            {
                var xml = new XmlSerializer(typeof(Deck));
                deckDeserialise = (Deck)xml.Deserialize(reader);
            }

            Assert.AreEqual(original, deckDeserialise.DeckId);
        }

        [TestMethod()]
        public void DeckTest002_CheckDateTimeAdded()
        {
            Deck d = new Deck();

            Assert.IsTrue(d.CreatedDate.Year > 1999);

        }
    }
}