using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ESLTracker.ViewModels.Settings
{
    public class SettingsPanelViewModel
    {
        internal MainWindowViewModel MainWindowViewModel { get; set; }

        public ICommand CommandSaveButtonPressed
        {
            get { return new RelayCommand(new Action<object>(SaveClicked)); }
        }

        private void SaveClicked(object param)
        {
            Properties.Settings.Default.Save();
            MainWindowViewModel.SettingsVisible = false;
        }
    }
}
