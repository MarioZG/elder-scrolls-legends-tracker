using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;

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

        string opponentClassWins = "Select opponent class";
        public string OpponentClassWins
        {
            get
            {
                return opponentClassWins;
            }
            set
            {
                opponentClassWins = value;
                RaisePropertyChangedEvent("OpponentClassWins");
            }
        }
       
        public string SummaryText
        {
            get
            {
                if (! String.IsNullOrWhiteSpace(game.OpponentName)
                    && game.OpponentClass.HasValue)
                {
                    return string.Format("Game vs {0} ({1})", game.OpponentName, game.OpponentClass);
                }
                else
                {
                    return "Waitng for game details...";
                }
            }
        }

        public bool isEditControl = false;
        public bool IsEditControl {
            get { return isEditControl; }
            set { isEditControl = value; RaisePropertyChangedEvent("IsEditControl"); }
        }

        public RelayCommandWithSettings CommandButtonCreate
        {
            get
            {
                return new RelayCommandWithSettings(
                    new Action<object, Properties.ISettings>(CommandButtonCreateExecute),
                    new Func<object, Properties.ISettings, bool>(CommandButtonCreateCanExecute),
                    Properties.Settings.Default
                    );
            }
        }

        public RelayCommand CommandButtonSaveChanges
        {
            get
            {
                return new RelayCommand(
                    new Action<object>(CommandButtonSaveChangesExecute)
                    );
            }
        }

        public EditGameViewModel()
        {
            //this.PropertyChanged += EditGameViewModel_PropertyChanged;
            Game.PropertyChanged += Game_PropertyChanged;
            Tracker.Instance.PropertyChanged += Instance_PropertyChanged;
            Utils.Messenger.Default.Register<Utils.Messages.EditGame>(this, EditGameStart, Utils.Messages.EditGame.Context.StartEdit);

        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ActiveDeck")
            {
                RaisePropertyChangedEvent("AllowedGameTypes");
            }
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
            else if ((e.PropertyName == "OpponentName")
                || (e.PropertyName == "OpponentClass"))
            {
                  RaisePropertyChangedEvent("SummaryText");
            }
        }

        public void UpdateBindings()
        {
            RaisePropertyChangedEvent("Game");
        }

        public void CommandButtonCreateExecute(object parameter, Properties.ISettings settings)
        {
            //object[] args = parameter as object[];
            GameOutcome? outcome = EnumManager.ParseEnumString<GameOutcome>(parameter as string);
            if ((outcome.HasValue)
                && this.Game.OpponentClass.HasValue)
            {
                this.Game.Deck = Tracker.Instance.ActiveDeck;
                this.Game.OpponentAttributes.AddRange(Utils.ClassAttributesHelper.Classes[this.game.OpponentClass.Value]);
                this.Game.Outcome = outcome.Value;

                if (this.Game.Type == GameType.PlayRanked)
                {
                    if (Game.OpponentRank != PlayerRank.TheLegend)
                    {
                        this.Game.OpponentLegendRank = null;
                    }

                    if (this.Game.PlayerRank != PlayerRank.TheLegend)
                    {
                        this.Game.PlayerLegendRank = null;
                    }
                    settings.PlayerRank = this.Game.PlayerRank.Value;
                }

                DataModel.Game addedGame = this.Game;
                Tracker.Instance.Games.Add(this.Game);

                Utils.Messenger.Default.Send(
                    new Utils.Messages.EditDeck() { Deck = game.Deck },
                    Utils.Messages.EditDeck.Context.StatsUpdated);

                FileManager.SaveDatabase();

                this.Game = new DataModel.Game();

                //restore values that are likely the same,  like game type, player rank etc
                this.Game.Type = addedGame.Type;
                this.Game.PlayerRank = addedGame.PlayerRank;
                this.Game.PlayerLegendRank = addedGame.PlayerLegendRank;
                this.UpdateBindings();

                //clear opp class
                //opponentClass.Reset();

                //clear opp rank
                //opponentRank.SelectedRank = null;
                //opponentRank.LegendRank = null;

                RaisePropertyChangedEvent("");

            }

        }

        public bool CommandButtonCreateCanExecute(object parameter, Properties.ISettings settings)
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

        public void ShowWinsVsClass(DeckClass? deckClass)
        {
            //this.Game.OpponentClass = deckClass; //ugly hack until class slectro can be bound in xaml
            if ((deckClass != null) && (Tracker.Instance.ActiveDeck != null))
            {
                var res = Tracker.Instance.ActiveDeck.GetDeckVsClass(deckClass);
                dynamic data = (res as System.Collections.IEnumerable).Cast<object>().FirstOrDefault();
                if (data == null)
                {
                    data = new { Class = deckClass, Victory = 0, Defeat = 0, WinPercent = "-" };
                }
                this.OpponentClassWins = string.Format("vs {0}: {1}-{2} ({3}%)",
                    data.Class,
                    data.Victory,
                    data.Defeat,
                    data.WinPercent);
            }
            else
            {
                this.OpponentClassWins = "Select opponent class";
            }
        }

        private void EditGameStart(EditGame obj)
        {
            if (IsEditControl)
            {
                this.Game = obj.Game;
                //Game.UpdateAllBindings();
                RaisePropertyChangedEvent("");
            }
        }

        private void CommandButtonSaveChangesExecute(object parameter)
        {
            if (this.Game.OpponentClass.HasValue)
            {
                this.Game.OpponentAttributes.AddRange(Utils.ClassAttributesHelper.Classes[this.Game.OpponentClass.Value]);

                if (this.Game.Type == GameType.PlayRanked)
                {
                    if (Game.OpponentRank != PlayerRank.TheLegend)
                    {
                        this.Game.OpponentLegendRank = null;
                    }

                    if (this.Game.PlayerRank != PlayerRank.TheLegend)
                    {
                        this.Game.PlayerLegendRank = null;
                    }
                }
                else
                {
                    Game.OpponentRank = null;
                    Game.OpponentLegendRank = null;
                    Game.PlayerLegendRank = null;
                    Game.PlayerRank = null;
                    Game.BonusRound = null;
                }
                Utils.Messenger.Default.Send(
                    new Utils.Messages.EditDeck() { Deck = game.Deck },
                    Utils.Messages.EditDeck.Context.StatsUpdated);

                FileManager.SaveDatabase();
            }

            Messenger.Default.Send(
                new EditGame(Game),
                EditGame.Context.EditFinished);
            Game.UpdateAllBindings();
        }


    }
}
