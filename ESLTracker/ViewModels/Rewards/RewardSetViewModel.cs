using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels.Rewards
{
    public class RewardSetViewModel : ViewModelBase
    {
        PropertiesObservableCollection<Reward> rewards;

        public PropertiesObservableCollection<Reward> RewardsAdded
        {
            get
            {
                return new PropertiesObservableCollection<Reward>(rewards.Where( r=> 
                                 r.Quantity > 0 //thers qty 
                                 && (r.Type != RewardType.Card || r.CardInstance.HasCard)),  //if card, ensure selected
                    Rewards_CollectionChanged);
            }
        }

        public PropertiesObservableCollection<AddSingleRewardViewModel> RewardsEditor
        {
            get
            {
                return new PropertiesObservableCollection<AddSingleRewardViewModel>(
                    rewards.Where( r=> r.Reason == rewardReason).Select( r=> new AddSingleRewardViewModel(trackerFactory) { Reward = r, ParentRewardViewModel = this}),
                    Rewards_CollectionChanged);
            }
        }

        RewardReason? rewardReason;
        public RewardReason? RewardReason
        {
            get
            {
                return rewardReason;
            }
            set
            {
                rewardReason = value;
                RaisePropertyChangedEvent(nameof(RewardReason));
                RewardReasonChanged();
            }
        }
        
        Deck arenaDeck;
        public Deck ArenaDeck
        {
            get { return arenaDeck; }
            set { SetProperty(ref arenaDeck, value); }
        }

        private bool LinkRewardToDeck()
        {
            if (tracker.ActiveDeck == null)
            {
                return false;
            }
            bool matchVersus = (tracker.ActiveDeck.Type == DeckType.VersusArena)
                && (this.RewardReason == DataModel.Enums.RewardReason.VersusArena);
            bool matchSolo = (tracker.ActiveDeck.Type == DeckType.SoloArena)
                && (this.RewardReason == DataModel.Enums.RewardReason.SoloArena);
            bool gauntlet = (tracker.ActiveDeck.Type == DeckType.Constructed)
                && (this.RewardReason == DataModel.Enums.RewardReason.Gauntlet);
            return matchSolo || matchVersus || gauntlet;
        }

        private ITrackerFactory trackerFactory;
        ITracker tracker;

        public RewardSetViewModel() : this(new TrackerFactory())
        {
            
        }

        internal RewardSetViewModel(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            tracker = trackerFactory.GetTracker();
            rewards = new PropertiesObservableCollection<Reward>(CreateEmptySet(), Rewards_CollectionChanged);
        }

        private void Rewards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChangedEvent(nameof(RewardsAdded));
        }

        private IEnumerable<Reward> CreateEmptySet()
        {
            return new Reward[] {
                new Reward() { Type = RewardType.Gold },
                new Reward() { Type = RewardType.SoulGem },
                new Reward() { Type = RewardType.Pack },
                new Reward() { Type = RewardType.Card }
            };
        }

        //command for done  button - add grid values to xml file
        public RelayCommand CommandDoneButtonPressed
        {
            get { return new RelayCommand(new Action<object>(DoneClicked), new Func<object, bool>(CanExecuteDone)); }
        }

        public RelayCommand CommandEditReward
        {
            get { return new RelayCommand(new Action<object>(EditReward)); }
        }

        public RelayCommand CommandDeleteReward
        {
            get { return new RelayCommand(new Action<object>(DeleteReward)); }
        }

        public void DoneClicked(object param)
        {
            var newRewards = RewardsAdded.Where(r => !tracker.Rewards.Contains(r));
            //fix up excaly same date
            DateTime date = trackerFactory.GetDateTimeNow();
            foreach (Reward r in newRewards)
            {
                r.Date = date;
                r.ArenaDeck = ArenaDeck;
            }
            trackerFactory.GetTracker().Rewards.AddRange(newRewards);
            trackerFactory.GetFileManager().SaveDatabase();

            rewards.Clear();
            rewards = new PropertiesObservableCollection<Reward>(CreateEmptySet(), Rewards_CollectionChanged);
            RewardReason = null;
            RefreshRewardLists();
        }

        public bool CanExecuteDone(object param)
        {
            return RewardsAdded.Count > 0;
        }

        public void RewardReasonChanged()
        {

            if (!LinkRewardToDeck())
            {
                this.ArenaDeck = null;
            }
            else
            {
                this.ArenaDeck = tracker.ActiveDeck;
            }

            if (rewardReason.HasValue)
            {

                //add poteicallly missing if we are coming back
                IEnumerable<RewardType> possibletypes = (RewardType[])Enum.GetValues(typeof(RewardType));
                var typesAdded = rewards.Where(r => r.Reason == rewardReason).Select(r => r.Type).Distinct();
                if (typesAdded.Count() < possibletypes.Count())
                {
                    foreach (RewardType rt in possibletypes.Except(typesAdded))
                    {
                        AddNewReward(rt);
                    }
                }

                RaisePropertyChangedEvent(nameof(RewardsEditor));
            }
        }

        internal void AddNewReward(RewardType rt)
        {
            var indexToInsert = -1;
            indexToInsert = FindIndexToInsert(rt);

            if (indexToInsert > -1)
            {
                rewards.Insert(indexToInsert, new Reward(trackerFactory) { Type = rt, Reason = rewardReason.Value });
            }
            else
            {
                rewards.Add(new Reward(trackerFactory) { Type = rt, Reason = rewardReason.Value });
            }
            RaisePropertyChangedEvent(nameof(RewardsEditor));
        }

        /// <summary>
        /// find last of same type and reason
        ///if found, inster after
        ///if not
        ///..find any of higher type of this reson, insert before
        ///..if not found, find any lower and insert after
        ///..fallback - append to end
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        private int FindIndexToInsert(RewardType rt)
        {
            int indexToInsert;
            int index = RewardsEditor.Where(r => r.Reward.Type == rt)
                                   .Select(r => rewards.IndexOf(r.Reward))
                                   .OrderByDescending(i => i)
                                   .DefaultIfEmpty(-1)
                                   .FirstOrDefault();

            if (index > -1)
            {
                indexToInsert = index + 1;
            }
            else
            {
                index = RewardsEditor.Where(r => (int)r.Reward.Type > (int)rt)
                                .Select(r => rewards.IndexOf(r.Reward))
                                .OrderBy(i => i)
                                .DefaultIfEmpty(-1)
                                .FirstOrDefault();
                if (index > -1)
                {
                    indexToInsert = index;
                }
                else
                {
                    index = RewardsEditor.Where(r => (int)r.Reward.Type < (int)rt)
                        .Select(r => rewards.IndexOf(r.Reward))
                        .OrderByDescending(i => i)
                        .DefaultIfEmpty(-1)
                        .FirstOrDefault();
                    if (index > -1)
                    {
                        indexToInsert = index + 1;
                    }
                    else
                    {
                        indexToInsert = -1;
                    }
                }
            }

            return indexToInsert;
        }

        internal void DeleteReward(object obj)
        {
            Reward reward = obj as Reward;
            if (reward != null)
            {
                if ((reward.Reason != rewardReason) ||
                    (RewardsEditor.Where(r => r.Reward.Type == reward.Type).Count() > 1) //more than one current type
                    )
                {
                    this.rewards.Remove(obj as Reward);
                }
                else
                {
                    reward.Quantity = 0;
                    if (reward.CardInstance != null)
                    {
                        reward.CardInstance.Card = Card.Unknown;
                    }
                }
            }
            RefreshRewardLists();
        }

        private void EditReward(object obj)
        {
            Reward reward = (Reward)obj;
            if (rewardReason != reward.Reason)
            {
                RewardReason = reward.Reason; //jjust ensure is visible
            }                
        }

        private void RefreshRewardLists()
        {
            RaisePropertyChangedEvent(nameof(RewardsEditor));
            RaisePropertyChangedEvent(nameof(RewardsAdded));
        }
    }
}
