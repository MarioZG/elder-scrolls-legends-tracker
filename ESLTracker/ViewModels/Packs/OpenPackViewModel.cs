using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels.Packs
{
    public class OpenPackViewModel : ViewModelBase
    {
        private TrackerFactory trackerFactory;

        public Pack Pack;
        public ObservableCollection<string> CardNames { get; set; } = new ObservableCollection<string>()
                    { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty};

        public ICommand CommandSave
        {
            get { return new RelayCommand(new Action<object>(CommandSaveExecute)); }
        }

        public OpenPackViewModel() : this(new TrackerFactory())
        {
            
        }

        public OpenPackViewModel(TrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            Pack = new Pack();
            CardNames.CollectionChanged += CardNames_CollectionChanged;
        }

        private void CardNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {           
            new CardsDatabase().PopulateCollection(CardNames, Pack.Cards);
        }

        private void CommandSaveExecute(object obj)
        {
            Pack.DateOpened = trackerFactory.GetDateTimeNow();
            new CardsDatabase().PopulateCollection(CardNames, Pack.Cards);
            trackerFactory.GetTracker().Packs.Add(Pack);
            Pack = new Pack();
        }
    }
}
