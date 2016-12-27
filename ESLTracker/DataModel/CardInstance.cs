using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ESLTracker.Utils;

namespace ESLTracker.DataModel
{
    public class CardInstance 
    {
        private TrackerFactory trackerFactory;

        public Guid CardId
        {
            get
            {
                return Card.Id;
            }
            set
            {
                LoadCardFromDataBase(value);
            }
        }

        [XmlIgnore]
        public Card Card { get; set; }

        public bool IsGolden { get; set; }

        public CardInstance() : this(new TrackerFactory())
        {

        }

        public CardInstance(Card card) : this(card, new TrackerFactory())
        {

        }

        public CardInstance(TrackerFactory trackerFactory) : this(null, trackerFactory)
        {
            
        }

        public CardInstance(Card card, TrackerFactory trackerFactory)
        {
            this.Card = card;
            this.trackerFactory = trackerFactory;
        }

        private void LoadCardFromDataBase(Guid value)
        {
            this.Card = trackerFactory.GetCardsDatabase().FindCardById(value);
        }
    }
}
