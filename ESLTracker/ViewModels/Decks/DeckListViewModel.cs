﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckListViewModel : ViewModelBase
    {

        public Dictionary<DeckType, bool> DeckTypeFilter { get; set; }

        public DeckClass? filteredClass;
        public DeckClass? FilteredClass
        {
            get { return filteredClass; }
            set
            {
                filteredClass = value;
                RaisePropertyChangedEvent("FilteredClass");
            }
        }

        public ObservableCollection<Deck> FilteredDecks { get; set; }

        //command for filter toggle button pressed
        public ICommand CommandResetFilterButtonPressed
        {
            get { return new RelayCommand(new Action<object>(ResetFilters)); }
        }


        private IDeckClassSelectorViewModel classFilterViewModel;
        private IDeckTypeSelectorViewModel typeFilterViewModel;

        public void SetClassFilterViewModel(IDeckClassSelectorViewModel cfvm)
        {
            classFilterViewModel = cfvm;
            classFilterViewModel.PropertyChanged += DeckListViewModel_PropertyChanged;
        }

        public void SetTypeFilterViewModel(IDeckTypeSelectorViewModel dts)
        {
            typeFilterViewModel = dts;
            typeFilterViewModel.FilteredTypes.CollectionChanged += FilteredTypes_CollectionChanged;
        }

        public DeckListViewModel()
        {
            DeckTypeFilter = new Dictionary<DeckType, bool>();
            foreach (DeckType a in Enum.GetValues(typeof(DeckType)))
            {
                DeckTypeFilter.Add(a, false);
            }


            FilteredDecks = new ObservableCollection<Deck>(Tracker.Instance.Decks);

        }

        private void DeckListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedClass")
            {
                ApplyFilter();
            }
        }

        private void FilteredTypes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Deck activeDeck = Tracker.Instance.ActiveDeck; //we loose it when FilteredDecks.Clear();
            FilteredDecks.Clear();

            FilterDeckList(Tracker.Instance.Decks).All(d => { FilteredDecks.Add(d); return true; });

            if (!FilteredDecks.Contains(activeDeck))
            {
                Tracker.Instance.ActiveDeck = null;
            }
            else
            {
                Tracker.Instance.ActiveDeck = activeDeck;
            }
        }

        public IEnumerable<Deck> FilterDeckList(IEnumerable<Deck> deckBase)
        {
            IEnumerable<Deck> filteredList;
            if (this.classFilterViewModel.SelectedClass != null)
            {
                //specific class slected (there might nbe more in filteredclasses propery!!!
                filteredList = deckBase.Where(d => 
                    (d.Class == this.classFilterViewModel.SelectedClass)
                    && (this.typeFilterViewModel.FilteredTypes.Contains(d.Type))
                    );
                return filteredList;
            }
            else
            {
                //filter by attributes
                filteredList = deckBase.Where(d => 
                    (this.classFilterViewModel.FilteredClasses.Contains(d.Class))
                    && (this.typeFilterViewModel.FilteredTypes.Contains(d.Type))
                    );
                return filteredList;
            }


        }


        public void ResetFilters(object param)
        {
            this.classFilterViewModel.Reset();
            this.typeFilterViewModel.Reset();
            ApplyFilter();
        }
    }
}