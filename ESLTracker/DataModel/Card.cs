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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

        [XmlIgnore]
        public string Set { get; set; }

        [XmlIgnore]
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public List<CardKeyword> Keywords { get; set; } = new List<CardKeyword>();

        public string ImageName
        {
            get
            {
                Regex rgx = new Regex("[^a-zA-Z0-9]");
                var name = rgx.Replace(Name, "");
                return "pack://application:,,,/Resources/Cards/" + name + ".png";
            }
        }

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
