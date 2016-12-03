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

        public ICommand CommandCancelButtonPressed
        {
            get { return new RelayCommand(new Action<object>(CancelButtonPressed)); }
        }

        public ICommand CommandOpenDataFolder
        {
            get { return new RelayCommand(new Action<object>(OpenDataFolder)); }
        }

        private void SaveClicked(object param)
        {
            Properties.Settings.Default.Save();
            MainWindowViewModel.SettingsVisible = false;
            MainWindowViewModel.AllowCommands = true;
        }

        private void CancelButtonPressed(object obj)
        {
            MainWindowViewModel.SettingsVisible = false;
            MainWindowViewModel.AllowCommands = true;
            Properties.Settings.Default.Reload();
        }

        private void OpenDataFolder(object param)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.DataPath);
        }
    }
}
