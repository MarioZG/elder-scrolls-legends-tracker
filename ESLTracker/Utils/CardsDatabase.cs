using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.Utils
{
    public class CardsDatabase : ICardsDatabase
    {

        static CardsDatabase _instance;
        [Obsolete("Use IFactory to obtain instance")]
        public static CardsDatabase Default
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CardsDatabase();
                }
                return _instance;
            }
        }

        private CardsDatabase()
        {

        }

        public IEnumerable<string> CardsNames {
            get
            {
                return Cards.Select(c => c.Name);
            }
        }

        IEnumerable<Card> cards;
        public IEnumerable<Card> Cards
        {
            get
            {
                if (cards == null)
                {
                    LoadCardsDatabase();
                }
                return cards;
            }
        }

        public void LoadCardsDatabase()
        {
            cards = SerializationHelper.DeserializeJson<IEnumerable<Card>>(System.IO.File.ReadAllText("./Resources/cards.json"));
        }

        public void PopulateCollection(
            ObservableCollection<string> cardNames, 
            ObservableCollection<CardInstance> targetCollection)
        {
            targetCollection.Clear();
            foreach (string name in cardNames)
            {
                Card card = FindCardByName(name);
                targetCollection.Add(new CardInstance(card));
            }
        }

        internal Card FindCardByName(string name)
        {
            return Cards.Where(c => c.Name.ToLower().Trim() == name.ToLower().Trim()).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
        }

        public Card FindCardById(Guid value)
        {
            return Cards.Where(c => c.Id == value).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
        }
    }
}
