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
                new PropertyMetadata(null, ExternalSelectedClassChanged));


        internal DataModel.Enums.DeckClass? InternalSelectedClass
        {
            get { return (DataModel.Enums.DeckClass?)GetValue(InternalSelectedClassProperty); }
            set { SetValue(InternalSelectedClassProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalSelectedClass.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty InternalSelectedClassProperty =
            DependencyProperty.Register("InternalSelectedClass", typeof(DataModel.Enums.DeckClass?), typeof(DeckClassSelector), new PropertyMetadata(null, InternalSelectedClassChanged));

        private static void InternalSelectedClassChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //DeckClassSelectorViewModel model = ((DeckClassSelector)d).DataContext;
            //model.SyncToggleButtons(e.NewValue as DataModel.Enums.DeckClass?);
            ((DeckClassSelector)d).SelectedClass = e.NewValue as DataModel.Enums.DeckClass?;
        }

        private static void ExternalSelectedClassChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DeckClassSelectorViewModel model = ((DeckClassSelector)d).DataContext;
            model.SyncToggleButtons(e.NewValue as DataModel.Enums.DeckClass?);
            ((DeckClassSelector)d).InternalSelectedClass = e.NewValue as DataModel.Enums.DeckClass?;
        }

        public DeckClassSelector()
        {
            InitializeComponent();

            var nameOfPropertyInVm = "SelectedClass";
            var binding = new Binding(nameOfPropertyInVm) { Mode = BindingMode.TwoWay };
            this.SetBinding(InternalSelectedClassProperty, binding);

        }

    }
}
