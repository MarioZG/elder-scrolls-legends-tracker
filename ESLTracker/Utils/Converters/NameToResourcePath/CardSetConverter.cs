using ESLTracker.BusinessLogic.Packs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Converters.NameToResourcePath
{
    public class NameToResourcePathCardSetConverter : NameToResourcePathBaseConverter<NameToResourcePathCardSetConverter>
    {
        protected override string Path => "Resources/Sets";

        protected override bool ShouldConvert(string castedValue)
        {
            return castedValue != CardSetsListProvider.AllSets.Name;
        }
    }
}
