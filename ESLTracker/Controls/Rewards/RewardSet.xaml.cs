using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
using ESLTracker.Utils;
using ESLTracker.ViewModels.Rewards;

namespace ESLTracker.Controls.Rewards
{
    /// <summary>
    /// Interaction logic for Reward.xaml
    /// </summary>
    public partial class RewardSet : UserControl
    {

        public RewardSet()
        {
            InitializeComponent();
            editorControls.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        // ensure focus on first element in reward lists
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (editorControls.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                ContentPresenter cp = (ContentPresenter)editorControls.ItemContainerGenerator.ContainerFromIndex(0);
                if (cp != null)
                {
                    cp.Loaded += Cp_Loaded;
                }
            }
        }

        // ensure focus on first element in reward lists
        private void Cp_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.FindVisualChildren<TextBox>(sender as DependencyObject)?.First()?.Focus();
        }
    }
}
