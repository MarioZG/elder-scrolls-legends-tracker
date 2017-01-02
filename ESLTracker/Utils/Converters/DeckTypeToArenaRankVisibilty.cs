using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.Utils.Converters
{
    public class DeckTypeToArenaRankVisibilty : ToVisibilityConverter<DeckType>
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new DeckTypeToArenaRankVisibilty();
            }
            return converter;
        }

        protected override bool Condition(object value)
        {
            return Deck.IsArenaDeck((DeckType)value);
        }
    }
}

