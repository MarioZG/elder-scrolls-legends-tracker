using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Properties;
using ESLTracker.Services;
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
            false,
            null,
            ClassAttributesHelper.Classes.Keys.ToList()); //default to all classes

        public Deck SelectedDeck
        {
            get {
                return tracker.ActiveDeck;
            }
            set
            {
                tracker.ActiveDeck = value;
            }
        }

        private string deckTextSearch;
        public string DeckTextSearch
        {
            get
            {
                return DeckTextSearchEntered ? deckTextSearch : "type deck or card name to filter";
            }
            set
            {
                if (DeckTextSearchEntered)
                {
                    SetProperty<string>(ref deckTextSearch, value);
                    ApplyFilter();
                }
                else
                {
                    DeckTextSearchEntered = true;
                }
            }
        }

        private bool deckTextSearchEntered;
        public bool DeckTextSearchEntered
        {
            get { return deckTextSearchEntered; }
            set { deckTextSearchEntered = value; RaisePropertyChangedEvent(); RaisePropertyChangedEvent(nameof(DeckTextSearch)); }
        }

        #region Commands
        //command for filter toggle button pressed
        public ICommand CommandResetFilterButtonPressed
        {
            get { return new RelayCommand(new Action<object>(CommandResetFiltersExecute)); }
        }

        public ICommand CommandEditDeck
        {
            get { return new RelayCommand(new Action<object>(CommandEditDeckExecute)); }
        }

        public ICommand CommandHideDeck
        {
            get {
                return new RelayCommand(
                          (object param) => CommandHideDeckExecute(new EditDeck() { Deck = SelectedDeck }),
                          (object param) => deckService.CommandHideDeckCanExecute(SelectedDeck));
            }
        }

        public ICommand CommandUnHideDeck
        {
            get { return new RelayCommand(
                    (object param) => CommandUnHideDeckExecute(new EditDeck() { Deck = SelectedDeck }),
                    (object param) => deckService.CommandUnHideDeckCanExecute(SelectedDeck));
            }
        }

        public ICommand CommandDeleteDeck
        {
            get
            {
                return new RelayCommand(
                    (object param) => CommandDeleteDeckExecute(new EditDeck() { Deck = SelectedDeck }),
                    (object param) => deckService.CanDelete(SelectedDeck));
            }
        }
        #endregion

        private readonly IMessenger messanger;
        private readonly ITracker tracker;
        private readonly IDeckService deckService;
        private readonly ISettings settings;
        private readonly IFileManager fileManager;
        private readonly DeckCalculations deckCalculations;

        public DeckListViewModel(
            IMessenger messanger,
            ITracker tracker,
            IDeckService deckService,
            ISettings settings,
            IFileManager fileManager,
            DeckCalculations deckCalculations)
        {
            this.messanger = messanger;
            messanger.Register<DeckListFilterChanged>(this, DeckFilterChanged, ControlMessangerContext.DeckList_DeckFilterControl);
            messanger.Register<EditDeck>(this, EditDeckFinished, Utils.Messages.EditDeck.Context.EditFinished);
            messanger.Register<EditDeck>(this, CommandHideDeckExecute, Utils.Messages.EditDeck.Context.Hide);
            messanger.Register<EditDeck>(this, CommandUnHideDeckExecute, Utils.Messages.EditDeck.Context.UnHide);
            messanger.Register<EditDeck>(this, CommandDeleteDeckExecute, Utils.Messages.EditDeck.Context.Delete);

            this.tracker = tracker;
            FilteredDecks = new ObservableCollection<Deck>(tracker.Decks);

            this.deckService = deckService;
            this.settings = settings;
            this.fileManager = fileManager;
            this.deckCalculations = deckCalculations;
        }

        private void DeckFilterChanged(DeckListFilterChanged obj)
        {
            //new filters object, that merges current values if != null
            lastDeckFilter = new DeckListFilterChanged(
                DeckListFilterChanged.Source.Unknown,
                obj.ChangeSource ==  DeckListFilterChanged.Source.TypeFilter ? obj.FilteredTypes :  lastDeckFilter.FilteredTypes,
                obj.ChangeSource == DeckListFilterChanged.Source.TypeFilter ? obj.ShowFInishedArenaRuns: lastDeckFilter.ShowFInishedArenaRuns,
                obj.ChangeSource == DeckListFilterChanged.Source.TypeFilter ? obj.ShowHiddenDecks : lastDeckFilter.ShowHiddenDecks,
                obj.ChangeSource == DeckListFilterChanged.Source.ClassFilter ? obj.SelectedClass : lastDeckFilter.SelectedClass, 
                obj.ChangeSource == DeckListFilterChanged.Source.ClassFilter ? obj.FilteredClasses : lastDeckFilter.FilteredClasses);

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            //System.Diagnostics.Debugger.Launch();
            Deck activeDeck = tracker.ActiveDeck; //we loose it when FilteredDecks.Clear();
            FilteredDecks.Clear();

            var filteredDecks = FilterDeckList(
                    tracker.Decks,
                    this.lastDeckFilter.FilteredTypes,
                    this.lastDeckFilter.ShowFInishedArenaRuns,
                    this.lastDeckFilter.ShowHiddenDecks,
                    this.lastDeckFilter.SelectedClass,
                    this.lastDeckFilter.FilteredClasses, 
                    this.deckTextSearch);

            DeckViewSortOrder sortOrder = settings.DeckViewSortOrder;

            filteredDecks = filteredDecks.OrderBy( d=> GetPropertyValue(d, sortOrder.ToString()));

            filteredDecks.All(d => { FilteredDecks.Add(d); return true; });

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
            bool showHiddenDecks,
            DeckClass? selectedClass,
            IEnumerable<DeckClass> filteredClasses,
            string searchString)
        {
            IEnumerable<Deck> filteredList;

            if (selectedClass.HasValue)
            {
                //specific class slected (there might nbe more in filteredclasses propery!!!
                filteredList = deckBase.Where(d =>
                    (d.Class == selectedClass)
                    && ((filteredTypes == null) || (filteredTypes.Contains(d.Type)))
                    && ((showCompletedArenaRuns) || (!deckCalculations.IsArenaRunFinished(d)))
                    && ((showHiddenDecks) || (!d.IsHidden))
                    && ((String.IsNullOrEmpty(searchString)) || (deckService.SearchString(d, searchString)))
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
                    && ((showCompletedArenaRuns) || (!deckCalculations.IsArenaRunFinished(d)))
                    && ((showHiddenDecks) || (!d.IsHidden))
                    && ((String.IsNullOrEmpty(searchString)) || (deckService.SearchString(d, searchString)))
                    );
                return filteredList;
            }


        }

        public void CommandResetFiltersExecute(object param)
        {
            messanger.Send<DeckListResetFilters>(new DeckListResetFilters(), ControlMessangerContext.DeckList_DeckFilterControl);
            this.DeckTextSearch = String.Empty;
            this.DeckTextSearchEntered = false;
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

        private void CommandHideDeckExecute(EditDeck editDeck)
        {
            if (editDeck.Deck != null)
            {
                editDeck.Deck.IsHidden = true;
                ApplyFilter();
            }
        }


        private void CommandUnHideDeckExecute(EditDeck editDeck)
        {
            if (editDeck.Deck != null)
            {
                editDeck.Deck.IsHidden = false;
                ApplyFilter();
            }
        }

        private void CommandDeleteDeckExecute(EditDeck editDeck)
        {
            if (deckService.CanDelete(editDeck.Deck))
            {
                deckService.DeleteDeck(editDeck.Deck);
            }
            fileManager.SaveDatabase();
            ApplyFilter();
        }
    }
}
