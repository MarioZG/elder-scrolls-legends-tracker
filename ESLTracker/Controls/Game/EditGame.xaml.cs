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
            if (DataModel.Tracker.Instance.ActiveDeck.Type == DeckType.VesrusArena)
            {
                this.cbGameType.SelectedItem = DataModel.Enums.GameType.VersusArena;
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnVicotry_Click(object sender, RoutedEventArgs e)
        {
            if (!opponentClass.DataContext.SelectedClass.HasValue)
            {
                return;
            }

            DataModel.Game game = new DataModel.Game()
            {
                Deck = this.selectedDeck.DataContext as DataModel.Deck,
                BonusRound = null,
                OpponentClass = this.opponentClass.DataContext.SelectedClass.Value,
                OrderOfPlay = (OrderOfPlay)this.cbOrderOfPlay.SelectedItem,
                Outcome = Utils.EnumManager.ParseEnumString<GameOutcome>(((Button)sender).CommandParameter.ToString()),
                Type = (GameType)this.cbGameType.SelectedItem,
                OpponentName = this.txtOpponentName.Text
            };

            game.OpponentAttributes.AddRange(opponentClass.DataContext.SelectedClassAttributes);

            if (game.Type == GameType.PlayRanked)
            {
                game.BonusRound = cbBonusRound.IsChecked;
                game.OpponentRank = (DataModel.Enums.PlayerRank)this.cbOpponentRank.SelectedItem;
                game.PlayerRank = (DataModel.Enums.PlayerRank)this.cbPlayerRank.SelectedItem;
            }

            DataModel.Tracker.Instance.Games.Add(game);

            this.opponentClass.DataContext.Reset();
            this.txtOpponentName.Text = "";
            this.cbOrderOfPlay.SelectedItem = null;
            this.cbOpponentRank.SelectedItem = null;
            this.cbBonusRound.IsChecked = false;

            this.selectedDeck.DataContext = null;
            this.selectedDeck.DataContext = DataModel.Tracker.Instance.ActiveDeck;
        }
    }
}
