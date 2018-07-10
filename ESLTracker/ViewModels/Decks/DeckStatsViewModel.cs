using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Services;
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



        public bool showControl = true;
        public bool ShowControl
        {
            get { return showControl; }
            set { showControl = value; RaisePropertyChangedEvent("ShowControl"); }
        }

        private readonly IMessenger messanger;
        private readonly ITracker tracker;
        private readonly DeckCalculations deckCalculations;

        public DeckStatsViewModel(ITracker tracker, IMessenger messanger, DeckCalculations deckCalculations)
        {
            this.messanger = messanger;
            this.tracker = tracker;
            this.deckCalculations = deckCalculations;

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
            WinRatioVsClass = deckCalculations.GetDeckVsClass(tracker.ActiveDeck, null);
            ActiveDeckGames = new ObservableCollection<DataModel.Game>(deckCalculations.GetDeckGames(tracker.ActiveDeck).OrderByDescending(g=> g.Date));
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
