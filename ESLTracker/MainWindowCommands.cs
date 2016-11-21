using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ESLTracker
{
    class NotifyIconLeftClickCommand : ICommand
    {
        public void Execute(object parameter)
        {
            if (parameter is Window)
            {
                Window w = parameter as Window; 
                w.WindowState = WindowState.Normal;
                w.ShowInTaskbar = true;
                w.Activate();
                w.Focus();
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    class ExitCommand : ICommand
    {
        public void Execute(object parameter)
        {
            Utils.FileManager.SaveDatabase();
            MainWindow.UpdateOverlay = false;
            ((App)Application.Current).Exit();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }


    class ShowRewardsCommand : ICommand
    {
        public void Execute(object parameter)
        {
            new RewardsSummary().Show();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    class NewDeckCommand : ICommand
    {
        public void Execute(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).DataContext.DeckEditVisible = true;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    class ShowOverlayCommand : ICommand
    {
        public void Execute(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).RestoreOverlay();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
