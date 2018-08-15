using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.GameClient;
using ESLTracker.Controls;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using ESLTracker.Utils.NLog;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels.Decks;
using ESLTracker.Windows;

namespace ESLTracker.ViewModels.Windows
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

        public UserInfoMessages UserInfo { get; private set; }

        public OverlayWindowRepository OverlayWindows { get; set; }

        public Deck ActiveDeck
        {
            get
            {
                return tracker.ActiveDeck;
            }
        }

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

        public IAsyncCommand<object> CommandRunGame
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

        public ICommand CommandShowPackOpening
        {
            get { return new RelayCommand(new Action<object>(CommandShowPackOpeningExecute)); }
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
        #endregion

        IMessenger messanger;
        ITracker tracker;
        IDeckService deckService;
        ISettings settings;
        IWinAPI winApi;
        IFileSaver fileSaver;
        ILauncherService launcherService;

        public MainWindowViewModel(
            ITracker tracker, 
            IMessenger messanger,
            IDeckService deckService,
            ISettings settings,
            IWinAPI winApi,
            IFileSaver fileSaver,
            ILauncherService launcherService,
            OverlayWindowRepository overlayWindows,
            UserInfoMessages userInfoMessages
            )
        {
            this.tracker = tracker;
            this.messanger = messanger;
            this.OverlayWindows = overlayWindows;
            this.UserInfo = userInfoMessages;

            messanger.Register<Utils.Messages.EditDeck>(this, EditDeckStart, Utils.Messages.EditDeck.Context.StartEdit);
            messanger.Register<Utils.Messages.EditDeck>(this, EditDeckFinished, Utils.Messages.EditDeck.Context.EditFinished);
            messanger.Register<Utils.Messages.EditGame>(this, EditGameStart, Utils.Messages.EditGame.Context.StartEdit);
            messanger.Register<Utils.Messages.EditGame>(this, EditGameFinished, Utils.Messages.EditGame.Context.EditFinished);
            messanger.Register<Utils.Messages.EditSettings>(this, EditSettingsFinished, Utils.Messages.EditSettings.Context.EditFinished);
            messanger.Register<ActiveDeckChanged>(this, ActiveDeckChanged);

            this.deckService = deckService;
            this.settings = settings;
            this.winApi = winApi;
            this.fileSaver = fileSaver;
            this.launcherService = launcherService;

            this.OverlayWindows.CollectionChanged += (s, e) => RaisePropertyChangedEvent(nameof(OverlayWindows));
        }

        internal void ActiveDeckChanged(ActiveDeckChanged activeDeckChanged)
        {
            RaisePropertyChangedEvent(nameof(ActiveDeck));
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
                fileSaver.SaveDatabase(tracker);
                OverlayWindows.UpdateOverlay = false;
                ot.Close();
                settings.LastActiveDeckId = tracker.ActiveDeck?.DeckId;
                settings.Save();
                ((App)Application.Current).CloseApplication();
            }
        }

        public void ShowRewards(object parameter)
        {
            MasserContainer.Container.GetInstance<RewardsSummary>().Show();
        }

        bool startingGame;
        public async Task<object> CommandRunGameExecute(object parameter)
        {
            startingGame = true;
            CommandManager.InvalidateRequerySuggested();

            var proc = await launcherService.StartGame();

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

        private void CommandShowPackOpeningExecute(object obj)
        {
            new ESLTracker.Controls.PackStatistics.OpeningPackStatsWindow().Show();
        }

        private void CommandAboutExecute(object obj)
        {
            MasserContainer.Container.GetInstance<About>().ShowDialog();
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
            this.tracker.ActiveDeck = obj.Deck;

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
