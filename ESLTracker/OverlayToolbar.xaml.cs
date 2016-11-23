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
using System.Windows.Shapes;

namespace ESLTracker
{
    /// <summary>
    /// Interaction logic for OverlayToolbar.xaml
    /// </summary>
    public partial class OverlayToolbar : Window
    {
        public OverlayToolbar()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            if (this.Left == -1 || this.Top == -1)
            {
                //first run with clear settings
                this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width;
                this.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 4;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
             //this.DragMove(); 
        }

        private void btnMove_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //this.DragMove();
        }

        private void btnGame_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ToolBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           // base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

    }
}
