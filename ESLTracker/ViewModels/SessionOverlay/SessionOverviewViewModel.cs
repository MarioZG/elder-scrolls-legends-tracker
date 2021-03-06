﻿using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.Games;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using ESLTracker.Properties;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TESLTracker.Utils;

namespace ESLTracker.ViewModels.SessionOverlay
{
    public class SessionOverviewViewModel : ViewModelBase
    {
        private readonly DeckCalculations deckCalculations;
        private readonly IMessenger messenger;
        private readonly ITracker tracker;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly RankCalculations rankCalculations;
        private readonly ISettings settings;

        public RelayCommand CommandResetSession { get; private set; }


        public IEnumerable<TESLTracker.DataModel.Game> Games
        {
            get
            {
                return tracker.Games.Where(g => g.Date >= StartDate).ToList();
            }
        }

        public DateTime StartDate { get; private set; }
        public TimeSpan TimePassed
        {
            get
            {
                return dateTimeProvider.DateTimeNow - StartDate;
            }
        }

        public int Victories
        {
            get
            {
                return deckCalculations.Victories(Games);
            }
        }

        public int Defeats
        {
            get
            {
                return deckCalculations.Defeats(Games);
            }
        }

        public string WinRatio
        {
            get
            {
                return deckCalculations.WinRatio(Games);
            }
        }

        public PlayerRank CurrentRank
        {
            get; set;
        }

        public int CurrentRankProgress { get; set; }

        public int CurrentRankStarsCount { get; set; }

        public int? LegendStartRank { get; set; }
        public int? LegendCurrentRank { get; set; }
        public int? LegendMinRank { get; set; }
        public int? LegendMaxRank { get; set; }

        public SessionOverviewViewModel(
            IDateTimeProvider dateTimeProvider,
            ISettings settings,
            DeckCalculations deckCalculations,
            IMessenger messenger,
            ITracker tracker,
            RankCalculations rankCalculations)
        {
            this.deckCalculations = deckCalculations;
            this.messenger = messenger;
            this.tracker = tracker;
            this.dateTimeProvider = dateTimeProvider;
            this.rankCalculations = rankCalculations;
            this.settings = settings;

            CommandResetSession = new RelayCommand(new Action<object>(CommandResetSessionExecut));

            messenger.Register<EditDeck>(this, GameAdded, EditDeck.Context.StatsUpdated);

            StartDate = settings.SessionOverlay_ResetOnApplicationStart ? dateTimeProvider.DateTimeNow : settings.SessionOverlay_SessionStartDateTime;
            settings.SessionOverlay_SessionStartDateTime = StartDate;

            new DispatcherTimer(
                TimeSpan.FromSeconds(1),
                DispatcherPriority.Normal,
                delegate { RaisePropertyChangedEvent(nameof(TimePassed)); },
                Application.Current.Dispatcher)
            .Start();

            CalculateRankProgress();
        }

        private void GameAdded(EditDeck obj)
        {
            RaisePropertyChangedEvent(String.Empty);
            CalculateRankProgress();

        }

        private void CalculateRankProgress()
        {
            PlayerRank rank;
            int rankProgress, rankStarsCount;
            int? legendStart, legendMin, legedmax, legendCurrent;
            rankCalculations.CalculateCurrentRankProgress(
                this.tracker.Games,
                StartDate,
                out rank, 
                out rankProgress, 
                out rankStarsCount,
                out legendStart, 
                out legendMin, 
                out legedmax, 
                out legendCurrent
                );

            CurrentRank = rank;
            CurrentRankProgress = rankProgress;
            CurrentRankStarsCount = rankStarsCount;

            LegendCurrentRank = legendCurrent;
            LegendMaxRank = legedmax;
            LegendMinRank = legendMin;
            LegendStartRank = legendStart;

            RaisePropertyChangedEvent(nameof(CurrentRank));
            RaisePropertyChangedEvent(nameof(CurrentRankProgress));
            RaisePropertyChangedEvent(nameof(CurrentRankStarsCount));
            RaisePropertyChangedEvent(nameof(LegendStartRank));
            RaisePropertyChangedEvent(nameof(LegendMinRank));
            RaisePropertyChangedEvent(nameof(LegendMaxRank));
            RaisePropertyChangedEvent(nameof(LegendCurrentRank));
        }

        private void CommandResetSessionExecut(object obj)
        {
            StartDate = dateTimeProvider.DateTimeNow;
            RaisePropertyChangedEvent(String.Empty);
        }


    }
}
