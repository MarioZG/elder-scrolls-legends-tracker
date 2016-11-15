using System.Windows.Controls;
using ESLTracker.ViewModels;

namespace ESLTracker.Controls
{
    /// <summary>
    /// Interaction logic for DeckClassSelector.xaml
    /// </summary>
    public partial class DeckClassSelector : UserControl
    {

        new public DeckClassSelectorViewModel DataContext
        {
            get
            {
                return (DeckClassSelectorViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }


        public DeckClassSelector()
        {
            InitializeComponent();
        }

    }
}
