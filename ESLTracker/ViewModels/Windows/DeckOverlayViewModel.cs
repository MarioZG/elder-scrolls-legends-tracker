using TESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.Utils;

namespace ESLTracker.ViewModels.Windows
{
    public class DeckOverlayViewModel : ViewModelBase
    {
        public PropertiesObservableCollection<CardInstance> ActiveDeckCards
        {
            get
            {
                return tracker?.ActiveDeck?.SelectedVersion?.Cards;
            }
        }

        private readonly ITracker tracker;
        private readonly IMessenger messanger;

        public DeckOverlayViewModel(ITracker tracker, IMessenger messanger)
        {
            this.tracker = tracker;
            this.messanger = messanger;

            messanger.Register<ActiveDeckChanged>(this, OnActiveDeckChanged);
        }

        private void OnActiveDeckChanged(ActiveDeckChanged obj)
        {
            RaisePropertyChangedEvent(nameof(ActiveDeckCards));
        }

    }
}
