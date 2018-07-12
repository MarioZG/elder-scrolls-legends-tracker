using ESLTracker.BusinessLogic.Cards;
using ESLTracker.Utils;
using ESLTracker.Utils.SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ESLTracker.DataModel
{
    public class CardSet
    {
        [XmlIgnore]
        public string Name { get; set; }

        [XmlAttribute("id")]
        public Guid Id { get; set; }

        [XmlIgnore]
        public bool HasPacks { get; set; }

        [XmlIgnore]
        public string ImagePath
        {
            get
            {
                Regex rgx = new Regex("[^a-zA-Z0-9]");
                var name = rgx.Replace(Name, "");
                return "pack://application:,,,/Resources/Sets/" + name + ".png";
            }
        }

        public static explicit operator CardSet(string name)
        {
            var cardDatabase = MasserContainer.Container.GetInstance<ICardsDatabase>();
            return cardDatabase.CardSets.Where(cs => cs.Name.ToLower() == name.ToLower()).SingleOrDefault();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
