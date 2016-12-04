using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;

namespace ESLTracker.ViewModels
{
    public class DeckClassSelectorViewModel : ViewModelBase, IDeckClassSelectorViewModel
    {
        /// <summary>
        /// fiter of attributes, 
        /// bool value binded to isenabled property of trigger button
        /// </summary>
        public Dictionary<DeckAttribute, bool> FilterButtonState { get; set; }

        public ObservableCollection<DeckAttribute> FilterButtonStateCollection
        {
            get { return new ObservableCollection<DeckAttribute>(
                FilterButtonState.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList()
                ); }
        }

        /// <summary>
        /// source for drop down , list of classes that match current filter
        /// </summary>
        public ObservableCollection<DeckClass> FilteredClasses { get; set; }

        private DeckClass? selectedClass;
        public DeckClass? SelectedClass {
            get
            {
                return selectedClass;
            }
            set
            {
                if (selectedClass != value)
                {
                    selectedClass = value;
                    SyncToggleButtons(value);
                    RaisePropertyChangedEvent("SelectedClass");
                }
            }
        }

        public DeckAttributes SelectedClassAttributes
        {
            get
            {
                if (SelectedClass.HasValue)
                {
                    return ClassAttributesHelper.Classes[SelectedClass.Value];
                }
                else
                {
                    return null;
                }
            }
        }

        IMessenger messenger;
        object messangerContext;
        public object MessangerContext {
            get { return messangerContext; }
            set
            {
                messenger.Unregister<DeckListResetFilters>(this, MessangerContext);
                messangerContext = value;
                messenger.Register<DeckListResetFilters>(this, ResetFiltersMessage, MessangerContext);
            }
        }

        //command for filter toggle button pressed
        public ICommand CommandFilterButtonPressed
        {
            get { return new RelayCommand(new Action<object>(FilterClicked)); }
        }



        public DeckClassSelectorViewModel() : this(new TrackerFactory())
        {

        }
        public DeckClassSelectorViewModel(ITrackerFactory factory)
        {
            messenger = factory.GetMessanger();
            FilterButtonState = new Dictionary<DeckAttribute, bool>();
            foreach (DeckAttribute a in Enum.GetValues(typeof(DeckAttribute)))
            {
                FilterButtonState.Add(a, false);
            }

            FilteredClasses = new ObservableCollection<DeckClass>();
            FilterCombo();
        }

        public void FilterClicked(object param)
        {
            DeckAttribute attrib;
            if (!Enum.TryParse<DeckAttribute>(param.ToString(), out attrib))
            {
                throw new ArgumentException(string.Format("Unknow value for deck attribute={0}", param));
            }

            //toggle filter value
            FilterButtonState[attrib] = ! FilterButtonState[attrib];

            FilterCombo();
        }

        public void FilterCombo()
        {
            var filteredClasses = Utils.ClassAttributesHelper.FindClassByAttribute(FilterButtonState.Where(f => f.Value).Select(f => f.Key)).ToList();
          
            if ((filteredClasses.Count >= 1)
                && (FilterButtonState.Any(f => f.Value)))
            {
                selectedClass = filteredClasses.OrderBy( fc=> ClassAttributesHelper.Classes[fc].Count).First();
            }
            else 
            {
                selectedClass = null;
            }
            RaisePropertyChangedEvent("SelectedClass");
            //remove classes not in use.Clear() will trigger binding, as SelectedClass will be set to null by framework
            foreach (DeckClass dc in FilteredClasses.ToList())
            {
                if (!filteredClasses.Contains(dc))
                {
                    FilteredClasses.Remove(dc);
                }
            }
            // FilteredClasses.Clear();
            foreach (DeckClass dc in filteredClasses)
            {
                if (!FilteredClasses.Contains(dc))
                {
                    int i = 0;
                    IComparer<DeckClass> comparer = Comparer<DeckClass>.Default;
                    while (i < FilteredClasses.Count && comparer.Compare(FilteredClasses[i], dc) < 0)
                        i++;

                    FilteredClasses.Insert(i, dc);
                }
            }
            messenger.Send(new DeckListFilterChanged(DeckListFilterChanged.Source.ClassFilter, null, null, SelectedClass, FilteredClasses), MessangerContext);
        }

        public void Reset()
        {
            ResetToggleButtons();
            FilterCombo();
            RaisePropertyChangedEvent("FilterButtonStateCollection");
        }

        private void ResetFiltersMessage(object obj)
        {
            Reset();
        }


        private void ResetToggleButtons()
        {
            foreach (DeckAttribute a in Enum.GetValues(typeof(DeckAttribute)))
            {
                FilterButtonState[a] = false;
            }
        }

        internal void SyncToggleButtons(DeckClass? value)
        {
            //if (value != null)
            {
                ResetToggleButtons();
                //toggle attributes buttons
                if (value!= null)
                {
                    foreach (DeckAttribute da in ClassAttributesHelper.Classes[value.Value])
                    {
                        FilterButtonState[da] = true;
                    }
                }
                FilterCombo();
                RaisePropertyChangedEvent("FilterButtonStateCollection");
            }
        }
    }
}
