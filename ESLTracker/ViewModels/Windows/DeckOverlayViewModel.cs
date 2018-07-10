using ESLTracker.DataModel;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels.Windows
{
    public class DeckOverlayViewModel : ViewModelBase
    {
        ITracker tracker;

        public DeckOverlayViewModel(ITracker tracker)
        {
            this.tracker = tracker;
        }


        public PropertiesObservableCollection<CardInstance> ActiveDeckCards () => tracker.ActiveDeck.SelectedVersion.Cards;
    }
}
