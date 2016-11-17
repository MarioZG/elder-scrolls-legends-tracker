using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.ViewModels.Decks
{
    public interface IDeckTypeSelector
    {
        ObservableCollection<DeckType> FilteredTypes { get; set; }
    }
}
