using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.Properties;

namespace ESLTracker.ViewModels
{
    public class RelayCommandWithSettings : RelayCommand
    {
        Action<object, ISettings> action;
        Func<object, ISettings, bool> canExecute;
        ISettings settings;

        public RelayCommandWithSettings(Action<object, ISettings> action, ISettings settings) : this (action, null, settings)
        {
        }

        public RelayCommandWithSettings(
            Action<object, ISettings> action, 
            Func<object, ISettings, bool> canExecute,
            ISettings settings)
        {
            this.action = action;
            this.canExecute = canExecute;
            this.settings = settings;
        }

        public override void Execute(object parameter)
        {
            if (parameter != null)
            {
                action(parameter, settings);
            }
            else
            {
                action(null, settings);
            }
        }

    }
}
