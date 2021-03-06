﻿using ESLTracker.BusinessLogic.Packs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Converters.NameToResourcePath
{
    public class CardSetConverter : BaseConverter<CardSetConverter>
    {
        protected override string Path => "Resources/Sets";

        protected override void ValidateValue(object value)
        {
            if (!(value is string))
            {
                throw new ArgumentException(nameof(CardSetConverter) + " can accept only string as value");
            }
        }

        protected override bool ShouldConvert(string castedValue)
        {
            return castedValue != CardSetsListProvider.AllSets.Name;
        }
    }
}
