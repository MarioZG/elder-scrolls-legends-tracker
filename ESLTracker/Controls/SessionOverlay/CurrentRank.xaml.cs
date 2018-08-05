using FontAwesome.WPF;
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

namespace ESLTracker.Controls.SessionOverlay
{
    /// <summary>
    /// Interaction logic for CurrentRank.xaml
    /// </summary>
    public partial class CurrentRank : UserControl
    {
        #region Dependecy properties

        #region CurrentProgress
        public int CurrentProgress
        {
            get { return (int)GetValue(CurrentProgressProperty); }
            set { SetValue(CurrentProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentProgressProperty =
            DependencyProperty.Register("CurrentProgress", typeof(int), typeof(CurrentRank), new PropertyMetadata(-99, CurrentProgressChanged));

        private static void CurrentProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurrentRank cr = d as CurrentRank;
            int newValue = (int)e.NewValue;
            for(int i=0; i< cr.starsContainer.Children.Count; i++)
            {
                ImageAwesome star = (ImageAwesome)cr.starsContainer.Children[i];
                star.Icon = i < newValue + 2 ? FontAwesomeIcon.Star : FontAwesomeIcon.StarOutline;
            }
        }

        #endregion

        #region MaxStarsCount
        public int MaxStarsCount
        {
            get { return (int)GetValue(MaxStarsCountProperty); }
            set { SetValue(MaxStarsCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxStarsCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxStarsCountProperty =
            DependencyProperty.Register("MaxStarsCount", typeof(int), typeof(CurrentRank), new PropertyMetadata(-99, MaxStarsCountChanged));

        private static void MaxStarsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurrentRank cr = d as CurrentRank;
            int newValue = (int)e.NewValue;
            if(newValue == 0)
            {
                //legend - hide all
                cr.starsContainer.Visibility = Visibility.Collapsed;
            }
            else
            {
                cr.starsContainer.Visibility = Visibility.Visible;
                for (int i = 0; i < cr.starsContainer.Children.Count; i++)
                {
                    ImageAwesome star = (ImageAwesome)cr.starsContainer.Children[i];
                    star.Visibility = i < newValue + 2 ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        #endregion

        #endregion

        public CurrentRank()
        {
            InitializeComponent();
        }
    }
}
