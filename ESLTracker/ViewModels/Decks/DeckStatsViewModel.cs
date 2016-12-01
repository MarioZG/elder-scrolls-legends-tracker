using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

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

        public DeckStatsViewModel()
        {
            Tracker.Instance.PropertyChanged += Instance_PropertyChanged;
            if (Tracker.Instance.ActiveDeck != null)
            {
                //load data for active deck from settigs
                RefreshData();
            }
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "ActiveDeck")
                && (Tracker.Instance.ActiveDeck != null))
            {
                RefreshData();
            }
        }

        private void RefreshData()
        {
            WinRatioVsClass = Tracker.Instance.ActiveDeck.GetDeckVsClass();
            ActiveDeckGames = new ObservableCollection<DataModel.Game>(Tracker.Instance.ActiveDeck.GetDeckGames().OrderByDescending(g=> g.Date));
            ActiveDeckRewards = new ObservableCollection<Reward>(Tracker.Instance.Rewards.Where(r => r.ArenaDeckId == Tracker.Instance.ActiveDeck.DeckId));
            RaisePropertyChangedEvent("WinRatioVsClass");

            //hide if no games
            ShowControl = ActiveDeckGames.Count > 0;
        }
    }
}
