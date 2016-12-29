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
    public class RewardSetViewModel : ViewModelBase, IRewardSetViewModel
    {
        ObservableCollection<Reward> rewards;
        public ObservableCollection<Reward> Rewards
        {
            get
            {
                return rewards;
            }
            set
            {
                rewards = value;
                RaisePropertyChangedEvent("Rewards");
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
                RaisePropertyChangedEvent("RewardReason");
                RaisePropertyChangedEvent("ArenaDeckVisible");
                RewardReasonChanged();
            }
        }
        
        Deck arenaDeck;
        public Deck ArenaDeck
        {
            get
            {
                return arenaDeck;
            }
            set
            {
                arenaDeck = value;
                RaisePropertyChangedEvent("ArenaDeck");
            }
        }

        Visibility arenaDeckVisible;
        public Visibility ArenaDeckVisible
        {
            get
            {
                return LinkRewardToDeck() ? Visibility.Visible : Visibility.Collapsed;
            }
            set
            {
                arenaDeckVisible = value;
                RaisePropertyChangedEvent("ArenaDeckVisible"); 
            }
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
            return matchSolo || matchVersus ;
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
            Rewards = new ObservableCollection<Reward>();
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
            var newRewards = Rewards.Where(r => !tracker.Rewards.Contains(r));
            //fix up excaly same date
            DateTime date = trackerFactory.GetDateTimeNow();
            foreach (Reward r in newRewards)
            {
                r.Date = date;
            }
            trackerFactory.GetTracker().Rewards.AddRange(newRewards);
            trackerFactory.GetFileManager().SaveDatabase();
            Rewards.Clear();
            RewardReason = null;
        }

        public bool CanExecuteDone(object param)
        {
            //return false;
            //null passd when control deactivaed, control view model when activated
            return this.activeControl == null;
        }

        //managing AddSingleReward Controls
        List<AddSingleRewardViewModel> rewardControls = new List<AddSingleRewardViewModel>();

        AddSingleRewardViewModel activeControl;

        public void RegisterControl(AddSingleRewardViewModel c)
        {
            rewardControls.Add(c);
        }

        public void SetActiveControl(AddSingleRewardViewModel activeControl)
        {
            this.activeControl = activeControl;
            CommandManager.InvalidateRequerySuggested();
            if (activeControl != null)
            {
                activeControl.IsInEditMode = true;
                activeControl.Margin = new Thickness(activeControl.ActualWidth / 3, 0, activeControl.ActualWidth / 3, 0);
                activeControl.GuildSelectionVisible = this.RewardReason == DataModel.Enums.RewardReason.Quest;
            }

            //reset other controls
            foreach (AddSingleRewardViewModel asr in rewardControls.Where( c=> c != activeControl))
            {
                    asr.Reset();

                    //when no controls active set visible, when one active, set other to collapsed
                    asr.Visibility = activeControl == null ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        //end of managing single reward controls

        public void RewardReasonChanged()
        {
            foreach (AddSingleRewardViewModel asr in rewardControls)
            {
                asr.Reset();
                asr.Visibility = Visibility.Visible;
            }

            if (! LinkRewardToDeck())
            {
                this.ArenaDeck = null;
                this.ArenaDeckVisible = Visibility.Collapsed;
            }
            else {
                this.ArenaDeck = tracker.ActiveDeck;
            }
        }

        public void AddReward(Reward reward)
        {
            if (reward != null)
            {
                reward.Reason = this.RewardReason.Value;
                reward.ArenaDeck = ArenaDeck;
                if (!Rewards.Contains(reward))
                {
                    Rewards.Add(reward);
                }
            }
        }

        private void DeleteReward(object obj)
        {
            if (obj is Reward)
            {
                this.Rewards.Remove(obj as Reward);
            }
        }

        private void EditReward(object obj)
        {
            Reward reward = (Reward)obj;
            foreach (AddSingleRewardViewModel asr in rewardControls.Where(c => c.Reward.Type == reward.Type))
            {
                asr.Reward = reward;
                SetActiveControl(asr);
            }                
        }
    }
}
