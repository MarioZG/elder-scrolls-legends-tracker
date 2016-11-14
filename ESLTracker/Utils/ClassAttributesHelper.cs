using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.Utils
{
    public class ClassAttributesHelper
    {
        public static Dictionary<DeckClass, DeckAttributes> classes = new Dictionary<DeckClass, DeckAttributes>()
        {
            { DeckClass.Neutral, new DeckAttributes() { DeckAttribute.Neutral } },
            { DeckClass.Agility, new DeckAttributes() { DeckAttribute.Agility } },
            { DeckClass.Strength, new DeckAttributes() { DeckAttribute.Strength } },
            { DeckClass.Willpower, new DeckAttributes() { DeckAttribute.Willpower } },
            { DeckClass.Inteligence, new DeckAttributes() { DeckAttribute.Intelligence } },
            { DeckClass.Endurance, new DeckAttributes() { DeckAttribute.Endurance } },
            { DeckClass.Archer, new DeckAttributes() { DeckAttribute.Agility, DeckAttribute.Strength } },

            { DeckClass.Assassin, new DeckAttributes() { DeckAttribute.Agility, DeckAttribute.Intelligence}},
            { DeckClass.Battlemage, new DeckAttributes() { DeckAttribute.Intelligence, DeckAttribute.Strength}},
            { DeckClass.Crusader, new DeckAttributes() { DeckAttribute.Strength, DeckAttribute.Willpower}},
            { DeckClass.Mage, new DeckAttributes() { DeckAttribute.Intelligence, DeckAttribute.Willpower}},
            { DeckClass.Monk, new DeckAttributes() { DeckAttribute.Agility, DeckAttribute.Willpower}},
            { DeckClass.Scout, new DeckAttributes() { DeckAttribute.Agility, DeckAttribute.Endurance}},
            { DeckClass.Sorcerer, new DeckAttributes() { DeckAttribute.Endurance, DeckAttribute.Intelligence}},
            { DeckClass.Spellsword, new DeckAttributes() { DeckAttribute.Endurance, DeckAttribute.Willpower}},
            { DeckClass.Warrior, new DeckAttributes() { DeckAttribute.Endurance, DeckAttribute.Strength}}
        };

        public static Dictionary<DeckClass, DeckAttributes> Classes
        {
            get
            {
                return classes;
            }
        }

        public static List<DeckClass> FindClassByAttribute(List<DeckAttribute> filter)
        {
            List<DeckClass> ret;

            ret = classes
                .Where(c => filter.All(f=> c.Value.Contains(f)))
                .Select(c => c.Key).ToList();

            return ret;
        }

    }
}
