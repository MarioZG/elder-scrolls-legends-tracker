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
using ESLTracker.ViewModels.Decks;

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for DeckList.xaml
    /// </summary>
    public partial class DeckList : UserControl
    {
        new public DeckListViewModel DataContext
        {
            get
            {
                return (DeckListViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public DeckList()
        {
            InitializeComponent();
            this.DataContext.SetClassFilterViewModel(this.deckFilter.deckClassFilter.DataContext);
            this.DataContext.SetTypeFilterViewModel(this.deckFilter.deckTypeFilter.DataContext);
        }

    }
}
