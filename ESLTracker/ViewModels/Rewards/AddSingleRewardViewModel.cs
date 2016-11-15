using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ESLTracker.Controls.Rewards;
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
                RaisePropertyChangedEvent("IsInEditMode");
                RaisePropertyChangedEvent("GuildSelectionVisible");
            }
        }

        //events for RewadSet
        public event EventHandler ControlClicked;
        public event EventHandler<NewRewardEventArgs> NewReward;


        //command for filter toggle button pressed
        public ICommand CommandAddButtonPressed
        {
            get { return new RelayCommand(new Action<object>(AddClicked)); }
        }

        //command for filter toggle button pressed
        public ICommand CommandControlClicked
        {
            get { return new RelayCommand(new Action<object>(ControlActivated)); }
        }

        //command for filter toggle button pressed
        public ICommand CommandCloseClicked
        {
            get { return new RelayCommand(new Action<object>(CloseClicked)); }
        }

        internal void Reset()
        {
            this.IsInEditMode = false;
            this.Quantity = 0;
            this.Comment = String.Empty;
            this.Guild = null;
            InitTypeSpecifics(this.type);
            //this.Margin = new Thickness(0, 0, 0, 0);
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

        }

        public void ControlActivated(object param)
        {
            this.IsInEditMode = true;
        }

        public void CloseClicked(object param)
        {
            Reset();
        }


    }
}
