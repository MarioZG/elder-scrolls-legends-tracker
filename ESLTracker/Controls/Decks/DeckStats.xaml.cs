using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels.Decks;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for DeckStats.xaml
    /// </summary>
    public partial class DeckStats : UserControl
    {
        public DeckStats()
        {
            InitializeComponent();
            base.DataContext = MasserContainer.Container.GetInstance<DeckStatsViewModel>();
        }
    }
}
