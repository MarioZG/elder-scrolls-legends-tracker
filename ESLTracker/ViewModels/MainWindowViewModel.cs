using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
