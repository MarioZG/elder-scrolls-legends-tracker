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

        public IEnumerable<string> CardNamesList
        {
            get
            {
                return trackerFactory.GetCardsDatabase().CardsNames;
            }
        }

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
            trackerFactory.GetCardsDatabase().PopulateCollection(CardNames, Pack.Cards);
        }

        private void CommandSaveExecute(object obj)
        {
            ITracker tracker = trackerFactory.GetTracker();
            Pack.DateOpened = trackerFactory.GetDateTimeNow();
            trackerFactory.GetCardsDatabase().PopulateCollection(CardNames, Pack.Cards);
            tracker.Packs.Add(Pack);
            new FileManager(trackerFactory).SaveDatabase();
            Pack = new Pack();
        }
    }
}
