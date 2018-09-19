using ESLTracker.DataModel;
using ESLTracker.Utils.Extensions;
using ESLTracker.ViewModels.Cards;
using ESLTracker.Windows;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ESLTracker.Controls.Cards
{
    /// <summary>
    /// Interaction logic for SelectCard.xaml
    /// </summary>
    public partial class SelectCard : UserControl
    {
        public CardInstance CardInstance
        {
            get { return (CardInstance)GetValue(CardProperty); }
            set { SetValue(CardProperty, value);  }
        }

        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("CardInstance", typeof(CardInstance), typeof(SelectCard), new PropertyMetadata(null, CardInstanceChanged));

        private static void CardInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SelectCardViewModel)((SelectCard)d).LayoutRoot.DataContext).CardInstance = e.NewValue as CardInstance;
        }

        public bool ShowIsPremium
        {
            get { return (bool)GetValue(ShowIsPremiumProperty); }
            set { SetValue(ShowIsPremiumProperty, value); }
        }

        public static readonly DependencyProperty ShowIsPremiumProperty =
            DependencyProperty.Register("ShowIsPremium", typeof(bool), typeof(SelectCard), new PropertyMetadata(false));



        public bool ShowQuantity
        {
            get { return (bool)GetValue(ShowQuantityProperty); }
            set { SetValue(ShowQuantityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowQuantity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowQuantityProperty =
            DependencyProperty.Register("ShowQuantity", typeof(bool), typeof(SelectCard), new PropertyMetadata(false));



        public IEnumerable<string> CardNameAutocomplete
        {
            get { return (IEnumerable<string>)GetValue(CardNameAutocompleteProperty); }
            set { SetValue(CardNameAutocompleteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardNameAutocomplete.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardNameAutocompleteProperty =
            DependencyProperty.Register("CardNameAutocomplete", typeof(IEnumerable<string>), typeof(SelectCard), new PropertyMetadata(null));

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        public bool ReadOnlyNot
        {
            get { return ! (bool)GetValue(ReadOnlyProperty); }
        }

        // Using a DependencyProperty as the backing store for ReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(SelectCard), new PropertyMetadata(false));



        public ICommand MouseLeftClick
        {
            get { return (ICommand)GetValue(MouseLeftClickProperty); }
            set { SetValue(MouseLeftClickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseLeftClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseLeftClickProperty =
            DependencyProperty.Register("MouseLeftClick", typeof(ICommand), typeof(SelectCard), new PropertyMetadata(null));
        
        public SelectCard()
        {
            InitializeComponent();

            this.IsVisibleChanged += SelectCard_IsVisibleChanged;
            ((SelectCardViewModel)this.LayoutRoot.DataContext).PropertyChanged += SelectCard_PropertyChanged;
        }

        private void SelectCard_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectCardViewModel.CardInstance))
            {
                if (this.CardInstance?.CardId != ((SelectCardViewModel)sender).CardInstance?.CardId)
                {
                    this.CardInstance = ((SelectCardViewModel)sender).CardInstance;
                }
            }
        }

        //https://www.codeproject.com/Tips/478376/Setting-focus-to-a-control-inside-a-usercontrol-in
        //http://stackoverflow.com/questions/9535784/setting-default-keyboard-focus-on-loading-a-usercontrol
        private void SelectCard_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsKeyboardFocusWithin && this.Focusable && ((bool)e.NewValue == true))
            {
                Dispatcher.BeginInvoke(
                DispatcherPriority.ContextIdle,
                new Action(delegate ()
                {
                    txtName.Focus();
                }));
            }
        }

        /// <summary>
        /// Used for moving the caret to the end of the suggested auto-completion text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
            {
                var ci = ((SelectCardViewModel)this.LayoutRoot.DataContext).CardInstance;
                ci.IsPremium = !ci.IsPremium;
            }
            else if (e.Key == Key.Enter)
            {
                TextBox tb = e.OriginalSource as TextBox;
                if (tb == null)
                    return;

                ((SelectCardViewModel)(this.LayoutRoot.DataContext)).CardNameTyped(tb.Text);
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            DeckOverlay window = this.FindParent<DeckOverlay>();
            if ((window != null) && (e.LeftButton == MouseButtonState.Pressed))
            {
                window.DragMove();
            }
        }
    }
}
