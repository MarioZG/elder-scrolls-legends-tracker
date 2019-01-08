using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TESLTracker.DataModel.Enums;

namespace TESLTracker.DataModel
{
    [DebuggerDisplay("Name={Name}")]
    public class Card
    {
        public static Card Unknown { get; } = new Card() {
            Name = "Unknown",
            Attributes = new DeckAttributes(DeckClass.Neutral, new DeckAttribute[] { DeckAttribute.Neutral })
        };

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

        [XmlIgnore]
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public List<CardMechanic> Mechanics { get; set; } = new List<CardMechanic>();

        public string ImageName
        {
            get
            {
                Regex rgx = new Regex("[^a-zA-Z0-9]");
                var name = rgx.Replace(Name, "");
                return "pack://application:,,,/Resources/Cards/" + name + ".png";
            }
        }
    }
}
