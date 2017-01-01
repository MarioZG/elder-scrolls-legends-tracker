using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.Utils.Converters
{
    public class PlayerRankLegendToVisibiltyCollapsed : InvertibleConverter<Visibility, PlayerRank>
    {
        protected override Visibility ReturnWhenFalse
        {
            get
            {
                return Visibility.Collapsed;
            }
        }

        protected override Visibility ReturnWhenTrue
        {
            get
            {
                return Visibility.Visible;
            }
        }

        protected override bool Condition(object value)
        {
            return (((PlayerRank)value) == PlayerRank.TheLegend);
        }
    }
}
