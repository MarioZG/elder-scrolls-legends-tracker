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
                    _instance = new CardsDatabase("./Resources/cards.json");
                }
                return _instance;
            }
        }

        private string databasePath;

        private CardsDatabase()
        {

        }

        public CardsDatabase(string datbasePath)
        {
            this.databasePath = datbasePath;
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
            cards = SerializationHelper.DeserializeJson<IEnumerable<Card>>(System.IO.File.ReadAllText(this.databasePath));
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
