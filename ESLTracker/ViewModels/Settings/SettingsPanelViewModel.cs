using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.Properties;
using ESLTracker.Utils;

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

        private readonly IMessenger messanger;
        private readonly ISettings settings;

        private List<string> ChangedProperties = new List<string>();

        public SettingsPanelViewModel(IMessenger messanger, ISettings settings)
        {
            this.messanger = messanger;
            this.settings = settings;

            settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ChangedProperties.Add(e.PropertyName);
        }

        private void SaveClicked(object param)
        {
            settings.Save();
            messanger.Send(
                new Utils.Messages.EditSettings() {
                    ChangedProperties = new List<string>(this.ChangedProperties)
                },
                Utils.Messages.EditSettings.Context.EditFinished);
            ChangedProperties.Clear();
        }

        private void CancelButtonPressed(object obj)
        {
            settings.Reload();
            messanger.Send(new Utils.Messages.EditSettings(), Utils.Messages.EditSettings.Context.EditFinished);
            ChangedProperties.Clear();
        }

        private void OpenDataFolder(object param)
        {
            System.Diagnostics.Process.Start(settings.DataPath);
        }
    }
}
