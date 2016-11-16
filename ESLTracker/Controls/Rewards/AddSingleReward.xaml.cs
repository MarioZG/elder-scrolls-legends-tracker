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

       //use to be able to pass type from xaml - research better way!
        public RewardType Type {
            get { return this.DataContext.type; }
            set
            {
                this.DataContext.Type = value;
            }
        }

        public IRewardSetViewModel ParentDataContext
        {
            get { return this.DataContext.ParentDataContext; }
            set
            {
                this.DataContext.ParentDataContext = value;
            }
        }
            
    }
}
