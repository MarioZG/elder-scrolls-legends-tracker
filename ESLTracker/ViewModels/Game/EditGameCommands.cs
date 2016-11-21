using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels.Game
{
    class CreateNewGameCommand : ICommand
    {
        public void Execute(object parameter)
        {
            object[] args = parameter as object[];
            GameOutcome? outcome = EnumManager.ParseEnumString<GameOutcome>(args[0] as string);
            EditGameViewModel model = args[1] as EditGameViewModel;
            DeckClassSelectorViewModel opponentClass = args[2] as DeckClassSelectorViewModel;
            Controls.PlayerRank opponentRank = args[3] as Controls.PlayerRank;
            Controls.PlayerRank playerRank = args[4] as Controls.PlayerRank;
            if ((model != null)
                && (outcome.HasValue)
                && (opponentClass != null)
                && opponentClass.SelectedClass.HasValue)
            {
                model.Game.Deck = DataModel.Tracker.Instance.ActiveDeck;
                model.Game.OpponentClass = opponentClass.SelectedClass.Value;
                model.Game.OpponentAttributes.AddRange(opponentClass.SelectedClassAttributes);
                model.Game.Outcome = outcome.Value;

                if (model.Game.Type == GameType.PlayRanked)
                {
                    model.Game.OpponentRank = opponentRank.SelectedItem;
                    model.Game.OpponentLegendRank = opponentRank.LegendRank;

                    model.Game.PlayerRank = playerRank.SelectedItem;
                    model.Game.PlayerLegendRank = playerRank.LegendRank;

                }

                DataModel.Game addedGame = model.Game;
                DataModel.Tracker.Instance.Games.Add(model.Game);

                Utils.FileManager.SaveDatabase();

                DataModel.Deck active = DataModel.Tracker.Instance.ActiveDeck;
                DataModel.Tracker.Instance.ActiveDeck = null;
                DataModel.Tracker.Instance.ActiveDeck = active;

                model.Game = new DataModel.Game();

                //restore values that are likely the same,  like game type, player rank etc
                model.Game.Type = addedGame.Type;
                model.Game.PlayerRank = addedGame.PlayerRank;
                model.Game.PlayerLegendRank = addedGame.PlayerLegendRank;
                model.UpdateBindings();

                //clear opp class
                opponentClass.SelectedClass = null;

            }

        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
