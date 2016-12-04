using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckListViewModel : ViewModelBase
    {
        public ObservableCollection<Deck> FilteredDecks { get; set; }

        IDeckTypeSelectorViewModel lastDeckTypeFilter;

        //command for filter toggle button pressed
        public ICommand CommandResetFilterButtonPressed
        {
            get { return new RelayCommand(new Action<object>(CommandResetFiltersExecute)); }
        }

        public ICommand CommandEditDeck
        {
            get { return new RelayCommand(new Action<object>(CommandEditDeckExecute)); }
        }

        private IDeckClassSelectorViewModel classFilterViewModel;

        public void SetClassFilterViewModel(IDeckClassSelectorViewModel cfvm)
        {
            classFilterViewModel = cfvm;
            classFilterViewModel.PropertyChanged += DeckListViewModel_PropertyChanged;
        }

        public DeckListViewModel()
        {
            Messenger.Default.Register<DeckListFilterChanged>(this, DeckTypeFilterChanged, DeckListFilterChanged.Context.TypeFilterChanged);

            FilteredDecks = new ObservableCollection<Deck>(Tracker.Instance.Decks);
        }

        private void DeckTypeFilterChanged(DeckListFilterChanged obj)
        {
            lastDeckTypeFilter = obj.Filter;
            ApplyFilter(obj.Filter);
        }

        private void DeckListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedClass")
            {
                ApplyFilter(lastDeckTypeFilter);
            }
        }

        private void ApplyFilter(IDeckTypeSelectorViewModel deckTypeFilter)
        {
            Deck activeDeck = Tracker.Instance.ActiveDeck; //we loose it when FilteredDecks.Clear();
            FilteredDecks.Clear();

            FilterDeckList(Tracker.Instance.Decks, deckTypeFilter).All(d => { FilteredDecks.Add(d); return true; });

            if (!FilteredDecks.Contains(activeDeck))
            {
                Tracker.Instance.ActiveDeck = null;
            }
            else
            {
                Tracker.Instance.ActiveDeck = activeDeck;
            }
        }

        public IEnumerable<Deck> FilterDeckList(
            IEnumerable<Deck> deckBase,
            IDeckTypeSelectorViewModel deckTypeFilter)
        {
            IEnumerable<Deck> filteredList;
            if (this.classFilterViewModel.SelectedClass != null)
            {
                //specific class slected (there might nbe more in filteredclasses propery!!!
                filteredList = deckBase.Where(d =>
                    (d.Class == this.classFilterViewModel.SelectedClass)
                    && ((deckTypeFilter == null) || (deckTypeFilter.FilteredTypes.Contains(d.Type)))
                    && ((deckTypeFilter == null) || (deckTypeFilter.ShowCompletedArenaRuns) || (! d.IsArenaRunFinished()))
                    );
                return filteredList;
            }
            else
            {
                //filter by attributes
                filteredList = deckBase.Where(d =>
                    d.Class.HasValue
                    && (this.classFilterViewModel.FilteredClasses.Contains(d.Class.Value))
                    && ((deckTypeFilter == null) || (deckTypeFilter.FilteredTypes.Contains(d.Type)))
                    && ((deckTypeFilter == null) || (deckTypeFilter.ShowCompletedArenaRuns) || (!d.IsArenaRunFinished()))
                    );
                return filteredList;
            }


        }

        public void CommandResetFiltersExecute(object param)
        {
            this.classFilterViewModel.Reset();
            Messenger.Default.Send<DeckListFilterChanged>(null, DeckListFilterChanged.Context.ResetAllFilters);
            lastDeckTypeFilter = null;
            ApplyFilter(lastDeckTypeFilter);
        }

        private void CommandEditDeckExecute(object param)
        {
            //inform other views that we are about to edit deck
            Utils.Messenger.Default.Send(
                new EditDeck() { Deck = Tracker.Instance.ActiveDeck }, 
                EditDeck.Context.StartEdit );
        }
    }
}
