using ESLTracker.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public class SplashScreenManager
    {
        SplashScreen splash;

        object threadlock = new object();

        public void ShowSplash()
        {
            Thread thread = new Thread(
               new ThreadStart(
                   delegate ()
                   {
                       lock (threadlock)
                       {
                           splash = new SplashScreen();
                       }
                       splash.Show();
                       System.Windows.Threading.Dispatcher.Run();
                   }
               ));
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        public void UpdateProgress(string statusMessage)
        {
            lock (threadlock)
            {
                if (splash != null)
                {
                    splash?.SetText(statusMessage);
                }
            }
        }

        public void CloseSplash()
        {
            lock (threadlock)
            {
                splash?.Dispatcher.BeginInvoke(new Action(() => splash.Close()));
            }
        }
    }
}
