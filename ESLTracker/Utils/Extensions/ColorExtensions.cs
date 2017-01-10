using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Extensions
{
    public static class ColorExtensions
    {
        public static Color ApplyFactor(
            this Color color,
            double factor)
        {
            return Color.FromArgb(255, (byte)(color.R * factor), (byte)(color.G * factor), (byte)(color.B * factor));
        }

        public static System.Windows.Media.SolidColorBrush ToMediaBrush(
            this Color color)
        {
            return
                new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromArgb(255, color.R, color.G, color.B)
                    );
        }
    }
}
