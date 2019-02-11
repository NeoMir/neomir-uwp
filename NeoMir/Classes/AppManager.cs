using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NeoMir.Classes
{
    public class AppManager
    {
        // PROPERTIES
        public static Frame RootFrame { get; set; }
        public static List<App> Apps = new List<App>();
        private static int MaxApp = 4;
        public static int AppPosition = 0; // 0 = Accueil, 1 = App rang 1, 2 = App rang 2, etc..

        // METHODS
        public static void AddApp(string link)
        {
            if (Apps.Count < MaxApp)
            {
                Apps.Add(new App(link));
                AppPosition = Apps.Count;
                OpenLastApp();
            }
            else
            {
                // Do nothing for the moment
            }
        }

        public static void OpenLastApp()
        {
            Apps[Apps.Count - 1].Open();
        }

        public static void NextApp()
        {
            if (Apps.Count == 0)
            {
                return;
            }
            else if (AppPosition == Apps.Count)
            {
                AppPosition = 0;
                Window.Current.Content = RootFrame;
                Window.Current.Activate();
            }
            else
            {
                AppPosition += 1;
                Apps[AppPosition - 1].Open();
            }
        }

        public static void PrevApp()
        {
            if (Apps.Count == 0)
            {
                return;
            }
            else if (AppPosition == 0 )
            {
                AppPosition = Apps.Count;
                Apps[AppPosition - 1].Open();
            }
            else if (AppPosition == 1)
            {
                AppPosition = 0;
                Window.Current.Content = RootFrame;
                Window.Current.Activate();
            }
            else
            {
                AppPosition -= 1;
                Apps[AppPosition - 1].Open();
            }
        }

        public static void GoToHome()
        {
            AppPosition = 0;
            RootFrame.Navigate(typeof(Pages.MainPage));
            Window.Current.Content = RootFrame;
            Window.Current.Activate();
        }

        public static void GoToApps()
        {
            AppPosition = 0;
            RootFrame.Navigate(typeof(Pages.AppsPage));
            Window.Current.Content = RootFrame;
            Window.Current.Activate();
        }
    }
}
