using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckPreviewViewModel : ViewModelBase
    {

        private Deck deck;

        public Deck Deck
        {
            get { return deck; }
            set {
                deck = value;
                RaisePropertyChangedEvent(nameof(Deck));
                RaisePropertyChangedEvent(nameof(CurrentVersion));
            }
        }

        private bool isInEditMode = false;

        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set { isInEditMode = value; RaisePropertyChangedEvent(nameof(IsInEditMode)); }
        }

        public ICommand CommandEditMode
        {
            get
            {
                return new RelayCommand(CommandEditModeExecute);
            }
        }

        public DeckVersion CurrentVersion
        {
            get
            {
                return Deck?.History.Where(dh => dh.VersionId == Deck.SelectedVersionId).First();
            }
        }

        private void CommandEditModeExecute(object obj)
        {
            this.IsInEditMode = !IsInEditMode;
        }
    }
}
