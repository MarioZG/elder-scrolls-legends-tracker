using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Utils;

namespace ESLTracker.Services
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
                    _instance = LoadCardsDatabase("./Resources/cards.json");
                }
                return _instance;
            }
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
                return cards;
            }
            set
            {
                cards = value;
            }
        }

        public Version Version { get; set; }
        public string VersionInfo { get; set; }
        public DateTime VersionDate { get; set; }

        IEnumerable<CardSet> cardSets;
        public IEnumerable<CardSet> CardSets
        {
            get
            {
                return cardSets;
            }
            set
            {
                cardSets = value;
            }
        }

        private CardsDatabase()
        {

        }

        public static CardsDatabase LoadCardsDatabase(string datbasePath)
        {
            if (new Utils.IOWrappers.FileWrapper().Exists(datbasePath))
            {
                CardsDatabase database = SerializationHelper.DeserializeJson<CardsDatabase>(System.IO.File.ReadAllText(datbasePath));
                return database;
            }
            else
            {
                return null;
            }
        }

        public Card FindCardByName(string name)
        {
            return Cards.Where(c => c.Name.ToLower().Trim() == name.ToLower().Trim()).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
        }

        public Card FindCardById(Guid value)
        {
            return Cards.Where(c => c.Id == value).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
        }

        public void RealoadDB()
        {
            _instance = LoadCardsDatabase("./Resources/cards.json");
        }

        public CardSet FindCardSetById(Guid? value)
        {
            if (value.HasValue)
            {
                return CardSets.Where(c => c.Id == value).SingleOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}
