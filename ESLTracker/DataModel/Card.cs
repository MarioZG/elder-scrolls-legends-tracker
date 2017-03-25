using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.DataModel
{
    [DebuggerDisplay("Name={Name}")]
    public class Card
    {
        public static Card Unknown { get; } = new Card() { Name = "Unknown" };

        private Guid id;
        public Guid Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        [XmlIgnore]
        public string Name { get; set; }

        [XmlIgnore]
        public CardRarity Rarity { get; set; }

        [XmlIgnore]
        public bool IsUnique { get; set; }

        [XmlIgnore]
        public DeckAttributes Attributes { get; set; }

        [XmlIgnore]
        public CardType Type { get; set; }

        [XmlIgnore]
        public int Cost { get; set; }

        [XmlIgnore]
        public int Attack { get; set; }

        [XmlIgnore]
        public int Health { get; set; }

        [XmlIgnore]
        public string Race { get; set; }

        [XmlIgnore]
        public string Text { get; set; }

        public string ImageName {get; set;}

        private ITrackerFactory trackerFactory;

        public Card() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public Card(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
        }
    }
}
