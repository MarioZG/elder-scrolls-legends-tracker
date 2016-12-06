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

        DeckListFilterChanged lastDeckFilter = new DeckListFilterChanged(
            DeckListFilterChanged.Source.Unknown, 
            null, 
            false, 
            null,
            ClassAttributesHelper.Classes.Keys.ToList()); //default to all classes

        IMessenger messanger;
        ITracker tracker;

        //command for filter toggle button pressed
        public ICommand CommandResetFilterButtonPressed
        {
            get { return new RelayCommand(new Action<object>(CommandResetFiltersExecute)); }
        }

        public ICommand CommandEditDeck
        {
            get { return new RelayCommand(new Action<object>(CommandEditDeckExecute)); }
        }

        public DeckListViewModel() : this (new TrackerFactory())
        {
        }

        public DeckListViewModel(ITrackerFactory factory)
        {
            this.messanger = factory.GetMessanger();
            messanger.Register<DeckListFilterChanged>(this, DeckFilterChanged, ControlMessangerContext.DeckList_DeckFilterControl);
            messanger.Register<DeckListFilterChanged>(this, DeckFilterChanged, ControlMessangerContext.DeckList_DeckFilterControl);
            messanger.Register<EditDeck>(this, EditDeckFinished, Utils.Messages.EditDeck.Context.EditFinished);

            tracker = factory.GetTracker();
            FilteredDecks = new ObservableCollection<Deck>(tracker.Decks);
        }

        private void DeckFilterChanged(DeckListFilterChanged obj)
        {
            //new filters object, that merges current values if != null
            lastDeckFilter = new DeckListFilterChanged(
                DeckListFilterChanged.Source.Unknown,
                obj.ChangeSource ==  DeckListFilterChanged.Source.TypeFilter ? obj.FilteredTypes :  lastDeckFilter.FilteredTypes,
                obj.ChangeSource == DeckListFilterChanged.Source.TypeFilter ? obj.ShowFInishedArenaRuns: lastDeckFilter.ShowFInishedArenaRuns,
                obj.ChangeSource == DeckListFilterChanged.Source.ClassFilter ? obj.SelectedClass : lastDeckFilter.SelectedClass, 
                obj.ChangeSource == DeckListFilterChanged.Source.ClassFilter ? obj.FilteredClasses : lastDeckFilter.FilteredClasses);

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Deck activeDeck = tracker.ActiveDeck; //we loose it when FilteredDecks.Clear();
            FilteredDecks.Clear();

            FilterDeckList(
                    tracker.Decks,
                    this.lastDeckFilter.FilteredTypes,
                    this.lastDeckFilter.ShowFInishedArenaRuns,
                    this.lastDeckFilter.SelectedClass,
                    this.lastDeckFilter.FilteredClasses)
                .All(d => { FilteredDecks.Add(d); return true; });

            if (!FilteredDecks.Contains(activeDeck))
            {
                tracker.ActiveDeck = null;
            }
            else
            {
                tracker.ActiveDeck = activeDeck;
            }
        }

        public IEnumerable<Deck> FilterDeckList(
            IEnumerable<Deck> deckBase,
            IEnumerable<DeckType> filteredTypes,
            bool showCompletedArenaRuns,
            DeckClass? selectedClass,
            IEnumerable<DeckClass> filteredClasses)
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
                    && (filteredClasses == null || filteredClasses.Contains(d.Class.Value))
                    && ((filteredTypes == null) || (filteredTypes.Contains(d.Type)))
                    && ((showCompletedArenaRuns) || (!d.IsArenaRunFinished()))
                    );
                return filteredList;
            }


        }

        public void CommandResetFiltersExecute(object param)
        {
            messanger.Send<DeckListResetFilters>(new DeckListResetFilters(), ControlMessangerContext.DeckList_DeckFilterControl);
            ApplyFilter();
        }

        private void CommandEditDeckExecute(object param)
        {
            //inform other views that we are about to edit deck
            messanger.Send(
                new EditDeck() { Deck = tracker.ActiveDeck }, 
                EditDeck.Context.StartEdit );
        }

        private void EditDeckFinished(EditDeck obj)
        {
            ApplyFilter();
        }
    }
}
