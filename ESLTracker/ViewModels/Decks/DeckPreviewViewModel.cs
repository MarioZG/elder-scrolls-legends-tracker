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
        public Deck Deck {get;set;}
        public DeckVersion CurrentVersion
        {
            get
            {
                return Deck?.History.Where(dh => dh.VersionId == Deck.SelectedVersionId).First();
            }
        }
    }
}
