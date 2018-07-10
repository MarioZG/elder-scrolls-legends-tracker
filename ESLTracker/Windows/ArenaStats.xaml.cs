using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ESLTracker.ViewModels;
using ESLTracker.ViewModels.Windows;

namespace ESLTracker.Windows
{
    /// <summary>
    /// Interaction logic for ArenaStats.xaml
    /// </summary>
    public partial class ArenaStats : Window
    {
        new public ArenaStatsViewModel DataContext
        {
            get
            {
                return (ArenaStatsViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public ArenaStats()
        {
            InitializeComponent();
        }
    }    
}
