using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.Decks.DeckImports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.BusinessLogic.Decks.DeckImports
{
    [TestClass]
    [DeploymentItem("./Resources/cards.json", "./Resources/")]
    public class SPCodeImporterTests : BaseTest
    {
        private Mock<ICardInstanceFactory> mockCardInstanceFactory = new Mock<ICardInstanceFactory>();
        //protected Mock<ICardSPCodeProvider> mockCardSPCodeProvider = new Mock<ICardSPCodeProvider>();

        [TestMethod()]
        public void ImportOneCardTest()
        {
            SPCodeImporter importer = CreateObject();

            importer.ExecuteImport("SPABakAAAA");

            Assert.AreEqual(1, importer.Cards.Count);
            Assert.AreEqual(1, importer.Cards[0].Quantity);
            Assert.AreEqual("Ald Velothi Assassin", importer.Cards[0].Card.Name);

        }

        [TestMethod()]
        public void ImportFullDeck()
        {
            SPCodeImporter importer = CreateObject();

            importer.ExecuteImport("SPAKhyrrlxcdmvgOddeehqqqACmmkvAMmGqamyoorkakbDiSqNaMgalI"); //batman with sword!

            Assert.AreEqual(24, importer.Cards.Count);
            
            Assert.AreEqual(3, importer.Cards.Where( c=> c.Card.Name == "Rapid Shot").First().Quantity);
            Assert.AreEqual(3, importer.Cards.Where( c=> c.Card.Name == "Ald Velothi Assassin").First().Quantity);
            Assert.AreEqual(3, importer.Cards.Where( c=> c.Card.Name == "Barrow Stalker").First().Quantity);
            Assert.AreEqual(1, importer.Cards.Where( c=> c.Card.Name == "Green Pact Ambusher").First().Quantity);
            Assert.AreEqual(3, importer.Cards.Where( c=> c.Card.Name == "Sword of the Inferno").First().Quantity);
            Assert.AreEqual(1, importer.Cards.Where( c=> c.Card.Name == "The Night Mother").First().Quantity);
            Assert.AreEqual(1, importer.Cards.Where( c=> c.Card.Name == "Galyn the Shelterer").First().Quantity);
            Assert.AreEqual(3, importer.Cards.Where( c=> c.Card.Name == "Indoril Mastermind").First().Quantity);
            Assert.AreEqual(3, importer.Cards.Where( c=> c.Card.Name == "Quicksilver Crossbow").First().Quantity);
            Assert.AreEqual(3, importer.Cards.Where( c=> c.Card.Name == "Skaven Pyromancer").First().Quantity);
            Assert.AreEqual(3, importer.Cards.Where( c=> c.Card.Name == "Tree Minder").First().Quantity);
            Assert.AreEqual(3, importer.Cards.Where( c=> c.Card.Name == "Archein Venomtongue").First().Quantity);
            Assert.AreEqual(3, importer.Cards.Where( c=> c.Card.Name == "Falkreath Defiler").First().Quantity);
            Assert.AreEqual(2, importer.Cards.Where( c=> c.Card.Name == "Merchant's Camel").First().Quantity);
            Assert.AreEqual(2, importer.Cards.Where( c=> c.Card.Name == "Preserver of the Root").First().Quantity);
            Assert.AreEqual(1, importer.Cards.Where( c=> c.Card.Name == "Cicero the Betrayer").First().Quantity);
            Assert.AreEqual(1, importer.Cards.Where( c=> c.Card.Name == "Gortwog gro-Nagorm").First().Quantity);
            Assert.AreEqual(1, importer.Cards.Where( c=> c.Card.Name == "Dark Harvester").First().Quantity);
            Assert.AreEqual(1, importer.Cards.Where( c=> c.Card.Name == "Pure-Blood Elder").First().Quantity);
            Assert.AreEqual(3, importer.Cards.Where( c=> c.Card.Name == "Unstoppable Rage").First().Quantity);
            Assert.AreEqual(1, importer.Cards.Where( c=> c.Card.Name == "Vigilant Giant").First().Quantity);
            Assert.AreEqual(1, importer.Cards.Where( c=> c.Card.Name == "Blood Magic Lord").First().Quantity);
            Assert.AreEqual(1, importer.Cards.Where( c=> c.Card.Name == "Night Talon Lord").First().Quantity);

        }


        private SPCodeImporter CreateObject()
        {
           return new SPCodeImporter(
               mockCardsDatabaseFactory.Object, 
               new CardInstanceFactory(),
               new CardSPCodeProvider(),
               new DeckCardsEditor(new CardInstanceFactory(), CardsDatabase));
        }
    }
}
