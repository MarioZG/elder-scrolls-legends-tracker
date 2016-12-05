using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ESLTracker.Controls.Rewards;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.ViewModels.Rewards
{
    public class AddSingleRewardViewModel : ViewModelBase
    {
        private Reward reward = new Reward();
        public Reward Reward
        {
            get { return reward; }
            set { reward = value; RaisePropertyChangedEvent("Reward"); }
        }


        private bool guildSelectionVisible;
        public bool GuildSelectionVisible
        {
            get { return guildSelectionVisible && isInEditMode && reward.Type == RewardType.Gold; }
            set { guildSelectionVisible = value; RaisePropertyChangedEvent("GuildSelectionVisible"); }
        }

        public string BackgroundImagePath
        {
            get
            {
                return @"pack://application:,,,/"
                 + "Resources/RewardType/" + reward.Type.ToString() + ".png";
            }
        }

        private bool isInEditMode;
        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set { isInEditMode = value;
                Visibility = Visibility.Visible;
                RaisePropertyChangedEvent("IsInEditMode");
                RaisePropertyChangedEvent("GuildSelectionVisible");
            }
        }

        private bool qtyFocus;
        public bool QtyFocus
        {
            get { return qtyFocus; }
            set
            {
                qtyFocus = value;
                RaisePropertyChangedEvent("QtyFocus");
            }
        }

        private bool commentsFocus;
        public bool CommentsFocus
        {
            get { return commentsFocus; }
            set
            {
                commentsFocus = value;
                RaisePropertyChangedEvent("CommentsFocus");
            }
        }

        //command for add button
        public ICommand CommandAddButtonPressed
        {
            get { return new RelayCommand(new Action<object>(AddClicked)); }
        }

        //command when control is cliked
        public ICommand CommandControlClicked
        {
            get { return new RelayCommand(new Action<object>(ControlActivated)); }
        }

        //command command for close icon
        public ICommand CommandCloseClicked
        {
            get { return new RelayCommand(new Action<object>(CloseClicked)); }
        }

        IRewardSetViewModel parentDataContext;
        public IRewardSetViewModel ParentDataContext
        {
            get { return parentDataContext; }
            set
            {
                parentDataContext = value;
                parentDataContext.RegisterControl(this);
            }
        }

        private Visibility visibility;
        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                visibility = value;
                RaisePropertyChangedEvent("Visibility");
            }
        }

        private Thickness margin;
        public Thickness Margin
        {
            get { return margin; }
            set
            {
                margin = value;
                RaisePropertyChangedEvent("Margin");
            }
        }

        public int ActualWidth { get; set; }
        public int ActualHeight { get; set; }

        public AddSingleRewardViewModel()
        {

        }

        internal void Reset()
        {
            this.IsInEditMode = false;
            this.Reward = new Reward()
            {
                Quantity = 0,
                Comment = String.Empty,
                RewardQuestGuild = null,
                Type = reward.Type
            };
            InitTypeSpecifics(this.reward.Type);
            this.Margin = new Thickness(0, 0, 0, 0);
        }

        private void InitTypeSpecifics(RewardType value)
        {
            switch (reward.Type)
            {
                case RewardType.Gold:
                    GuildSelectionVisible = true;
                    break;
                case RewardType.SoulGem:
                    GuildSelectionVisible = false;
                    break;
                case RewardType.Pack:
                    this.reward.Quantity = 1;
                    GuildSelectionVisible = false;
                    break;
                case RewardType.Card:
                    this.reward.Quantity = 1;
                    GuildSelectionVisible = false;
                    break;
                default:
                    break;
            }
            RaisePropertyChangedEvent("Reward");
        }

        public void AddClicked(object param)
        {
            ParentDataContext.AddReward(reward);
            this.ParentDataContext.SetActiveControl(null);
            Reset();
        }

        public void ControlActivated(object param)
        {
            //do not set any props here, use SetActiveControl() of RewardsSet, for better testing!
            this.ParentDataContext.SetActiveControl(this);

            this.QtyFocus = reward.Type == RewardType.Gold || reward.Type == RewardType.SoulGem;
            this.CommentsFocus = reward.Type == RewardType.Pack || reward.Type == RewardType.Card;
        }

        public void CloseClicked(object param)
        {
            Reset();
            this.ParentDataContext.SetActiveControl(null);
        }

        internal void SetRewardType(RewardType value)
        {
            this.Reward.Type = value;
            RaisePropertyChangedEvent("BackgroundImagePath");
        }

    }
}
