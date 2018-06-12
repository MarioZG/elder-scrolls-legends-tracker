using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Cards
{
    internal class SoulGemCalculator
    {
        internal int CalculateCardsSellValue(IEnumerable<CardInstance> cards)
        {
            if (cards == null)
            {
                return 0;
            }
            var notNullCards = cards.Where(c => c.Card != null && c.Card != Card.Unknown);
            return
                notNullCards.Where(c => c.Card.Rarity == CardRarity.Common && !c.IsPremium).Sum(ci => ci.Quantity) * 5
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Common && c.IsPremium).Sum(ci => ci.Quantity) * 50
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Rare && !c.IsPremium).Sum(ci => ci.Quantity) * 20
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Rare && c.IsPremium).Sum(ci => ci.Quantity) * 100
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Epic && !c.IsPremium).Sum(ci => ci.Quantity) * 100
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Epic && c.IsPremium).Sum(ci => ci.Quantity) * 400
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Legendary && !c.IsPremium).Sum(ci => ci.Quantity) * 400
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Legendary && c.IsPremium).Sum(ci => ci.Quantity) * 1200;
        }

        internal int CalculateCardsPurchaseValue(IEnumerable<CardInstance> cards)
        {
            if (cards == null)
            {
                return 0;
            }
            var notNullCards = cards.Where(c => c.Card != null && c.Card != Card.Unknown);
            return
                notNullCards.Where(c => c.Card.Rarity == CardRarity.Common && !c.IsPremium).Sum(ci => ci.Quantity) * 50
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Common && c.IsPremium).Sum(ci => ci.Quantity) * 200
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Rare && !c.IsPremium).Sum(ci => ci.Quantity) * 100
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Rare && c.IsPremium).Sum(ci => ci.Quantity) * 400
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Epic && !c.IsPremium).Sum(ci => ci.Quantity) * 400
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Epic && c.IsPremium).Sum(ci => ci.Quantity) * 1600
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Legendary && !c.IsPremium).Sum(ci => ci.Quantity) * 1200
                + notNullCards.Where(c => c.Card.Rarity == CardRarity.Legendary && c.IsPremium).Sum(ci => ci.Quantity) * 4800;
        }
    }
}
