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
using ESLTracker.Properties;
using ESLTracker.Services;
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
                RaisePropertyChangedEvent(nameof(Game));
                ShowWinsVsClass(game?.OpponentClass);
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
                RaisePropertyChangedEvent(nameof(OpponentClassWins));
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
            set { isEditControl = value; RaisePropertyChangedEvent(nameof(IsEditControl)); }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; RaisePropertyChangedEvent(nameof(ErrorMessage)); }
        }

        public IEnumerable<string> OpponentDeckTagAutocomplete
        {
            get
            {
                return tracker.DeckTags;
            }
        }

        public ICommand CommandButtonCreate
        {
            get
            {
                return new RelayCommand(
                    new Action<object>(CommandButtonCreateExecute)
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
        ISettings settings;

        public EditGameViewModel() : this(new TrackerFactory())
        {

        }

        internal EditGameViewModel(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;

            messanger = trackerFactory.GetService<IMessenger>();
            tracker = trackerFactory.GetTracker();
            Game.PropertyChanged += Game_PropertyChanged;
            messanger.Register<ActiveDeckChanged>(this, ActiveDeckChanged);
            messanger.Register<EditGame>(this, EditGameStart, Utils.Messages.EditGame.Context.StartEdit);
            messanger.Register<NewDeckTagCreated>(this, RefreshTagsList);
            this.settings = trackerFactory.GetService<ISettings>();

            this.BeginEdit();
        }

        internal void ActiveDeckChanged(ActiveDeckChanged activeDeckChanged)
        {
            if (!IsEditControl)
            {
                if (activeDeckChanged.ActiveDeck != null)
                {
                    this.Game.Deck = activeDeckChanged.ActiveDeck;
                    this.Game.DeckVersionId = activeDeckChanged.ActiveDeck.SelectedVersionId;
                    if (savedState != null)
                    {
                        savedState.Deck = tracker.ActiveDeck;
                        savedState.DeckVersionId = tracker.ActiveDeck.SelectedVersionId;
                    }
                    RaisePropertyChangedEvent(nameof(AllowedGameTypes));
                }                
            }
        }

        private void Game_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Game.Type))
            {
                RaisePropertyChangedEvent(nameof(DisplayPlayerRank));
                RaisePropertyChangedEvent(nameof(DisplayBonusRound));

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
            else if (e.PropertyName == nameof(Game.OpponentName))
            {
                RaisePropertyChangedEvent(nameof(SummaryText));
            }
            else if (e.PropertyName == nameof(Game.OpponentClass))
            {
                RaisePropertyChangedEvent(nameof(SummaryText));
                ShowWinsVsClass(this.Game.OpponentClass);
            }
            else if (e.PropertyName == nameof(Game.OpponentDeckTag))
            {
                if (! tracker.DeckTags.Contains(Game.OpponentDeckTag))
                {
                    messanger.Send(new NewDeckTagCreated());
                }
            }
        }

        public void UpdateBindings()
        {
            RaisePropertyChangedEvent(nameof(Game));
        }

        public void CommandButtonCreateExecute(object parameter)
        {
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
                UpdateGameData(outcome);

                DataModel.Game addedGame = this.Game;
                tracker.Games.Add(this.Game);

                messanger.Send(
                    new Utils.Messages.EditDeck() { Deck = game.Deck },
                    Utils.Messages.EditDeck.Context.StatsUpdated);

                trackerFactory.GetFileManager().SaveDatabase();

                this.Game = new DataModel.Game();

                //restore values that are likely the same,  like game type, player rank etc
                this.Game.Type = addedGame.Type;
                this.Game.PlayerRank = addedGame.PlayerRank;
                this.Game.PlayerLegendRank = addedGame.PlayerLegendRank;

                this.BeginEdit();

                this.UpdateBindings();

                RaisePropertyChangedEvent(String.Empty);

            }

        }

        internal void UpdateGameData(GameOutcome? outcome)
        {
            this.Game.Deck = tracker.ActiveDeck;
            this.Game.DeckVersionId = tracker.ActiveDeck.SelectedVersionId;
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
            List<GameType> allowedGameTypes = null;
            if (tracker.ActiveDeck != null)
            {
                switch (tracker.ActiveDeck.Type)
                {
                    case DeckType.Constructed:
                        allowedGameTypes = new List<GameType>() { GameType.PlayCasual, GameType.PlayRanked };
                        if (( this.game.Type.HasValue) && (! allowedGameTypes.Contains(this.Game.Type.Value)))
                        {
                            this.Game.Type = null;
                            if (savedState != null)
                            {
                                savedState.Type = null;
                            }
                        }
                        break;
                    case DeckType.VersusArena:
                        this.Game.Type = GameType.VersusArena;
                        if (savedState != null)
                        {
                            savedState.Type = GameType.VersusArena;
                        }
                        allowedGameTypes = new List<GameType>() { GameType.VersusArena };
                        break;
                    case DeckType.SoloArena:
                        this.Game.Type = GameType.SoloArena;
                        if (savedState != null)
                        {
                            savedState.Type = GameType.SoloArena;
                        }
                        allowedGameTypes = new List<GameType>() { GameType.SoloArena };
                        break;
                    default:
                        break;
                }
            }
            else 
            {
                if (savedState != null)
                {
                    savedState.Type = null;
                }
            }
            if (allowedGameTypes == null)
            {
                allowedGameTypes = new List<GameType>(Enum.GetValues(typeof(GameType)).Cast<GameType>());
            }
            return allowedGameTypes;
        }

        public void ShowWinsVsClass(DeckClass? deckClass)
        {
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

        private void RefreshTagsList(NewDeckTagCreated obj)
        {
            RaisePropertyChangedEvent(nameof(OpponentDeckTagAutocomplete));
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
                    new EditDeck() { Deck = game.Deck },
                    EditDeck.Context.StatsUpdated);

                trackerFactory.GetFileManager().SaveDatabase();
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
            Game.DeckVersionId = savedState.DeckVersionId;
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
            Game.OpponentDeckTag = savedState.OpponentDeckTag;
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
