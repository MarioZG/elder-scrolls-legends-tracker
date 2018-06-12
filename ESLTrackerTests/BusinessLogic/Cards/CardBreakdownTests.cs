using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.BusinessLogic.Cards
{
    [TestClass]
    [DeploymentItem("./Resources/cards.json", "./Resources/")]
    public class CardBreakdownTests : BaseTest
    {
        private Random random;
        private IEnumerable<CardInstance> cards;
        private object sw;

        [TestInitialize]
        public void ClassInit(TestContext context)
        {
            random = new Random((int)DateTime.Now.Ticks);
            cards = CardsDatabase.Default.Cards.Select(c => new CardInstance() { Card = c, Quantity = random.Next(5) }).ToList();
        }

        [TestMethod]
        public void Performance_SelectMany_vs_SelectForAllInEnum()
        {
            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            var manaBreakdown =
                           ((CardKeyword[])Enum.GetValues(typeof(CardKeyword)))
                           .ToDictionary(
                               (i) => i,
                               (i) => cards.Where(ci => ci.Card.Keywords.Contains(i)).Sum(ci => ci.Quantity))
                           .Where(i => i.Value > 0);


            sw1.Stop();
            TestContext.WriteLine($"to dictionary for all values in enum {sw}");

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            var manaBreakdown2 = cards
                        .SelectMany(ci => Enumerable.Repeat(ci.Card.Keywords, ci.Quantity))
                        .SelectMany(ci => ci)
                        .GroupBy(
                        keyword => keyword,
                        (keyword, count) => new KeyValuePair<CardKeyword, int>(keyword, count.Count()))
                       ;

            sw2.Stop();
            TestContext.WriteLine($"to dictionary after repliction: elemet x qty {sw}");

            Assert.IsTrue(sw2.ElapsedTicks < sw1.ElapsedTicks);
        }
    }
}
