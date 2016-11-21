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
            if (Tracker.Instance.ActiveDeck == null)
            {
                return false;
            }
            bool matchVersus = (Tracker.Instance.ActiveDeck.Type == DeckType.VersusArena)
                && (this.RewardReason == DataModel.Enums.RewardReason.VersusArena);
            bool matchSolo = (Tracker.Instance.ActiveDeck.Type == DeckType.SoloArena)
                && (this.RewardReason == DataModel.Enums.RewardReason.SoloArena);
            return matchSolo || matchVersus ;
        }

        public RewardSetViewModel()
        {
            Rewards = new ObservableCollection<Reward>();
        }

        //command for done  button - add grid values to xml file
        public ICommand CommandDoneButtonPressed
        {
            get { return new RelayCommand(new Action<object>(DoneClicked)); }
        }

        public void DoneClicked(object param)
        {
            var newRewards = Rewards.Where(r => !DataModel.Tracker.Instance.Rewards.Contains(r));
            //fix up excaly same date
            DateTime date = DateTime.Now;
            foreach (Reward r in newRewards)
            {
                r.Date = date;
            }
            Tracker.Instance.Rewards.AddRange(newRewards);
            Utils.FileManager.SaveDatabase();
            Rewards.Clear();
            RewardReason = null;
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

            if (activeControl != null)
            {
                activeControl.IsInEditMode = true;
                activeControl.Margin = new Thickness(activeControl.ActualWidth / 2, activeControl.ActualHeight / 2, activeControl.ActualWidth / 2, activeControl.ActualHeight / 2);
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
                this.ArenaDeck = Tracker.Instance.ActiveDeck;
            }
        }

        public void AddReward(Reward reward)
        {
            if (reward != null)
            {
                reward.Reason = this.RewardReason.Value;
                reward.ArenaDeck = ArenaDeck;
                Rewards.Add(reward);
            }
        }
    }
}
