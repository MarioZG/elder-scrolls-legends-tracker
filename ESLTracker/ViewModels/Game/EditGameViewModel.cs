using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;

namespace ESLTracker.ViewModels.Game
{
    public class EditGameViewModel : ViewModelBase, IEditableObject
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

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; RaisePropertyChangedEvent("ErrorMessage"); }
        }


        public ICommand CommandButtonCreate
        {
            get
            {
                return new RelayCommand(
                    new Action<object>(CommandButtonCreateExecute)//,
                    //new Func<object, bool>(CommandButtonCreateCanExecute)
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

        public RelayCommand CommandButtonCancelChanges
        {
            get
            {
                return new RelayCommand(
                    new Action<object>(CommandButtonCancelChangesExecute)
                    );
            }
        }


        public RelayCommand CommandButtonResetAllChanges
        {
            get
            {
                return new RelayCommand(
                    new Action<object>(CommandButtonResetAllChangesExecute)
                    );
            }
        }

        public ICommand commandExecuteWhenContinueOnError;
        public ICommand CommandExecuteWhenContinueOnError
        {
            get { return commandExecuteWhenContinueOnError; }
            set { commandExecuteWhenContinueOnError = value; RaisePropertyChangedEvent(); }
        }

        ITrackerFactory trackerFactory;
        IMessenger messanger;
        ITracker tracker;

        public EditGameViewModel() : this(new TrackerFactory())
        {

        }

        internal EditGameViewModel(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;

            messanger = trackerFactory.GetMessanger();
            tracker = trackerFactory.GetTracker();
            Game.PropertyChanged += Game_PropertyChanged;
            tracker.PropertyChanged += Instance_PropertyChanged;
            messanger.Register<Utils.Messages.EditGame>(this, EditGameStart, Utils.Messages.EditGame.Context.StartEdit);

            this.BeginEdit();
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ActiveDeck")
            {
                if (savedState != null)
                {
                    savedState.Deck = tracker.ActiveDeck;
                }
                RaisePropertyChangedEvent("AllowedGameTypes");
            }
       }

        private void Game_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
            {
                RaisePropertyChangedEvent("DisplayPlayerRank");
                RaisePropertyChangedEvent("DisplayBonusRound");

                if (this.Game.Type == GameType.PlayRanked)
                {
                    this.Game.BonusRound = false;
                    this.Game.PlayerRank = Properties.Settings.Default.PlayerRank;
                }
                else
                {
                    Game.OpponentRank = null;
                    Game.OpponentLegendRank = null;
                    Game.PlayerLegendRank = null;
                    Game.PlayerRank = null;
                    Game.BonusRound = null;
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

        public void CommandButtonCreateExecute(object parameter)
        {
            //object[] args = parameter as object[];
            GameOutcome? outcome = EnumManager.ParseEnumString<GameOutcome>(parameter as string);
            this.ErrorMessage = null;
            if (tracker.ActiveDeck == null)
            {
                this.ErrorMessage += "Please select active deck" + Environment.NewLine;
            }
            if (!this.Game.OpponentClass.HasValue)
            {
                this.ErrorMessage += "Please select opponent class" + Environment.NewLine;
            }
            this.ErrorMessage = this.ErrorMessage?.Trim();

            if ((tracker.ActiveDeck != null)
                && (outcome.HasValue)
                && this.Game.OpponentClass.HasValue)
            {
                UpdateGameData(trackerFactory.GetSettings(), outcome);

                DataModel.Game addedGame = this.Game;
                tracker.Games.Add(this.Game);

                messanger.Send(
                    new Utils.Messages.EditDeck() { Deck = game.Deck },
                    Utils.Messages.EditDeck.Context.StatsUpdated);

                new FileManager(trackerFactory).SaveDatabase();

                this.Game = new DataModel.Game();

                //restore values that are likely the same,  like game type, player rank etc
                this.Game.Type = addedGame.Type;
                this.Game.PlayerRank = addedGame.PlayerRank;
                this.Game.PlayerLegendRank = addedGame.PlayerLegendRank;

                this.BeginEdit();

                this.UpdateBindings();

                //clear opp class
                //opponentClass.Reset();

                //clear opp rank
                //opponentRank.SelectedRank = null;
                //opponentRank.LegendRank = null;

                RaisePropertyChangedEvent("");

            }

        }

        internal void UpdateGameData(Properties.ISettings settings, GameOutcome? outcome)
        {
            this.Game.Deck = tracker.ActiveDeck;
            this.Game.Outcome = outcome.Value;
            this.Game.Date = trackerFactory.GetDateTimeNow(); //game date - when it concluded (we dont know when it started)
            FileVersionInfo fvi = trackerFactory.GetWinAPI().GetEslFileVersionInfo();
            if (fvi != null)
            {
                this.Game.ESLVersion = new SerializableVersion(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
            }
            else
            {
                this.Game.ESLVersion = tracker.Games
                    .Where(g => g.ESLVersion != null)
                    .OrderByDescending(g => g.Date)
                    .Select(g => g.ESLVersion)
                    .FirstOrDefault();
            }
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
        }

        public bool CommandButtonCreateCanExecute(object parameter)
        {
            return ((tracker.ActiveDeck != null) 
                && this.Game.OpponentClass.HasValue);
        }

        private IEnumerable<GameType> GetAllowedGameTypes()
        {
            if (tracker.ActiveDeck != null)
            {
#pragma warning disable CS0162 // Unreachable code detected
                switch (tracker.ActiveDeck.Type)
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
#pragma warning restore CS0162 // Unreachable code detected
            }
            return Enum.GetValues(typeof(GameType)).Cast<GameType>();
        }

        public void ShowWinsVsClass(DeckClass? deckClass)
        {
            //this.Game.OpponentClass = deckClass; //ugly hack until class slectro can be bound in xaml
            if ((deckClass != null) && (tracker.ActiveDeck != null))
            {
                var res = tracker.ActiveDeck.GetDeckVsClass(deckClass);
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
                this.BeginEdit();
                RaisePropertyChangedEvent("");
            }
        }

        public void CommandButtonSaveChangesExecute(object parameter)
        {
            if (this.Game.OpponentClass.HasValue)
            {
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

                messanger.Send(
                    new Utils.Messages.EditDeck() { Deck = game.Deck },
                    Utils.Messages.EditDeck.Context.StatsUpdated);

                new FileManager(trackerFactory).SaveDatabase();
            }

            this.EndEdit();
            messanger.Send(
                new EditGame(Game),
                EditGame.Context.EditFinished);
            Game.UpdateAllBindings();
        }

        private void CommandButtonCancelChangesExecute(object obj)
        {
            this.CancelEdit();
            messanger.Send(
                new EditGame(Game),
                EditGame.Context.EditFinished);
            Game.UpdateAllBindings();
        }

        DataModel.Game savedState;

        public void BeginEdit()
        {
            savedState = Game.Clone() as DataModel.Game;
        }

        public void EndEdit()
        {
            savedState = Game;
        }

        public void CancelEdit()
        {
            Game.BonusRound = savedState.BonusRound;
            Game.Date = savedState.Date;
            Game.Deck = savedState.Deck;
            Game.DeckId = savedState.DeckId;
            Game.Notes = savedState.Notes;
            Game.OpponentClass = savedState.OpponentClass;
            Game.OpponentLegendRank = savedState.OpponentLegendRank;
            Game.OpponentName = savedState.OpponentName;
            Game.OpponentRank = savedState.OpponentRank;
            Game.OrderOfPlay = savedState.OrderOfPlay;
            Game.Outcome = savedState.Outcome;
            Game.PlayerLegendRank = savedState.PlayerLegendRank;
            Game.PlayerRank = savedState.PlayerRank;
            Game.Type = savedState.Type;
        }

        internal bool IsDirty()
        {
            return !Game.Equals(savedState);
        }

        private void CommandButtonResetAllChangesExecute(object obj)
        {
            CommandExecuteWhenContinueOnError.Execute(null);
            this.ErrorMessage = String.Empty;
        }
    }
}
