using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;

namespace ESLTracker.Utils
{
    public class ClassAttributesHelper
    {
        public static Dictionary<DeckClass, DeckAttributes> classes = new Dictionary<DeckClass, DeckAttributes>()
        {
            { DeckClass.Neutral, new DeckAttributes(DeckClass.Neutral) { DeckAttribute.Neutral } },
            { DeckClass.Agility, new DeckAttributes(DeckClass.Agility) { DeckAttribute.Agility } },
            { DeckClass.Strength, new DeckAttributes(DeckClass.Strength) { DeckAttribute.Strength } },
            { DeckClass.Willpower, new DeckAttributes(DeckClass.Willpower) { DeckAttribute.Willpower } },
            { DeckClass.Inteligence, new DeckAttributes(DeckClass.Inteligence) { DeckAttribute.Intelligence } },
            { DeckClass.Endurance, new DeckAttributes(DeckClass.Endurance) { DeckAttribute.Endurance } },
            { DeckClass.Archer, new DeckAttributes(DeckClass.Archer) { DeckAttribute.Agility, DeckAttribute.Strength } },

            { DeckClass.Assassin, new DeckAttributes(DeckClass.Assassin) { DeckAttribute.Intelligence, DeckAttribute.Agility}},
            { DeckClass.Battlemage, new DeckAttributes(DeckClass.Battlemage) { DeckAttribute.Strength, DeckAttribute.Intelligence}},
            { DeckClass.Crusader, new DeckAttributes(DeckClass.Crusader) { DeckAttribute.Strength, DeckAttribute.Willpower}},
            { DeckClass.Mage, new DeckAttributes(DeckClass.Mage) { DeckAttribute.Intelligence, DeckAttribute.Willpower}},
            { DeckClass.Monk, new DeckAttributes(DeckClass.Monk) { DeckAttribute.Willpower, DeckAttribute.Agility}},
            { DeckClass.Scout, new DeckAttributes(DeckClass.Scout) { DeckAttribute.Agility, DeckAttribute.Endurance}},
            { DeckClass.Sorcerer, new DeckAttributes(DeckClass.Sorcerer) { DeckAttribute.Intelligence, DeckAttribute.Endurance}},
            { DeckClass.Spellsword, new DeckAttributes(DeckClass.Spellsword) { DeckAttribute.Willpower, DeckAttribute.Endurance}},
            { DeckClass.Warrior, new DeckAttributes(DeckClass.Warrior) { DeckAttribute.Strength, DeckAttribute.Endurance}},

            { DeckClass.Redoran, new DeckAttributes(DeckClass.Redoran) { DeckAttribute.Strength, DeckAttribute.Willpower, DeckAttribute.Endurance}},
            { DeckClass.Telvanni, new DeckAttributes(DeckClass.Telvanni) { DeckAttribute.Intelligence, DeckAttribute.Agility, DeckAttribute.Endurance}},
            { DeckClass.Hlaalu, new DeckAttributes(DeckClass.Hlaalu) { DeckAttribute.Strength, DeckAttribute.Willpower, DeckAttribute.Agility}},
            { DeckClass.Tribunal , new DeckAttributes(DeckClass.Tribunal ) { DeckAttribute.Intelligence, DeckAttribute.Willpower, DeckAttribute.Endurance}},
            { DeckClass.Dagoth , new DeckAttributes(DeckClass.Dagoth ) { DeckAttribute.Strength, DeckAttribute.Intelligence, DeckAttribute.Agility}}
        };

        public static Dictionary<DeckClass, DeckAttributes> Classes
        {
            get
            {
                return classes;
            }
        }

        public static IEnumerable<DeckClass> FindClassByAttribute(DeckAttribute filter)
        {
            return FindClassByAttribute(new List<DeckAttribute>() { filter });
        }

        public static IEnumerable<DeckClass> FindClassByAttribute(IEnumerable<DeckAttribute> filter)
        {
            List<DeckClass> ret;

            ret = classes
                .Where(c => filter.All(f=> c.Value.Contains(f)))
                .Select(c => c.Key).ToList();

            return ret;
        }

        public static DeckClass FindSingleClassByAttribute(IEnumerable<DeckAttribute> filter)
        {
            filter = filter.Where(da => da != DeckAttribute.Neutral);
            DeckClass? ret;

            ret = classes
                .Where(c => filter.All(f => c.Value.Contains(f)) && filter.Count() == c.Value.Count)
                .Select(c => c.Key).FirstOrDefault();

            return ret.HasValue ? ret.Value : DeckClass.Neutral;
        }

        public static Dictionary<DeckAttribute, Color> DeckAttributeColors = new Dictionary<DeckAttribute, Color>()
        {
            { DeckAttribute.Agility, Color.FromArgb(255, 8, 100, 30 )},
            { DeckAttribute.Endurance, Color.FromArgb(255, 99, 55, 148 )},
            { DeckAttribute.Intelligence, Color.FromArgb(255, 5, 117, 192 )},
            { DeckAttribute.Neutral, Color.FromArgb(255, 115, 115, 115 )},
            { DeckAttribute.Strength, Color.FromArgb(255, 151, 49, 30 )},
            { DeckAttribute.Willpower, Color.FromArgb(255, 136, 102, 2 )}
        };

    }
}
