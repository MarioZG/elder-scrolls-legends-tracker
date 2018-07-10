using ESLTracker.BusinessLogic.Rewards;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESLTracker.ViewModels.Rewards
{
    public class RewardSetViewModel : ViewModelBase
    {
        /// <summary>
        /// Keep all rewards lines here (including qty 0)
        /// </summary>
        private PropertiesObservableCollection<Reward> rewards;

        /// <summary>
        /// list of rewards with qty > 0
        /// </summary>
        public PropertiesObservableCollection<Reward> RewardsAdded
        {
            get
            {
                return new PropertiesObservableCollection<Reward>(rewards?.Where( r=> 
                                 r.Quantity > 0 //thers qty 
                                 && (r.Type != RewardType.Card || r.CardInstance.HasCard)).OrderBy( r=> r.Reason).ThenBy( r=> r.Type),  //if card, ensure selected
                    Rewards_CollectionChanged);
            }
        }

        /// <summary>
        /// list ov reward view models for current reason 
        /// </summary>
        public PropertiesObservableCollection<AddSingleRewardViewModel> RewardsEditor { get; private set; }

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

        IAddSingleRewardViewModelFactory addSingleRewardViewModelFactory;
        ITracker tracker;
        IDateTimeProvider datetimeProvider;
        IFileManager fileManager;
        IRewardFactory rewardFactory;

        #region commands
        public RelayCommand CommandDoneButtonPressed { get; private set; }
        public RelayCommand CommandEditReward { get; private set; }
        public RelayCommand CommandDeleteReward { get; private set; }
        #endregion

        public RewardSetViewModel(
            IAddSingleRewardViewModelFactory addSingleRewardViewModelFactory, 
            ITracker tracker,
            IDateTimeProvider datetimeProvider,
            IFileManager fileManager,
            IRewardFactory rewardFactory)
        {
            this.addSingleRewardViewModelFactory = addSingleRewardViewModelFactory;
            this.tracker = tracker;
            this.datetimeProvider = datetimeProvider;
            this.fileManager = fileManager;
            this.rewardFactory = rewardFactory;

            CommandDoneButtonPressed = new RelayCommand(new Action<object>(DoneClicked), new Func<object, bool>(CanExecuteDone));
            CommandEditReward = new RelayCommand(new Action<object>(EditReward));
            CommandDeleteReward = new RelayCommand(new Action<object>(DeleteReward));

            rewards = new PropertiesObservableCollection<Reward>(new List<Reward>(), Rewards_CollectionChanged);
            RewardsEditor = new PropertiesObservableCollection<AddSingleRewardViewModel>(new List<AddSingleRewardViewModel>(), Rewards_CollectionChanged);
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



        public void DoneClicked(object param)
        {
            var newRewards = RewardsAdded.Where(r => !tracker.Rewards.Contains(r));
            //fix up excaly same date
            DateTime date = datetimeProvider.DateTimeNow;
            foreach (Reward r in newRewards)
            {
                r.Date = date;
                r.ArenaDeck = ArenaDeck;
            }
            tracker.Rewards.AddRange(newRewards);
            fileManager.SaveDatabase();

            rewards.Clear();
            RewardsEditor.Clear();
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
                RewardsEditor = new PropertiesObservableCollection<AddSingleRewardViewModel>(
                    rewards.Where(r => r.Reason == rewardReason).OrderBy( r => r.Type).Select(r => addSingleRewardViewModelFactory.Create(r, this)),
                    Rewards_CollectionChanged);

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

        internal Reward AddNewReward(RewardType rt)
        {
            var indexToInsert = -1;
            indexToInsert = FindIndexToInsert(rt);

            var newReward = rewardFactory.CreateReward(rt, rewardReason.Value);
            if ((indexToInsert > -1 )
                && (indexToInsert < RewardsEditor.Count))
            {
                rewards.Add(newReward);
                RewardsEditor.Insert(indexToInsert, addSingleRewardViewModelFactory.Create(newReward, this));
            }
            else
            {
                rewards.Add(newReward);
                RewardsEditor.Add(addSingleRewardViewModelFactory.Create(newReward, this));
            }
            
            RaisePropertyChangedEvent(nameof(RewardsEditor));

            return newReward;
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
            var rewardsList = RewardsEditor.Select(r => r.Reward).ToList();
            int index = RewardsEditor.Where(r => r.Reward.Type == rt)
                                   .Select(r => rewardsList.IndexOf(r.Reward))
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
                                .Select(r => rewardsList.IndexOf(r.Reward))
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
                        .Select(r => rewardsList.IndexOf(r.Reward))
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
                    rewards.Remove(obj as Reward);
                    RewardsEditor.Remove(RewardsEditor.Where(re => Object.ReferenceEquals(obj, re.Reward)).FirstOrDefault());
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
           RaisePropertyChangedEvent(nameof(RewardsAdded));
        }
    }
}
