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

        IMessenger messanger;

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


        public DeckListViewModel() : this (new TrackerFactory())
        {
        }

        public DeckListViewModel(ITrackerFactory factory)
        {
            this.messanger = factory.GetMessanger();
            messanger.Register<DeckListFilterChanged>(this, DeckTypeFilterChanged, DeckListFilterChanged.Context.TypeFilterChanged);

            FilteredDecks = new ObservableCollection<Deck>(Tracker.Instance.Decks);
        }

        private void DeckTypeFilterChanged(DeckListFilterChanged obj)
        {
            lastDeckTypeFilter = obj.Filter;
            ApplyFilter(obj.Filter.FilteredTypes, obj.Filter.ShowCompletedArenaRuns);
        }

        private void DeckListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedClass")
            {
                ApplyFilter(lastDeckTypeFilter.FilteredTypes, lastDeckTypeFilter.ShowCompletedArenaRuns);
            }
        }

        private void ApplyFilter(
            IEnumerable<DeckType> filteredTypes, 
            bool showCompletedArenaRuns)
        {
            Deck activeDeck = Tracker.Instance.ActiveDeck; //we loose it when FilteredDecks.Clear();
            FilteredDecks.Clear();

            FilterDeckList(
                    Tracker.Instance.Decks, 
                    filteredTypes, 
                    showCompletedArenaRuns,
                    this.classFilterViewModel != null ? this.classFilterViewModel.SelectedClass : null,
                    this.classFilterViewModel != null ? this.classFilterViewModel.FilteredClasses.ToList() : ClassAttributesHelper.Classes.Keys.ToList())
                .All(d => { FilteredDecks.Add(d); return true; });

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
            IEnumerable<DeckType> filteredTypes,
            bool showCompletedArenaRuns,
            DeckClass? selectedClass,
            IEnumerable<DeckClass> filteredClassed)
        {
            IEnumerable<Deck> filteredList;
            if (selectedClass.HasValue)
            {
                //specific class slected (there might nbe more in filteredclasses propery!!!
                filteredList = deckBase.Where(d =>
                    (d.Class == selectedClass)
                    && ((filteredTypes == null) || (filteredTypes.Contains(d.Type)))
                    && ((showCompletedArenaRuns) || (! d.IsArenaRunFinished()))
                    );
                return filteredList;
            }
            else
            {
                //filter by attributes
                filteredList = deckBase.Where(d =>
                    d.Class.HasValue
                    && (filteredClassed == null || filteredClassed.Contains(d.Class.Value))
                    && ((filteredTypes == null) || (filteredTypes.Contains(d.Type)))
                    && ((showCompletedArenaRuns) || (!d.IsArenaRunFinished()))
                    );
                return filteredList;
            }


        }

        public void CommandResetFiltersExecute(object param)
        {
            this.classFilterViewModel.Reset();
            messanger.Send<DeckListFilterChanged>(null, DeckListFilterChanged.Context.ResetAllFilters);
            ApplyFilter(null, lastDeckTypeFilter != null ? lastDeckTypeFilter.ShowCompletedArenaRuns : false);
        }

        private void CommandEditDeckExecute(object param)
        {
            //inform other views that we are about to edit deck
            messanger.Send(
                new EditDeck() { Deck = Tracker.Instance.ActiveDeck }, 
                EditDeck.Context.StartEdit );
        }
    }
}
