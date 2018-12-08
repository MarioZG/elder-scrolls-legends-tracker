using ESLTracker.BusinessLogic.Cards;
using ESLTracker.Utils;
using ESLTracker.Utils.SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
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

        //this shuld be in other class - decorator pattern or wpf converter?. Due to isses with serialisation left here
        [XmlIgnore]
        public object ImagePath
        {
            get
            {
                if (this.Id != Guid.Empty)
                {
                    Regex rgx = new Regex("[^a-zA-Z0-9]");
                    var name = rgx.Replace(this.Name, "");
                    return "pack://application:,,,/Resources/Sets/" + name + ".png";
                }
                else
                {
                    return Binding.DoNothing;
                }
            }
        }

        public static explicit operator CardSet(string name)
        {
            var cardDatabaseFactory = MasserContainer.Container.GetInstance<ICardsDatabaseFactory>();
            return cardDatabaseFactory.GetCardsDatabase().CardSets.Where(cs => cs.Name.ToLower() == name.ToLower()).SingleOrDefault();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
