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
                    return new ObservableCollection<DataModel.Game>(activeDeckGames.Where(g => g.OpponentClass == d.Class).ToList());
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

        public DeckStatsViewModel()
        {
            Tracker.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "ActiveDeck")
                && (Tracker.Instance.ActiveDeck != null))
            {
                WinRatioVsClass = Tracker.Instance.ActiveDeck.GetDeckGames()
                    .GroupBy(d => d.OpponentClass)
                    .Select(d => new { Class = d.Key,
                        Attributes = ClassAttributesHelper.Classes[d.Key],
                        Total = d.Count(),
                        Victory = d.Where(d2=>d2.Outcome == GameOutcome.Victory).Count(),
                        Defeat = d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                        WinPercent = d.Count() > 0 ? Math.Round((decimal)d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() / (decimal)d.Count() * 100,0).ToString() : "-"
                    });
                ActiveDeckGames = new ObservableCollection<DataModel.Game>(Tracker.Instance.ActiveDeck.GetDeckGames());
                ActiveDeckRewards = new ObservableCollection<Reward>(Tracker.Instance.Rewards.Where(r => r.ArenaDeckId == Tracker.Instance.ActiveDeck.DeckId));
                RaisePropertyChangedEvent("WinRatioVsClass");
            }
        }
    }
}
