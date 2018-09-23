using ESLTracker;
using ESLTracker.BusinessLogic.Cards;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.BusinessLogic.Cards
{
    [TestClass]
    public class CardImageTests : BaseTest
    {

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            string s = System.IO.Packaging.PackUriHelper.UriSchemePack;
            System.Windows.Application.ResourceAssembly = Assembly.GetAssembly(typeof(App));
        }

        [TestInitialize]
        override public void TestInitialize()
        {
            base.TestInitialize();

            CardImage.CardMiniatureCache.Clear();
            CardImage.RarityCache.Clear();
        }

        [TestMethod]
        public void GetCardMiniature001_PerfTests()
        {

            CardImage ci = CreateCardImageObject();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (Card card in CardsDatabase.Cards)
            {
                var cardImage = ci.GetCardMiniature(card);
            }
            sw.Stop();
            TimeSpan firstPass = sw.Elapsed;

            TestContext.WriteLine($"GetCardMiniature first pass {sw.Elapsed}");

            sw.Restart();
            foreach (Card card in CardsDatabase.Cards)
            {
                var cardImage = ci.GetCardMiniature(card);
            }
            sw.Stop();
            TimeSpan secondPass = sw.Elapsed;

            TestContext.WriteLine($"GetCardMiniature second pass {sw.Elapsed}");

            Assert.AreEqual(CardsDatabase.Cards.Count(), CardImage.CardMiniatureCache.Count);
            Assert.IsTrue(firstPass.TotalMilliseconds > secondPass.TotalMilliseconds * 5, $"first oass={firstPass}; sec pass={secondPass}"); //first pass must be waaay faster
        }

        [TestMethod]
        public void CardsDatabaseTest_EnsureAllCardsHaveImages()
        {
            Resources resService = new Resources();
            CardImage cardImage = CreateCardImageObject();

            foreach (Card card in CardsDatabase.Cards)
            {
                Assert.IsNotNull(cardImage.GetCardMiniature(card), card.Name);

                Uri imageUri = new Uri(card.ImageName, UriKind.RelativeOrAbsolute);
                Assert.IsTrue(resService.ResourceExists(imageUri), card.Name);
            }

        }

        private CardImage CreateCardImageObject()
        {
            return new CardImage(mockLogger.Object, new Resources());
        }
    }
}
