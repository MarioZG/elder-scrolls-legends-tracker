using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ESLTracker.DataModel.Enums;
using ESLTracker.ViewModels.Rewards;

namespace ESLTracker.Controls.Rewards
{
    /// <summary>
    /// Interaction logic for AddSingleReward.xaml
    /// </summary>
    public partial class AddSingleReward : UserControl
    {

        new public AddSingleRewardViewModel DataContext
        {
            get
            {
                return (AddSingleRewardViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public AddSingleReward()
        {
            InitializeComponent();
        }

       
        public RewardType Type {
            get { return this.DataContext.type; }
            set
            {
                this.DataContext.Type = value;
            }
        }

        /*
        public event EventHandler ControlClicked;
        public event EventHandler<NewRewardEventArgs> NewReward;

        private bool selectGuild = false;

        private void InitTypeSpecifics(RewardType value)
        {
            this.image.Source = new BitmapImage(new Uri(@"pack://application:,,,/"
             + "Resources/RewardType/" + value.ToString() + ".png", UriKind.Absolute));
            switch (type)
            {
                case RewardType.Gold:
              //      SetGuildSelection(Visibility.Visible);
                    break;
                case RewardType.SoulGem:
                 //   SetGuildSelection(Visibility.Hidden);
                    break;
                case RewardType.Pack:
                    this.txtQuantity.Text = "1";
                  //  SetGuildSelection(Visibility.Hidden);
                    break;
                case RewardType.Card:
                    this.txtQuantity.Text = "1";
                   // SetGuildSelection(Visibility.Hidden);
                    break;
                default:
                    break;
            }
        }

        public event EventHandler ControlClicked;
        public event EventHandler<NewRewardEventArgs> NewReward;



        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowControls();
            ControlClicked?.Invoke(this, new EventArgs());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SetControlsVisibility(Visibility.Hidden);

            int qty;
            Int32.TryParse(this.txtQuantity.Text, out qty);

            Guild? guild = null;

            if (selectGuild)
            {
                guild = (Guild)cbGuild.SelectedItem;
            }

            NewReward?.Invoke(this, new NewRewardEventArgs( new DataModel.Reward {
                Comment = this.txtComment.Text == "comment" ? String.Empty : this.txtComment.Text,
                Quantity = this.txtComment.Text == "qty" ? 0: qty,
                Type = this.type,
                RewardQuestGuild = guild
            }));

        }

        internal void Reset()
        {
            this.txtQuantity.Text = "0";
            this.txtComment.Text = String.Empty;
            SetControlsVisibility(Visibility.Hidden);
            selectGuild = false;
            InitTypeSpecifics(this.type);
            this.Margin = new Thickness(0, 0, 0, 0);
        }

        internal void SetGuildSelection(Visibility visibilty)
        {
            this.cbGuild.Visibility = visibilty;
            selectGuild = visibilty == Visibility.Visible ? true : false;
        }

        private void close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NewReward?.Invoke(this, new NewRewardEventArgs(null));
        }

        */
    }
}
