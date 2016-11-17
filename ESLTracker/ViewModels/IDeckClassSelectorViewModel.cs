using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.ViewModels
{
    public interface IDeckClassSelectorViewModel : INotifyPropertyChanged
    {
        ICommand CommandFilterButtonPressed { get; }
        Dictionary<DeckAttribute, bool> FilterButtonState { get; set; }
        ObservableCollection<DeckClass> FilteredClasses { get; set; }
        DeckClass? SelectedClass { get; set; }
        DeckAttributes SelectedClassAttributes { get; }

        void FilterClicked(object param);
        void Reset();
    }
}