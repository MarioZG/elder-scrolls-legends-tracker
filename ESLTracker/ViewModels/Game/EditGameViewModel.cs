using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels.Game
{
    public class EditGameViewModel : ViewModelBase
    {
        public DataModel.Game game = new DataModel.Game();
        public DataModel.Game Game
        {
            get { return game; }
            set
            {
                game = value;
                Game.PropertyChanged += Game_PropertyChanged;
                RaisePropertyChangedEvent("Game");
            }
        }

        public bool DisplayPlayerRank
        {
            get
            {
                return this.Game.Type == DataModel.Enums.GameType.PlayRanked;
            }
        }

        public bool DisplayBonusRound
        {   
            get
            {
                return this.Game.Type == DataModel.Enums.GameType.PlayRanked;
            }
        }

        public IEnumerable<GameType> AllowedGameTypes
        {
            get
            {
                return GetAllowedGameTypes();

            }
        }

       

        public RelayCommand CommandButtonCreate
        {
            get
            {
                return new RelayCommand(
                    new Action<object>(CommandButtonCreateExecute),
                    new Func<object, bool>(CommandButtonCreateCanExecute)
                    );
            }
        }

        public EditGameViewModel()
        {
            Game.PropertyChanged += Game_PropertyChanged;
            Tracker.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ActiveDeck")
            {
                RaisePropertyChangedEvent("AllowedGameTypes");
            }
            //throw new NotImplementedException();
        }

        private void Game_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
            {
                RaisePropertyChangedEvent("DisplayPlayerRank");
                RaisePropertyChangedEvent("DisplayBonusRound");

                if (this.Game.Type == DataModel.Enums.GameType.PlayRanked)
                {
                    this.Game.BonusRound = false;
                }
                else
                {
                    this.Game.BonusRound = null;
                }
            }
        }

        public void UpdateBindings()
        {
            RaisePropertyChangedEvent("Game");
        }

        public void CommandButtonCreateExecute(object parameter)
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

        public bool CommandButtonCreateCanExecute(object parameter)
        {
            return true;
        }

        private IEnumerable<GameType> GetAllowedGameTypes()
        {
            if (Tracker.Instance.ActiveDeck != null)
            {
                switch (Tracker.Instance.ActiveDeck.Type)
                {
                    case DeckType.Constructed:
                        return new List<GameType>() { GameType.PlayCasual, GameType.PlayRanked };
                        break;
                    case DeckType.VersusArena:
                        return new List<GameType>() { GameType.VersusArena };
                        this.Game.Type = GameType.VersusArena;
                        break;
                    case DeckType.SoloArena:
                        return new List<GameType>() { GameType.SoloArena };
                        this.Game.Type = GameType.SoloArena;
                        break;
                    default:
                        break;
                }
            }
            return Enum.GetValues(typeof(GameType)).Cast<GameType>();
        }


    }
}
