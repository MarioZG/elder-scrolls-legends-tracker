using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public DeckVersion CurrentVersion
        {
            get
            {
                return Deck?.History.Where(dh => dh.VersionId == Deck.SelectedVersionId).First();
            }
            set { }
        }
    }
}
