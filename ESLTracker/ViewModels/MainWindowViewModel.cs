using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Services;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using ESLTracker.Utils.NLog;
using ESLTracker.ViewModels.Decks;

namespace ESLTracker.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool deckEditVisible;

        public bool DeckEditVisible
        {
            get { return deckEditVisible; }
            set
            {
                deckEditVisible = value;
                RaisePropertyChangedEvent("DeckEditVisible");
            }
        }

        private bool deckListVisible = true;

        public bool DeckListVisible
        {
            get { return deckListVisible; }
            set { deckListVisible = value; RaisePropertyChangedEvent("DeckListVisible"); }
        }

        private bool deckPreviewVisible = true;

        public bool DeckPreviewVisible
        {
            get { return deckPreviewVisible; }
            set { deckPreviewVisible = value; RaisePropertyChangedEvent(nameof(DeckPreviewVisible)); }
        }

        private bool settingsVisible = false;

        public bool SettingsVisible
        {
            get { return settingsVisible; }
            set { settingsVisible = value; RaisePropertyChangedEvent("SettingsVisible"); }
        }

        private bool editGameVisible = false;

        public bool EditGameVisible
        {
            get { return editGameVisible; }
            set { editGameVisible = value; RaisePropertyChangedEvent("EditGameVisible"); }
        }

        private bool allowCommands = true;

        public bool AllowCommands
        {
            get { return allowCommands; }
            set { allowCommands = value; RaisePropertyChangedEvent("AllowCommands"); }
        }

        private bool showInTaskBar = true;

        public bool ShowInTaskBar
        {
            get { return showInTaskBar; }
            set { showInTaskBar = value; RaisePropertyChangedEvent("ShowInTaskBar"); }
        }

        private WindowState windowState;

        public WindowState WindowState
        {
            get { return windowState; }
            set { windowState = value; RaisePropertyChangedEvent("WindowState"); }
        }

        public List<DismissableMessage> UserInfo
        {
            get {
                var list = ((Utils.NLog.UserInfoLoggerTarget)NLog.LogManager.Configuration.FindTargetByName(App.UserInfoLogger)).Logs;
                list.CollectionChanged += (sender, collection) => { RaisePropertyChangedEvent(nameof(UserInfo)); };
                return list.ToList();
            }
        }

        public OverlayWindowRepository OverlayWindows { get; set; } = new OverlayWindowRepository();

        #region Commands
        public ICommand CommandEditSettings
        {
            get { return new RelayCommand(new Action<object>(EditSettings)); }
        }

        public ICommand CommandNotifyIconLeftClick
        {
            get { return new RelayCommand(new Action<object>(NotifyIconLeftClick)); }
        }

        public ICommand CommandExit
        {
            get { return new RelayCommand(new Action<object>(Exit)); }
        }

        public ICommand CommandShowRewards
        {
            get { return new RelayCommand(new Action<object>(ShowRewards)); }
        }

        public ICommand CommandNewDeck
        {
            get { return new RelayCommand(new Action<object>(NewDeck)); }
        }

        public ICommand CommandShowOverlay
        {
            get { return new RelayCommand(new Action<object>(ShowOverlay)); }
        }

        public IAsyncCommand CommandRunGame
        {
            get
            {
                return new RealyAsyncCommand<object>(
                    new Func<object,Task<object>>(CommandRunGameExecute),
                    new Func<object, bool>(CommandRunGameCanExecute)
                    );
            }
        }

        public ICommand CommandShowArenaStats
        {
            get { return new RelayCommand(new Action<object>(CommandShowArenaStatsExecute)); }
        }

        public ICommand CommandShowGamesStats
        {
            get { return new RelayCommand(new Action<object>(CommandShowGamesStatsExecute)); }
        }

        public ICommand CommandShowRankedProgress
        {
            get { return new RelayCommand(new Action<object>(CommandShowRankedProgressExecute)); }
        }

        public ICommand CommandEditDeck
        {
            get
            {
                return new RelayCommand(
                    new Action<object>(CommandEditDeckExecute),
                    CommandEditDeckCanExecute);
            }
        }

        public ICommand CommandAbout
        {
            get { return new RelayCommand(new Action<object>(CommandAboutExecute)); }
        }

        public ICommand CommandManageOverlayWindow
        {
            get
            {
                return new RelayCommand(new Action<object>(CommandManageOverlayWindowExecute));
            }
        }

        public ICommand CommandHideDeck
        {
            get
            {
                return new RelayCommand(
                        (object param) => messanger.Send(
                                                new EditDeck() { Deck = tracker.ActiveDeck },
                                                EditDeck.Context.Hide),
                          (object param) => deckService.CommandHideDeckCanExecute(tracker.ActiveDeck));
            }
        }

        public ICommand CommandUnHideDeck
        {
            get
            {
                return new RelayCommand(
                        (object param) => messanger.Send(
                                                new EditDeck() { Deck = tracker.ActiveDeck },
                                                EditDeck.Context.UnHide),
                      (object param) => deckService.CommandUnHideDeckCanExecute(tracker.ActiveDeck));
            }
        }

        public ICommand CommandDeleteDeck
        {
            get
            {
                return new RelayCommand(
                            (object param) => messanger.Send(
                                                new EditDeck() { Deck = tracker.ActiveDeck },
                                                EditDeck.Context.Delete),
                        (object param) => deckService.CanDelete(tracker.ActiveDeck));
            }
        }


        #endregion

        ITrackerFactory trackerFactory;
        IMessenger messanger;
        ITracker tracker;
        IDeckService deckService;
        ISettings settings;
        IWinAPI winApi;

        public MainWindowViewModel() : this(new TrackerFactory())
        {
        }

        internal MainWindowViewModel(TrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            tracker = trackerFactory.GetTracker();
            messanger = trackerFactory.GetService<IMessenger>();
            messanger.Register<Utils.Messages.EditDeck>(this, EditDeckStart, Utils.Messages.EditDeck.Context.StartEdit);
            messanger.Register<Utils.Messages.EditDeck>(this, EditDeckFinished, Utils.Messages.EditDeck.Context.EditFinished);
            messanger.Register<Utils.Messages.EditGame>(this, EditGameStart, Utils.Messages.EditGame.Context.StartEdit);
            messanger.Register<Utils.Messages.EditGame>(this, EditGameFinished, Utils.Messages.EditGame.Context.EditFinished);
            messanger.Register<Utils.Messages.EditSettings>(this, EditSettingsFinished, Utils.Messages.EditSettings.Context.EditFinished);

            deckService = trackerFactory.GetService<IDeckService>();
            settings = trackerFactory.GetService<ISettings>();
            winApi = trackerFactory.GetService<IWinAPI>();


            this.OverlayWindows.Add(new OverlayToolbar());
            this.OverlayWindows.Add(new DeckOverlay());
            this.OverlayWindows.CollectionChanged += (s, e) => RaisePropertyChangedEvent(nameof(OverlayWindows));
        }

        public void NotifyIconLeftClick(object parameter)
        {
            if ((WindowState == WindowState.Normal) && (parameter as string != "show"))
            {
                WindowState = WindowState.Minimized;
                ShowInTaskBar = false;
            }
            else
            {
                if (WindowState == WindowState.Normal)
                {
                    //force change for context menu option
                    WindowState = WindowState.Minimized;
                }
                ShowInTaskBar = true;
                WindowState = WindowState.Normal;
            }
        }

        public void Exit(object parameter)
        {
            bool checkIfCanClose = false;
            if (parameter !=null && parameter is bool)
            {
                checkIfCanClose = (bool)parameter;
            }
            OverlayToolbar ot = this.OverlayWindows.GetWindowByType<OverlayToolbar>();
            if (!checkIfCanClose || (checkIfCanClose && ot.CanClose(CommandExit)))
            {
                trackerFactory.GetFileManager().SaveDatabase();
                MainWindow.UpdateOverlay = false;
                ot.Close();
                settings.LastActiveDeckId = tracker.ActiveDeck?.DeckId;
                settings.Save();
                ((App)Application.Current).CloseApplication();
            }
        }

        public void ShowRewards(object parameter)
        {
            new RewardsSummary().Show();
        }

        public void NewDeck(object parameter)
        {
            messanger.Send(
                new Utils.Messages.EditDeck() { Deck = Deck.CreateNewDeck("New deck") },
                Utils.Messages.EditDeck.Context.StartEdit
                );
        }

        public void ShowOverlay(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).RestoreOverlay();
        }

        bool startingGame;
        public async Task<object> CommandRunGameExecute(object parameter)
        {
            startingGame = true;
            CommandManager.InvalidateRequerySuggested();

            var proc = trackerFactory.GetService<ILauncherService>().StartGame(this.winApi, this.messanger);

            startingGame = false;
            CommandManager.InvalidateRequerySuggested();
            return null;
        }

        private bool CommandRunGameCanExecute(object arg)
        {
            return (! startingGame)
                && ( winApi.GetEslProcess() == null);
        }

        public void EditSettings(object parameter)
        {
           // this.DeckStatsVisible = false;
            this.SettingsVisible = true;
            this.AllowCommands = false;

        }


        private void EditSettingsFinished(EditSettings obj)
        {
            this.SettingsVisible = false;
            this.AllowCommands = true;
        }

        private void EditGameStart(EditGame obj)
        {
            this.EditGameVisible = true;
            this.DeckPreviewVisible = false;
            this.AllowCommands = false;
        }

        private void EditGameFinished(EditGame obj)
        {
            this.EditGameVisible = false;
            this.DeckPreviewVisible = true;
            this.AllowCommands = true;
        }

        private void CommandShowArenaStatsExecute(object obj)
        {
            new ArenaStats().Show();
        }


        private void CommandShowGamesStatsExecute(object obj)
        {
            new GameStatistics().Show();
        }

        private void CommandShowRankedProgressExecute(object obj)
        {
            new RankedProgressChart().Show();
        }

        private bool CommandEditDeckCanExecute(object arg)
        {
            return tracker.ActiveDeck != null;
        }

        private void CommandEditDeckExecute(object obj)
        {
            messanger.Send(
                new EditDeck() { Deck = tracker.ActiveDeck },
                EditDeck.Context.StartEdit);
        }

        private void CommandAboutExecute(object obj)
        {
            new About().ShowDialog();
        }

        private void EditDeckStart(Utils.Messages.EditDeck obj)
        {
            this.DeckEditVisible = true;
            this.DeckPreviewVisible = false;
            this.DeckListVisible = false;
            this.AllowCommands = false;
        }

        private void EditDeckFinished(EditDeck obj)
        {
            this.DeckEditVisible = false;
            this.DeckPreviewVisible = true;
            this.DeckListVisible = true;
            this.AllowCommands = true;
        }

        private void CommandManageOverlayWindowExecute(object obj)
        {
            var window = OverlayWindows.GetWindowByType((Type)obj);
            ((IOverlayWindow)window).ShowOnScreen = !((IOverlayWindow)window).ShowOnScreen;
            RaisePropertyChangedEvent(nameof(OverlayWindows));
        }

    }
}
