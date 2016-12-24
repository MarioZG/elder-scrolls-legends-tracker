using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.DataModel
{
    public class Card
    {
        private Guid id;
        public Guid Id {
            get
            {
                return id;
            }
            set {
                id = value;
                LoadFromDatabase(value);
            }
        }

        public static Card Unknown { get; } = new Card() { Name = "Unknown" };

        private void LoadFromDatabase(Guid value)
        {
            this.Name = "laded from DB";
        }

        [XmlIgnore]
        public string Name { get; set; }

        [XmlIgnore]
        public CardRarity Rarity { get; set; }
    }
}
