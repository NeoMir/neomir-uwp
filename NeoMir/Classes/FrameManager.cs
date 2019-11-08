using DataAccessLibrary.Entitites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NeoMir.Classes
{
    /// <summary>
    /// Class that manages the Apps navigation through the system
    /// </summary>
    public class FrameManager
    {
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
        // Maximum allowed of opened apps
        public static int MaxApp = 3;
        // The actual position of the navigation is the list of apps
        public static int AppPosition = 0; // 0 = Accueil, 1 = App rang 1, 2 = App rang 2, etc..
        // The temporary pending app when we reach the maximum of app
        public static string PendingApp { get; set; }
        // Return the current window
        public delegate void NavigateToEventHnadler(Page page);

        public static event NavigateToEventHnadler NavigatedEvent;


        #region Methods

        // Crée une application
        public static void CreateApp(UserApp userApp)
        {
            Apps.Add(new App(userApp));
            AppPosition = Apps.Count;
        }

        // Ouvre une application
        public static void LaunchApp(App app)
        {
            app.Frame.Navigate(typeof(Pages.AppPage), app.UserApp.AppLink);
            Window.Current.Content = app.Frame;
            Window.Current.Activate();
        }

        // Naviguer vers l'application suivante.
        public static void NextApp()
        {
            if (Apps.Count == 0)
            {
                return;
            }
            else if (AppPosition == Apps.Count)
            {
                GoTo(MainPageFrame);
            }
            else
            {
                AppPosition += 1;
                LaunchApp(Apps[AppPosition - 1]);
            }
        }

        // Naviguer ver l'application précédente.
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
                GoTo(MainPageFrame);
            }
            else
            {
                AppPosition -= 1;
                LaunchApp(Apps[AppPosition - 1]);
            }
        }

        // Naviguer vers une Frame spécifique.
        public static void GoTo(Frame destination)
        {
            if (destination != Window.Current.Content)
            {
                AppPosition = 0;
                Window.Current.Content = destination;
                Window.Current.Activate();
                NavigatedEvent?.Invoke(GetCurrentPage());
            }
        }

        public static Page GetCurrentPage()
        {
            Frame currentWindow = Window.Current.Content as Frame;
            return currentWindow.Content as Page;
        }

        public static void GetCurrentUserInstalledApps()
        {
            //TODO: API Request
        }

        #endregion
    }
}
