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
            Utils.Messenger.Default.Send(new Utils.Messages.EditSettings(), Utils.Messages.EditSettings.Context.EditFinished);
        }

        private void CancelButtonPressed(object obj)
        {
            Properties.Settings.Default.Reload();
            Utils.Messenger.Default.Send(new Utils.Messages.EditSettings(), Utils.Messages.EditSettings.Context.EditFinished);
        }

        private void OpenDataFolder(object param)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.DataPath);
        }
    }
}
