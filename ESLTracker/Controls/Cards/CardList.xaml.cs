using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ESLTracker.Controls.Cards
{
    /// <summary>
    /// Interaction logic for CardList.xaml
    /// </summary>
    public partial class CardList : UserControl
    {

        public ICommand MouseLeftClick
        {
            get { return (ICommand)GetValue(MouseLeftClickProperty); }
            set { SetValue(MouseLeftClickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseLeftClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseLeftClickProperty =
            DependencyProperty.Register("MouseLeftClick", typeof(ICommand), typeof(CardList), new PropertyMetadata(null));


        public CardList()
        {
            InitializeComponent();
        }
    }
}
