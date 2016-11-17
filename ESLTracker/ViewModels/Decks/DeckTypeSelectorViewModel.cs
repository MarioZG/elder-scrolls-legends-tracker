using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckTypeSelectorViewModel : ViewModelBase, IDeckTypeSelectorViewModel
    {
        /// <summary>
        /// fiter of attributes, 
        /// bool value binded to isenabled property of trigger button
        /// </summary>
        public Dictionary<DeckType, bool> FilterButtonState { get; set; }


        public ObservableCollection<DeckType> FilteredTypes { get; set; } = new ObservableCollection<DeckType>(Enum.GetValues(typeof(DeckType)).OfType<DeckType>());


        //command for filter toggle button pressed
        public ICommand CommandFilterButtonPressed
        {
            get { return new RelayCommand(new Action<object>(FilterClicked)); }
        }

        public DeckTypeSelectorViewModel()
        {
            FilterButtonState = new Dictionary<DeckType, bool>();
            foreach (DeckType a in Enum.GetValues(typeof(DeckType)))
            {
                FilterButtonState.Add(a, false);
            }
        }

        public void FilterClicked(object param)
        {
            DeckType attrib;
            if (!Enum.TryParse<DeckType>(param.ToString(), out attrib))
            {
                throw new ArgumentException(string.Format("Unknow value for DeckType={0}", param));
            }

            //toggle filter value
            FilterButtonState[attrib] = !FilterButtonState[attrib];

            //fires to many events on colection, will filter once for reset, and then for every selected attrib
            FilteredTypes.Clear();

            //if all unslected - show all
            if (FilterButtonState.All(f => !f.Value))
            {
                Enum.GetValues(typeof(DeckType)).OfType<DeckType>().All(t => { FilteredTypes.Add(t); return true; });
            }
            else
            {
                foreach (DeckType type in FilterButtonState.Where(f => f.Value).Select(f => f.Key))
                {
                    FilteredTypes.Add(type);
                }
            }

        }

        public void Reset()
        {
            foreach (DeckType a in Enum.GetValues(typeof(DeckType)))
            {
                //unselct all in UI
                FilterButtonState[a] = false;

                //reset filter collection
                FilteredTypes.Clear();
                Enum.GetValues(typeof(DeckType)).OfType<DeckType>().All(t => { FilteredTypes.Add(t); return true; });
            }
            RaisePropertyChangedEvent("FilterButtonState");
        }
    }
}
