using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ESLTracker.Utils.Messages;
using ESLTracker.ViewModels.Game;

namespace ESLTracker.Controls.Game
{
    /// <summary>
    /// Interaction logic for EditGame.xaml
    /// </summary>
    public partial class EditGame : UserControl
    {
        new public EditGameViewModel DataContext
        {
            get
            {
                return (EditGameViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }


        public bool IsEditControl
        {
            get { return (bool)GetValue(IsEditControlProperty); }
            set { SetValue(IsEditControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditControlProperty =
            DependencyProperty.Register(
                "IsEditControl", 
                typeof(bool), 
                typeof(EditGame), 
                new PropertyMetadata(false));


        public EditGame()
        {
            InitializeComponent();

            DataModel.Tracker.Instance.PropertyChanged += Instance_PropertyChanged;
            opponentClass.DataContext.PropertyChanged += DataContext_PropertyChanged;

            //TODO: Find a way to move it to xaml!
            var nameOfPropertyInVm = "IsEditControl";
            var binding = new Binding(nameOfPropertyInVm) { Mode = BindingMode.TwoWay };
            this.SetBinding(IsEditControlProperty, binding);

            Utils.Messenger.Default.Register<Utils.Messages.EditGame>(this, EditGameStart, Utils.Messages.EditGame.Context.StartEdit);

        }

        private void EditGameStart(Utils.Messages.EditGame obj)
        {
            this.selectedDeck.DataContext = obj.Game.Deck;
        }

        private void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SelectedClass")
            {
                this.DataContext.ShowWinsVsClass(opponentClass.DataContext.SelectedClass);
            }
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (! IsEditControl
                && (this.selectedDeck.DataContext != null) 
                && (DataModel.Tracker.Instance.ActiveDeck != null))
            {
                this.DataContext.Game.Deck = DataModel.Tracker.Instance.ActiveDeck;
                if (DataModel.Tracker.Instance.ActiveDeck.Type == DeckType.VersusArena)
                {
                    this.cbGameType.SelectedItem = DataModel.Enums.GameType.VersusArena;
                }
                else if (DataModel.Tracker.Instance.ActiveDeck.Type == DeckType.SoloArena)
                {
                    this.cbGameType.SelectedItem = DataModel.Enums.GameType.SoloArena;
                }
            }
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            //TODO: once pkayer rank is correct control, move this to data model
            this.cbPlayerRank.SelectedRank = Properties.Settings.Default.PlayerRank;
        }
    }
}
