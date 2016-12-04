using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.ViewModels.Decks;

namespace ESLTracker.Utils.Messages
{
    public class DeckListFilterChanged
    {
        public enum Context
        {
            TypeFilterChanged,
            ResetAllFilters
        }

        public DeckListFilterChanged(DeckTypeSelectorViewModel deckTypeSelectorViewModel)
        {
            this.Filter = deckTypeSelectorViewModel;
        }

        public IDeckTypeSelectorViewModel Filter { get; set; }

    }
}
