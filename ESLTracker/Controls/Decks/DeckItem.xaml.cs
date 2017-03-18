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
using ESLTracker.DataModel;
using ESLTracker.Services;
using ESLTracker.Utils.Messages;

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for DeckItem.xaml
    /// </summary>
    public partial class DeckItem : UserControl
    {

        public DeckItem()
        {
            InitializeComponent();
            var messanger = new Utils.TrackerFactory().GetService<IMessenger>();

            //need to keep this for refreshing attributes icons - until class have correct binding!
            messanger.Register<Utils.Messages.EditDeck>(this, EditDeckEvent, Utils.Messages.EditDeck.Context.EditFinished);
            messanger.Register<Utils.Messages.EditDeck>(this, EditDeckEvent, Utils.Messages.EditDeck.Context.StatsUpdated);

        }

        private void EditDeckEvent(Utils.Messages.EditDeck obj)
        {
           if (this.DataContext != null && this.DataContext is Deck && obj.Deck.DeckId == ((DataModel.Deck)this.DataContext).DeckId)
            {
                ((Deck)this.DataContext).UpdateAllBindings();
            }
        }
    }
}
