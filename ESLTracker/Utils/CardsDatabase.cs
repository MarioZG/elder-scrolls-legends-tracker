using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.Utils
{
    public class CardsDatabase
    {
        public IEnumerable<string> CardsNames {
            get
            {
                return new List<string>() { "card 1", "better card" };
            }
        }

        public IEnumerable<Card> Cards
        {
            get
            {
                return new List<Card>() {
                    new Card() {Name = "card 1", Rarity = DataModel.Enums.CardRarity.Common },
                    new Card() {Name = "some other  1", Rarity = DataModel.Enums.CardRarity.Legendary }
                };
            }
        }

        internal void PopulateCollection(ObservableCollection<string> cardNames, ObservableCollection<Card> cards)
        {
            cards.Clear();
            foreach (string name in cardNames)
            {
                Card card = FindCardByName(name);
                cards.Add(card);
            }
        }

        internal Card FindCardByName(string name)
        {
            return Cards.Where(c => c.Name.ToLower().Trim() == name.ToLower().Trim()).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
        }
    }
}
