using ESLTracker.BusinessLogic.General;
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

namespace ESLTracker.Windows
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// 
    /// there is no datacontex and ApplicationInfo created manually, as this winodw is created before di container is initialised
    /// </summary>
    public partial class SplashScreen : Window
    {
        private readonly IApplicationInfo applicationInfo;

        public SplashScreen()
        {
            this.applicationInfo = new ApplicationInfo();

            InitializeComponent();
        }
        
        public string AppInfo
        {
            get
            {
                return $"Loading TESL Tracker version {applicationInfo.GetAssemblyFullSemVer()}";
            }
        }

        public void SetText(string text)
        {
            Dispatcher.BeginInvoke(new Action(() => this.txtStatus.Text = text));
        }
    }
}
