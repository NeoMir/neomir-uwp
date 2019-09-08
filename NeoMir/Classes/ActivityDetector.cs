using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace NeoMir.Classes
{
    /// <summary>
    /// Class to detect activity based on a Periodic timer.
    /// </summary>
    public class ActivityDetector
    {
        // PROPERTIES

        // Interval of time (in seconds) which we want to lock the app if the user doesn't have any recent activity
        private static int Period = 20;
        // Same as above but for the detection loop
        private TimeSpan PeriodToLock;
        // The periodic timer for the loop detection
        private ThreadPoolTimer PeriodicTimer;
        // The last user's activity
        private DateTime LastActivity;
        // The frame to redirect for the lock process
        private Frame LockFrame;
        // The difference of seconds between now and the last activity
        private double Difference;
        // If the app is lock or not
        private bool isLock;

        // CONSTRUCTOR
        public ActivityDetector()
        {
            Initialize();
            StartDetection();
        }

        // METHODS
        /// <summary>
        /// Initialize the variables
        /// </summary>
        private void Initialize()
        {
            Window.Current.CoreWindow.PointerPressed += UpdateLastActivity;
            LastActivity = DateTime.Now;
            this.LockFrame = FrameManager.LockPageFrame;
            this.PeriodToLock = TimeSpan.FromSeconds(Period);
            this.isLock = true;
        }

        /// <summary>
        /// Start the loop to detect each time if we can lock or not
        /// </summary>
        private void StartDetection()
        {
            this.PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                Difference = (DateTime.Now - LastActivity).TotalSeconds;
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        if (Difference >= PeriodToLock.TotalSeconds && !this.isLock)
                        {
                            isLock = true;
                            FrameManager.GoTo(this.LockFrame);
                        }
                    });
            }, this.PeriodToLock);
        }

        /// <summary>
        /// Update the time of the last activity
        /// </summary>
        private void UpdateLastActivity(CoreWindow sender, PointerEventArgs e)
        {
            if (isLock)
                isLock = !isLock;
            LastActivity = DateTime.Now;
        }
    }
}
