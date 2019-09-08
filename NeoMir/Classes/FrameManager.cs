using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NeoMir.Classes
{
    /// <summary>
    /// Class that manages the Apps navigation through the system
    /// </summary>
    public class FrameManager
    {
        private static List<App> installedApps;

        // PROPERTIES
        // The Frame where we can find the MainPage
        public static Frame MainPageFrame { get; set; }
        // The Frame where we can find the AppsPage
        public static Frame AppsPageFrame { get; set; }
        // The Frame where we can find the LockPage
        public static Frame LockPageFrame { get; set; }
        //The pair use for pairing miror and App
        public static Frame PairPageFrame { get; set; }
        // List of the apps
        public static List<App> Apps = new List<App>();
        // List of installed Apps
        public static List<App> InstalledApps
        {
            get
            {
                if (installedApps == null)
                {
                    installedApps = new List<App>();
                }
                return installedApps;
            }
        }
        // Maximum allowed of opened apps
        public static int MaxApp = 3;
        // The actual position of the navigation is the list of apps
        public static int AppPosition = 0; // 0 = Accueil, 1 = App rang 1, 2 = App rang 2, etc..
        // The temporary pending app when we reach the maximum of app
        public static string PendingApp { get; set; }
        // Return the current window


        #region Methods

        /// <summary>
        /// Creates & open the new application
        /// </summary>
        /// <param name="link">The link of the app</param>
        public static void CreateApp(string link)
        {
            App app = new App(link);
            Apps.Add(app);
            AppPosition = Apps.Count;
            Apps[AppPosition - 1].Frame.Navigate(typeof(Pages.AppPage), Apps[AppPosition - 1].Link);
            LaunchApp(Apps[AppPosition - 1]);
        }

        public static void CreateInstalledApp(string link)
        {
            App app = new App(link);
            InstalledApps.Add(app);
        }

        /// <summary>
        /// Navigate to an app directly from its instance
        /// </summary>
        /// <param name="app">The app we want navigate to</param>
        public static void LaunchApp(App app)
        {
            Window.Current.Content = app.Frame;
            Window.Current.Activate();
        }

        public static void LaunchInstalledApp(App app)
        {
            app.Frame.Navigate(typeof(Pages.AppPage), app.Link);
            Apps.Add(app);
            Window.Current.Content = app.Frame;
            Window.Current.Activate();
        }

        /// <summary>
        /// Reach the next app in the navigation
        /// </summary>
        public static void NextApp()
        {
            if (Apps.Count == 0)
            {
                return;
            }
            else if (AppPosition == Apps.Count)
            {
                Classes.FrameManager.GoTo(Classes.FrameManager.MainPageFrame);
            }
            else
            {
                AppPosition += 1;
                LaunchApp(Apps[AppPosition - 1]);
            }
        }

        /// <summary>
        /// Reach the previous app in the navigation
        /// </summary>
        public static void PrevApp()
        {
            if (Apps.Count == 0)
            {
                return;
            }
            else if (AppPosition == 0 )
            {
                AppPosition = Apps.Count;
                LaunchApp(Apps[AppPosition - 1]);
            }
            else if (AppPosition == 1)
            {
                Classes.FrameManager.GoTo(Classes.FrameManager.MainPageFrame);
            }
            else
            {
                AppPosition -= 1;
                LaunchApp(Apps[AppPosition - 1]);
            }
        }

        /// <summary>
        /// Go to the destination frame
        /// </summary>
        /// <param name="destination">The frame you would like to go.</param>
        public static void GoTo(Frame destination)
        {
            AppPosition = 0;
            Window.Current.Content = destination;
            Window.Current.Activate();
        }

        public static Page GetCurrentPage()
        {
            Frame currentWindow = Window.Current.Content as Frame;
            return currentWindow.Content as Page;
        }

        public static void GetCurrentUserInstalledApps()
        {
            //TODO: API Request
            CreateInstalledApp("https://electronjs.org/");
            CreateInstalledApp("https://unity3d.com/fr");
            CreateInstalledApp("https://ionicframework.com/");
            CreateInstalledApp("https://electronjs.org/");
        }
        #endregion
    }
}
