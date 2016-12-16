using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.DataModel
{
    public class DeckAttributes : List<DeckAttribute>
    {

        DeckClass deckClass;

        public DeckAttributes()
        {
        }

        public DeckAttributes(DeckClass deckClass)
        {
            this.deckClass = deckClass;
        }

        public DeckAttributes(DeckClass deckClass, IEnumerable<DeckAttribute> collection) : base(collection)
        {
            this.deckClass = deckClass;
        }

        public IEnumerable<string> ImageSources
        {
            get
            {
                foreach (DeckAttribute a in this)
                {
                    yield return "pack://application:,,,/Resources/DeckAttribute/"+a.ToString()+".png";
                }
            }
        }

        public override string ToString()
        {
            return deckClass.ToString();
        }

    }
}
