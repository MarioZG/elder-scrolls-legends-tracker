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
using ESLTracker.Utils;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels;

namespace ESLTracker.Controls.Game
{
    /// <summary>
    /// Interaction logic for GameListItem.xaml
    /// </summary>
    public partial class GameListItem : UserControl
    {
        public ICommand CommandEditGame
        {
            get { return new RelayCommand(new Action<object>(EditGameExecute)); }
        }

        public GameListItem()
        {
            InitializeComponent();
        }

        private void EditGameExecute(object obj)
        {
            var messanger = MasserContainer.Container.GetInstance<IMessenger>();
            messanger.Send(
                new Utils.Messages.EditGame(this.DataContext as DataModel.Game),
                Utils.Messages.EditGame.Context.StartEdit);
        }

    }
}
