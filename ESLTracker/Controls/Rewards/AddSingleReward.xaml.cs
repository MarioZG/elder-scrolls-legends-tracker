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
using TESLTracker.DataModel.Enums;
using ESLTracker.ViewModels.Rewards;
using TESLTracker.DataModel;

namespace ESLTracker.Controls.Rewards
{
    /// <summary>
    /// Interaction logic for AddSingleReward.xaml
    /// </summary>
    public partial class AddSingleReward : UserControl
    {
        public AddSingleReward()
        {
            InitializeComponent();
        }

        private void control_GotFocus(object sender, RoutedEventArgs e)
        {
            if (((AddSingleRewardViewModel)this.DataContext).CardSelectionVisible)
            {
               FocusManager.SetFocusedElement(this, this.cardSelect);
            }
        }
    }
}
