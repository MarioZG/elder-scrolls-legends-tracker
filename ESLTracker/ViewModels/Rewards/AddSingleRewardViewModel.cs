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
        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; RaisePropertyChangedEvent("Quantity"); }
        }

        private string comment;
        public string Comment
        {
            get { return comment; }
            set { comment = value; RaisePropertyChangedEvent("Comment"); }
        }

        private Guild? guild;
        public Guild? Guild
        {
            get { return guild; }
            set { guild = value; RaisePropertyChangedEvent("Guild"); }
        }

        private bool guildSelectionVisible;
        public bool GuildSelectionVisible
        {
            get { return guildSelectionVisible && isInEditMode && type == RewardType.Gold; }
            set { guildSelectionVisible = value; RaisePropertyChangedEvent("GuildSelectionVisible"); }
        }

        public RewardType type;
        public RewardType Type
        {
            get { return type; }
            set
            {
                this.type = value;
                InitTypeSpecifics(value);
                RaisePropertyChangedEvent("Type");
                RaisePropertyChangedEvent("BackgroundImagePath");
                RaisePropertyChangedEvent("GuildSelectionVisible");
            }
        }

        public string BackgroundImagePath
        {
            get
            {
                return @"pack://application:,,,/"
                 + "Resources/RewardType/" + Type.ToString() + ".png";
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

        RewardSetViewModel parentDataContext;
        public RewardSetViewModel ParentDataContext
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
            this.Quantity = 0;
            this.Comment = String.Empty;
            this.Guild = null;
            InitTypeSpecifics(this.type);
            this.Margin = new Thickness(0, 0, 0, 0);
        }

        private void InitTypeSpecifics(RewardType value)
        {
            switch (type)
            {
                case RewardType.Gold:
                    GuildSelectionVisible = true;
                    break;
                case RewardType.SoulGem:
                    GuildSelectionVisible = false;
                    break;
                case RewardType.Pack:
                    this.Quantity = 1;
                    GuildSelectionVisible = false;
                    break;
                case RewardType.Card:
                    this.Quantity = 1;
                    GuildSelectionVisible = false;
                    break;
                default:
                    break;
            }
        }

        public void AddClicked(object param)
        {
            Reward reward = new Reward()
            {
                Comment = this.Comment,
                Quantity = this.Quantity,
                Type = this.type,
                RewardQuestGuild = this.Guild
            };

            ParentDataContext.AddReward(reward);
            this.ParentDataContext.SetActiveControl(null);
            Reset();
        }

        public void ControlActivated(object param)
        {
            this.IsInEditMode = true;
            this.ParentDataContext.SetActiveControl(this);
        }

        public void CloseClicked(object param)
        {
            Reset();
            this.ParentDataContext.SetActiveControl(null);
        }


    }
}
