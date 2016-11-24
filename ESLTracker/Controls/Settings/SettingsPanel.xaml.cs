using System;
using System.Collections.Generic;
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
using ESLTracker.ViewModels.Settings;

namespace ESLTracker.Controls.Settings
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsPanel : UserControl
    {
        new public SettingsPanelViewModel DataContext
        {
            get
            {
                return (SettingsPanelViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public SettingsPanel()
        {
            InitializeComponent();
        }

    }
}
