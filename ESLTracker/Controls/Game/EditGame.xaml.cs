using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ESLTracker.DataModel.Enums;

namespace ESLTracker.Controls.Game
{
    /// <summary>
    /// Interaction logic for EditGame.xaml
    /// </summary>
    public partial class EditGame : UserControl
    {

        public EditGame()
        {
            InitializeComponent();
            this.selectedDeck.DataContext = DataModel.Tracker.Instance.ActiveDeck;

            DataModel.Tracker.Instance.PropertyChanged += Instance_PropertyChanged;

        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.selectedDeck.DataContext = DataModel.Tracker.Instance.ActiveDeck;
            if (this.selectedDeck.DataContext != null)
            {
                if (DataModel.Tracker.Instance.ActiveDeck.Type == DeckType.VersusArena)
                {
                    this.cbGameType.SelectedItem = DataModel.Enums.GameType.VersusArena;
                }
            }
        }

      
    }
}
