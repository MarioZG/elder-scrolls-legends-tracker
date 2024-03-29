﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ESLTracker.BusinessLogic.Decks;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Behaviors;
using ESLTracker.Utils.Messages;
using TESLTracker.Utils;

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

        private ObservableCollection<TESLTracker.DataModel.Game> activeDeckGames;
        public ObservableCollection<TESLTracker.DataModel.Game> ActiveDeckGames {
            get {
                if (selectedClassFilter != null)
                {
                    dynamic d = selectedClassFilter;
                    return new ObservableCollection<TESLTracker.DataModel.Game>(activeDeckGames.Where(g => g.OpponentClass == d.Class).OrderByDescending(g=> g.Date).ToList());
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

        public RelayCommand CommandStartDrag { get; private set; }


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

            CommandStartDrag = new RelayCommand(CommandStartDragExecute);

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
            ActiveDeckGames = new ObservableCollection<TESLTracker.DataModel.Game>(deckCalculations.GetDeckGames(tracker.ActiveDeck).OrderByDescending(g=> g.Date));
            RaisePropertyChangedEvent("WinRatioVsClass");

            //hide if no games
            ShowControl = ActiveDeckGames.Count > 0;
        }

        private void GameAdded(EditDeck obj)
        {
            RefreshData();
        }

        private void CommandStartDragExecute(object obj)
        {
            DragDropEventInfo eventInfo = obj as DragDropEventInfo;
            DragDrop.DoDragDrop(eventInfo.Source, eventInfo.Data, DragDropEffects.Move);
        }
    }
}
