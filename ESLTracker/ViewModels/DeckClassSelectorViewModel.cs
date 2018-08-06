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
                SetProperty<DeckClass?>(
                    ref selectedClass, 
                    value,
                    onChanged: () => SyncToggleButtons(value));
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

        public const bool SelectFirstMatchingClassDefaultValue = true;

        public bool SelectFirstMatchingClass { get; set; } = SelectFirstMatchingClassDefaultValue;



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

        public DeckClassSelectorViewModel(IMessenger messenger)
        {
            this.messenger = messenger;
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

            if (! SelectFirstMatchingClass && !FilterButtonState[attrib])
            {
                //unchecked color - remove class selection
                SelectedClass = null;
            }

            FilterCombo();
        }

        public void FilterCombo()
        {
            var filteredClasses = Utils.ClassAttributesHelper.FindClassByAttribute(FilterButtonState.Where(f => f.Value).Select(f => f.Key)).ToList();

            if (SelectFirstMatchingClass)
            {


                if ((filteredClasses.Count >= 1)
                     && (FilterButtonState.Any(f => f.Value)))
                {
                    selectedClass = filteredClasses.OrderBy(fc => ClassAttributesHelper.Classes[fc].Count).First();
                }
                else
                {
                    selectedClass = null;
                }
                RaisePropertyChangedEvent(nameof(SelectedClass)); //above cannot assign directly to SelectedClass, as it will caouse issues with sync with buttons
            }
            else
            {
                if ((filteredClasses.Count == 1)
                    && (FilterButtonState.Any(f => f.Value)))
                {
                    selectedClass = filteredClasses.Single();
                    RaisePropertyChangedEvent(nameof(SelectedClass)); //above cannot assign directly to SelectedClass, as it will caouse issues with sync with buttons
                }
            }
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
            messenger.Send(new DeckListFilterChanged(DeckListFilterChanged.Source.ClassFilter, null, null, null, SelectedClass, FilteredClasses), MessangerContext);
        }

        public void Reset()
        {
            selectedClass = null;
            ResetToggleButtons();
            FilterCombo();
            RaisePropertyChangedEvent(nameof(FilterButtonStateCollection));
            RaisePropertyChangedEvent(nameof(SelectedClass));
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
            if (SelectFirstMatchingClass || value != null)
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
