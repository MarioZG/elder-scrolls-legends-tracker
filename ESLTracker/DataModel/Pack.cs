using System;
using System.Collections.ObjectModel;
using ESLTracker.ViewModels;

namespace ESLTracker.DataModel
{
    public class Pack : ViewModelBase
    {
        public ObservableCollection<CardInstance> Cards { get; set; }

        public DateTime DateOpened { get; set; }

        private CardSet cardSet;
        public CardSet CardSet
        {
            get { return cardSet; }
            set { SetProperty<CardSet>(ref cardSet, value); }
        }

        [Obsolete("Use factory in production code or deckbuilder in unit tests to create new packs")]
        public Pack()
        {
        }

    }
}
