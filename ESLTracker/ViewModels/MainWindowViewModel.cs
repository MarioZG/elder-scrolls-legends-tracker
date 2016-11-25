using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.Properties;
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
                //DeckListVisible = ! value;
                RaisePropertyChangedEvent("DeckEditVisible");
            }
        }

        private bool deckListVisible = true;

        public bool DeckListVisible
        {
            get { return deckListVisible; }
            set { deckListVisible = value; RaisePropertyChangedEvent("DeckListVisible"); }
        }

        private bool deckStatsVisible = true;

        public bool DeckStatsVisible
        {
            get { return deckStatsVisible; }
            set { deckStatsVisible = value; RaisePropertyChangedEvent("DeckStatsVisible"); }
        }

        private bool settingsVisible = false;

        public bool SettingsVisible
        {
            get { return settingsVisible; }
            set { settingsVisible = value; RaisePropertyChangedEvent("SettingsVisible"); }
        }

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
            get { return new RelayCommandWithSettings(new Action<object, ISettings>(Exit), Properties.Settings.Default); }
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

        public ICommand CommandRunGame
        {
            get { return new RelayCommand(new Action<object>(RunGame)); }
        }

        public MainWindowViewModel()
        {
            Utils.Messenger.Default.Register<Utils.Messages.EditDeck>(this, EditDeck, Utils.Messages.EditDeck.Context.StartEdit);
            Utils.Messenger.Default.Register<Utils.Messages.EditDeck>(this, EditDeck, Utils.Messages.EditDeck.Context.Saved);
            Utils.Messenger.Default.Register<Utils.Messages.EditDeck>(this, EditDeck, Utils.Messages.EditDeck.Context.Cancel);
        }

        public void NotifyIconLeftClick(object parameter)
        {
            if (parameter is Window)
            {
                Window w = parameter as Window;
                w.WindowState = WindowState.Normal;
                w.ShowInTaskbar = true;
                w.Activate();
                w.Focus();
            }
        }

        public void Exit(object parameter, ISettings settings)
        {
            Utils.FileManager.SaveDatabase();
            MainWindow.UpdateOverlay = false;
            settings.LastActiveDeckId = Tracker.Instance.ActiveDeck?.DeckId;
            settings.Save();
            ((App)Application.Current).Exit();
        }

        public void ShowRewards(object parameter)
        {
            new RewardsSummary().Show();
        }

        public void NewDeck(object parameter)
        {
            Utils.Messenger.Default.Send(
                new Utils.Messages.EditDeck() { Deck = EditDeckViewModel.CreateDefaultDeck() },
                Utils.Messages.EditDeck.Context.StartEdit
                );
        }

        public void ShowOverlay(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).RestoreOverlay();
        }

        public void RunGame(object parameter)
        {
            if (WindowsUtils.GetEslProcess() == null)
            {
                System.Diagnostics.Process.Start("bethesdanet://run/5");
            }
        }

        public void EditSettings(object parameter)
        {
            this.DeckStatsVisible = false;
            this.SettingsVisible = true;

        }

        private void EditDeck(Utils.Messages.EditDeck obj)
        {
            this.DeckEditVisible = !this.DeckEditVisible;
        }
    }
}
