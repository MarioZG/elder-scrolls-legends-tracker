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

        // pairs old, new guid
        internal static Dictionary<string, string> GuidTranslation = new Dictionary<string, string>()
        {
            { "af763bc0-b6bd-4d33-b229-36473bcf5a51", "925259a6-95df-4d97-806d-895e3f2605a4" }, //Aela the Huntress
            { "ecf450ad-af09-438c-90ea-a6d58e6b3d53", "17d93fdd-e7b8-46b5-8a61-a459ab8ff9bf" }, //Aela's Huntmate
            { "7843fde3-4596-45f3-acbc-8ffc376597d7", "406c7363-e7af-4afb-82fd-2361c6059db9" }, //Circle Initiate
            { "5b569f48-f88f-11e6-bc64-92361f002671", "40813eec-294a-4f53-b5ba-2b68bd286468" }, //Companion Harbinger
            { "b27dfe4a-3948-44df-b82a-8e8ebcfbbf07", "1816a1ac-912d-44dd-9a40-c11343393eb1" }, //Grim Shield-Brother
            { "e62d3cc5-6650-11e6-bdf4-0800200c9a66", "17bc012e-3dc3-4b69-b276-1531c82b4660" }, //Whiterun Protector
        };

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
            var ret = Cards.Where(c => c.Id == value).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
            if (ret == Card.Unknown)
            {
                if (GuidTranslation.ContainsKey(value.ToString().ToLower()))
                {
                    ret = Cards.Where(c => c.Id == Guid.Parse(GuidTranslation[value.ToString().ToLower()])).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
                }
            }
            return ret;
        }

        public ICardsDatabase RealoadDB()
        {
            _instance = LoadCardsDatabase("./Resources/cards.json");
            return _instance;
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

        public CardSet FindCardSetByName(string value)
        {
            return CardSets.Where(c => c.Name == value).SingleOrDefault();
        }

        public IEnumerable<string> GetCardsNames(string setFilter = null)
        {
            return Cards.Where(c => String.IsNullOrEmpty(setFilter) || (c.Set == setFilter)).Select(c => c.Name);
        }
    }
}
