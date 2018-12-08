using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.BusinessLogic.Packs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.BusinessLogic.Cards;
using Moq;
using ESLTrackerTests.Builders;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.BusinessLogic.Packs.Tests
{
    [TestClass()]
    public class PacksChartsDataCalculatorTests
    {

        private Mock<ICardImage> mockCardImage;
        private Mock<ICardInstanceFactory> mockCardInstanceFactory;
        private Mock<IDataToSeriesTranslator> mockDataToSeriesTranslator;

        [TestInitialize]
        public void TestInitialize()
        {
            mockCardImage = new Mock<ICardImage>();
            mockCardInstanceFactory = new Mock<ICardInstanceFactory>();
            mockDataToSeriesTranslator = new Mock<IDataToSeriesTranslator>();
        }

        [TestMethod()]
        public void GetPieChartByClassDataTest()
        {
            Card green = new Card() { Attributes = ClassAttributesHelper.Classes[DeckClass.Agility]};
            Card blue = new Card() { Attributes = ClassAttributesHelper.Classes[DeckClass.Inteligence]};
            Card green_blue = new Card() { Attributes = ClassAttributesHelper.Classes[DeckClass.Assassin] };
            Card neutral = new Card() { Attributes = ClassAttributesHelper.Classes[DeckClass.Neutral] };

            var cardsData = new CardInstanceListBuilder()
                .WithCardAndQty(green, 4)
                .WithCardAndQty(blue, 2)
                .WithCardAndQty(green_blue, 1)
                .WithCardAndQty(neutral, 8)
                .Build();


            PacksChartsDataCalculator packsDataCalculator = CreatePacksChartsDataCalculatorObject();
            var actual = packsDataCalculator.GetPieChartByClassData(cardsData);

            mockDataToSeriesTranslator.Verify(dtst => dtst.CreateSeries(
                It.Is<string>(s => s.Contains(DeckAttribute.Agility.ToString())),
                It.IsAny<System.Windows.Media.Brush>(),
                4 + 0.5m), Times.Once);

            mockDataToSeriesTranslator.Verify(dtst => dtst.CreateSeries(
                It.Is<string>(s => s.Contains(DeckAttribute.Intelligence.ToString())),
                It.IsAny<System.Windows.Media.Brush>(),
                2 + 0.5m), Times.Once);

            mockDataToSeriesTranslator.Verify(dtst => dtst.CreateSeries(
                It.Is<string>(s => s.Contains(DeckAttribute.Neutral.ToString())),
                It.IsAny<System.Windows.Media.Brush>(),
                8), Times.Once);


        }

        [TestMethod()]
        public void GetPieChartByRarityDataTest()
        {
            Card common = new Card() { Rarity = CardRarity.Common };
            Card rare = new Card() { Rarity = CardRarity.Rare };
            Card legendary = new Card() { Rarity = CardRarity.Legendary };

            var cardsData = new CardInstanceListBuilder()
                .WithCardAndQty(common, 10)
                .WithCardAndQty(rare, 3)
                .WithCardAndQty(legendary, 1)
                .Build();


            PacksChartsDataCalculator packsDataCalculator = CreatePacksChartsDataCalculatorObject();
            var actual = packsDataCalculator.GetPieChartByRarityData(cardsData);

            mockDataToSeriesTranslator.Verify(dtst => dtst.CreateSeries(
                It.Is<string>(s => s.Contains(CardRarity.Common.ToString())),
                It.IsAny<System.Windows.Media.Brush>(),
                10), Times.Once);

            mockDataToSeriesTranslator.Verify(dtst => dtst.CreateSeries(
                  It.Is<string>(s => s.Contains(CardRarity.Rare.ToString())),
                  It.IsAny<System.Windows.Media.Brush>(),
                  3), Times.Once);

            mockDataToSeriesTranslator.Verify(dtst => dtst.CreateSeries(
                  It.Is<string>(s => s.Contains(CardRarity.Legendary.ToString())),
                  It.IsAny<System.Windows.Media.Brush>(),
                  1), Times.Once);
        }

        [TestMethod()]
        public void GetPieChartPremiumByRarityDataTest()
        {
            Card common = new Card() { Rarity = CardRarity.Common };
            Card rare = new Card() { Rarity = CardRarity.Rare };
            Card legendary = new Card() { Rarity = CardRarity.Legendary };

            var cardsData = new CardInstanceListBuilder()
                .WithCardAndQty(common, 10)
                .WithCardAndQty(common, 2, true)
                .WithCardAndQty(rare, 3)
                .WithCardAndQty(rare, 1,true)
                .WithCardAndQty(legendary, 1)
                .Build();


            PacksChartsDataCalculator packsDataCalculator = CreatePacksChartsDataCalculatorObject();
            var actual = packsDataCalculator.GetPieChartPremiumByRarityData(cardsData);

            mockDataToSeriesTranslator.Verify(dtst => dtst.CreateSeries(
                It.Is<string>(s => s.Contains(CardRarity.Common.ToString())),
                It.IsAny<System.Windows.Media.Brush>(),
                2), Times.Once);

            mockDataToSeriesTranslator.Verify(dtst => dtst.CreateSeries(
                  It.Is<string>(s => s.Contains(CardRarity.Rare.ToString())),
                  It.IsAny<System.Windows.Media.Brush>(),
                  1), Times.Once);

            mockDataToSeriesTranslator.Verify(dtst => dtst.CreateSeries(
                  It.Is<string>(s => s.Contains(CardRarity.Legendary.ToString())),
                  It.IsAny<System.Windows.Media.Brush>(),
                  It.IsAny<int>()), Times.Never);
        }

        [TestMethod()]
        public void GetTopXCardsDataTest()
        {
            Card card1 = new Card() { Rarity = CardRarity.Common };
            Card card2 = new Card() { Rarity = CardRarity.Rare };
            Card card3 = new Card() { Rarity = CardRarity.Legendary };

            var cardsData = new CardInstanceListBuilder()
                .WithCardAndQty(card1, 1)
                .WithCardAndQty(card2, 2, true)
                .WithCardAndQty(card3, 3)
                .Build();

            mockCardInstanceFactory.Setup( cif => cif.CreateFromCard(It.IsAny<Card>(), It.IsAny<int>()))
                .Returns((Card c,int  q) => { return new CardInstance() { Card = c, Quantity = q }; });


            PacksChartsDataCalculator packsDataCalculator = CreatePacksChartsDataCalculatorObject();
            var actual = packsDataCalculator.GetTopXCardsData(cardsData, 2);

            Assert.AreEqual(card3, actual.First().Card);
            Assert.AreEqual(3, actual.First().Quantity);
            Assert.AreEqual(card2, actual.Skip(1).First().Card);
            Assert.AreEqual(2, actual.Skip(1).First().Quantity);
        }

        private PacksChartsDataCalculator CreatePacksChartsDataCalculatorObject()
        {
            return new PacksChartsDataCalculator(
                mockCardImage.Object,
                mockCardInstanceFactory.Object,
                mockDataToSeriesTranslator.Object);
        }

    }
}