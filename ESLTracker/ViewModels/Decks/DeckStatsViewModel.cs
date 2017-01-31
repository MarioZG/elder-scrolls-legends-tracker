using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckStatsViewModel : ViewModelBase
    {
        public System.Collections.IEnumerable WinRatioVsClass { get; set; }// = new Dictionary<DeckClass, int>();

        private object selectedClassFilter;
        public object SelectedClassFilter
        {
            get { return selectedClassFilter; }
            set
            {
                selectedClassFilter = value;
                RaisePropertyChangedEvent("SelectedClassFilter");
                RaisePropertyChangedEvent("ActiveDeckGames");
            }
        }

        private ObservableCollection<DataModel.Game> activeDeckGames;
        public ObservableCollection<DataModel.Game> ActiveDeckGames {
            get {
                if (selectedClassFilter != null)
                {
                    dynamic d = selectedClassFilter;
                    return new ObservableCollection<DataModel.Game>(activeDeckGames.Where(g => g.OpponentClass == d.Class).OrderByDescending(g=> g.Date).ToList());
                }
                else
                {
                    return activeDeckGames;
                }
            }
            set
            {
                activeDeckGames = value;
                RaisePropertyChangedEvent("ActiveDeckGames");
            }
        }

        private ObservableCollection<DataModel.Reward> activeDeckRewards;
        public ObservableCollection<DataModel.Reward> ActiveDeckRewards
        {
            get
            {
                return activeDeckRewards;
            }
            set
            {
                activeDeckRewards = value;
                RaisePropertyChangedEvent("ActiveDeckRewards");
            }
        }

        public bool showControl = true;
        public bool ShowControl
        {
            get { return showControl; }
            set { showControl = value; RaisePropertyChangedEvent("ShowControl"); }
        }

        private IMessenger messanger;
        ITracker tracker;


        public DeckStatsViewModel() : this(new TrackerFactory())
        {

        }

        public DeckStatsViewModel(ITrackerFactory trackerFactory)
        {
            this.messanger = trackerFactory.GetMessanger();
            tracker = trackerFactory.GetTracker();

            if (tracker.ActiveDeck != null)
            {
                //load data for active deck from settigs
                RefreshData();
            }

            messanger.Register<EditDeck>(this, GameAdded, EditDeck.Context.StatsUpdated);
            messanger.Register<ActiveDeckChanged>(this, ActiveDeckChanged);
        }

        private void ActiveDeckChanged(ActiveDeckChanged obj)
        {
            if (obj.ActiveDeck != null) //null when filter on decklist is refreshed
            {
                RefreshData();
            }
        }

        private void RefreshData()
        {
            WinRatioVsClass = tracker.ActiveDeck.GetDeckVsClass();
            ActiveDeckGames = new ObservableCollection<DataModel.Game>(tracker.ActiveDeck.DeckGames.OrderByDescending(g=> g.Date));
            ActiveDeckRewards = new ObservableCollection<Reward>(tracker.Rewards.Where(r => r.ArenaDeckId == tracker.ActiveDeck.DeckId));
            RaisePropertyChangedEvent("WinRatioVsClass");

            //hide if no games
            ShowControl = ActiveDeckGames.Count > 0;
        }

        private void GameAdded(EditDeck obj)
        {
            RefreshData();
        }
    }
}
