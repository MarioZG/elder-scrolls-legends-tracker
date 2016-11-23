using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ESLTracker.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool deckEditVisible;

        public bool DeckEditVisible
        {
            get { return deckEditVisible; }
            set {
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

        private bool deckStatsVisible = false;

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


        public void EditSettings(object parameter)
        {
            this.DeckStatsVisible = false;
            this.SettingsVisible = true;

        }
    }
}
