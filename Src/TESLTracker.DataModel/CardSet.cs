using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TESLTracker.DataModel
{
    public class CardSet
    {
        [XmlIgnore]
        public string Name { get; set; }

        [XmlAttribute("id")]
        public Guid Id { get; set; }

        [XmlIgnore]
        public bool HasPacks { get; set; }

        //public static explicit operator CardSet(string name)
        //{
        //    var cardDatabaseFactory = MasserContainer.Container.GetInstance<ICardsDatabaseFactory>();
        //    return cardDatabaseFactory.GetCardsDatabase().CardSets.Where(cs => cs.Name.ToLower() == name.ToLower()).SingleOrDefault();
        //}

        public override string ToString()
        {
            return Name;
        }
    }
}
