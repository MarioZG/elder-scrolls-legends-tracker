using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.Controls
{
    /// <summary>
    /// Interaction logic for DeckClassSelector.xaml
    /// </summary>
    public partial class DeckClassSelector : UserControl
    {

        public DeckClass SelectedClass
        {
            get
            {
                return (DeckClass)this.comboBox.SelectedItem;
            }
            set
            {
                this.comboBox.SelectedItem = value;
            }
        }

        public DeckAttributes SelectedClassAttributes
        {
            get
            {
                return ClassAttributesHelper.Classes[SelectedClass];
            }
        }

        List<DeckAttribute> Filter = new List<DeckAttribute>();

        public DeckClassSelector()
        {
            InitializeComponent();

            this.comboBox.ItemsSource = Utils.ClassAttributesHelper.Classes.Keys;
        }

        private void btnStrength_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;
            if (tb.IsChecked.Value)
            {
                Filter.Add(EnumManager.ParseEnumString<DeckAttribute>(tb.Tag.ToString()));
            }
            else
            {
                Filter.Remove(EnumManager.ParseEnumString<DeckAttribute>(tb.Tag.ToString()));
            }
            FilterCombo();
        }

        private void FilterCombo()
        {
            var filteredClasses = Utils.ClassAttributesHelper.FindClassByAttribute(Filter);
            this.comboBox.ItemsSource = filteredClasses;
            if (filteredClasses.Count == 1)
            {
                this.comboBox.SelectedIndex = 0;
            }
        }

        public void Reset()
        {
            this.comboBox.SelectedItem = null;
            this.Filter.Clear();
            foreach (ToggleButton tb in WindowExtensions.FindVisualChildren<ToggleButton>(this))
            {
                tb.IsChecked = false;
            }
        }
    }
}
