using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.Games;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ESLTracker.ViewModels.SessionOverlay
{
    public class SessionOverviewViewModel : ViewModelBase
    {
        private readonly DeckCalculations deckCalculations;
        private readonly IMessenger messenger;
        private readonly ITracker tracker;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly RankCalculations rankCalculations;

        public IEnumerable<DataModel.Game> Games
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

        public SessionOverviewViewModel(
            IDateTimeProvider dateTimeProvider, 
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

            messenger.Register<EditDeck>(this, GameAdded, EditDeck.Context.StatsUpdated);

            StartDate = dateTimeProvider.DateTimeNow;

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
            rankCalculations.CalculateCurrentRankProgress(this.tracker.Games, out rank, out rankProgress, out rankStarsCount);

            CurrentRank = rank;
            CurrentRankProgress = rankProgress;
            CurrentRankStarsCount = rankStarsCount;

            RaisePropertyChangedEvent(nameof(CurrentRank));
            RaisePropertyChangedEvent(nameof(CurrentRankProgress));
            RaisePropertyChangedEvent(nameof(CurrentRankStarsCount));
        }
    }
}
