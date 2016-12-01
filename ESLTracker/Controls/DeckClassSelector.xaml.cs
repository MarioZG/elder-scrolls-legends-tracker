using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        public DataModel.Enums.DeckClass? SelectedClass
        {
            get { return (DataModel.Enums.DeckClass?)GetValue(SelectedClassProperty); }
            set { SetValue(SelectedClassProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedClass.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedClassProperty =
            DependencyProperty.Register(
                "SelectedClass", 
                typeof(DataModel.Enums.DeckClass?), 
                typeof(DeckClassSelector), 
                new PropertyMetadata(null, SelectedClassChanged));

        /// <summary>
        /// used to update viewModel, as cannot bind to viewmodel and external properry at once
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void SelectedClassChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DeckClassSelector)d).DataContext.SelectedClass = e.NewValue as DataModel.Enums.DeckClass?;
        }

        public DeckClassSelector()
        {
            InitializeComponent();

            this.DataContext.PropertyChanged += DataContext_PropertyChanged;
        }


        /// <summary>
        /// if viewModel updates selected class (due filter click), update dependency property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedClass")
            {
                this.SelectedClass = this.DataContext.SelectedClass;
            }
        }
    }
}
