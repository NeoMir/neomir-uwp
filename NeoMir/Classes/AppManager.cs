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
    public class AppManager
    {
        // PROPERTIES
        // The Frame where we can find the MainPage
        public static Frame MainPageFrame { get; set; }
        // The Frame where we can find the AppsPage
        public static Frame AppsPageFrame { get; set; }
        // The Frame where we can find the LockPage
        public static Frame LockPageFrame { get; set; }
        // List of the apps
        public static List<App> Apps = new List<App>();
        // The maximum of apps allowed to be openede
        public static List<App> InstalledApps = new List<App>();

        public static int MaxApp = 3;
        // The actual position of the navigation is the list of apps
        public static int AppPosition = 0; // 0 = Accueil, 1 = App rang 1, 2 = App rang 2, etc..
        // The temporary pending app when we reach the maximum of app
        public static string PendingApp { get; set; }

        // METHODS

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
                AppPosition = 0;
                Window.Current.Content = MainPageFrame;
                Window.Current.Activate();
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
                AppPosition = 0;
                Window.Current.Content = MainPageFrame;
                Window.Current.Activate();
            }
            else
            {
                AppPosition -= 1;
                LaunchApp(Apps[AppPosition - 1]);
            }
        }

        /// <summary>
        /// Go to the MainPage
        /// </summary>
        public static void GoToHome()
        {
            AppPosition = 0;
            Window.Current.Content = MainPageFrame;
            Window.Current.Activate();
        }

        /// <summary>
        /// Go to the AppsPage
        /// </summary>
        public static void GoToApps()
        {
            AppPosition = 0;
            Window.Current.Content = AppsPageFrame;
            Window.Current.Activate();
        }

        /// <summary>
        /// Go to the AppsPage
        /// </summary>
        public static void GoToLock()
        {
            AppPosition = 0;
            Window.Current.Content = LockPageFrame;
            Window.Current.Activate();
        }
    }
}
