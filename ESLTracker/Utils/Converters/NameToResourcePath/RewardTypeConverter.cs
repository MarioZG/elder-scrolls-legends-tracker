using ESLTracker.BusinessLogic.Packs;
using ESLTracker.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Converters.NameToResourcePath
{
    public class RewardTypeConverter : BaseConverter<RewardTypeConverter>
    {
        protected override string Path => "Resources/RewardType";

        protected override void ValidateValue(object value)
        {
            if (!(value is RewardType))
            {
                throw new ArgumentException(nameof(RewardTypeConverter) + " can accept only RewardType as value");
            }
        }

        protected override bool ShouldConvert(string castedValue)
        {
            return !String.IsNullOrWhiteSpace(castedValue);
        }
    }
}
