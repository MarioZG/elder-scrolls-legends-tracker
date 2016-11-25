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
            return String.Join(",", this);
        }

    }
}
