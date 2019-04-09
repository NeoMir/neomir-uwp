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
        // The Main Frame where we find navigate to MainPage and AppsPage
        public static Frame RootFrame { get; set; }
        // List of the apps
        public static List<App> Apps = new List<App>();
        // The maximum of apps allowed to be opened
        private static int MaxApp = 3;
        // The actual position of the navigation is the list of apps
        public static int AppPosition = 0; // 0 = Accueil, 1 = App rang 1, 2 = App rang 2, etc..
        // The temporary pending app when we reach the maximum of app
        public static string PendingApp;

        // METHODS

        /// <summary>
        /// Creates & open the new application
        /// </summary>
        /// <param name="link">The link of the app</param>
        public static void CreateApp(string link)
        {
            if (Apps.Count < MaxApp)
            {
                App app = new App(link);
                Apps.Add(app);
                AppPosition = Apps.Count;
                Apps[AppPosition - 1].Frame.Navigate(typeof(Pages.AppPage), Apps[AppPosition - 1].Link);
                LaunchApp(Apps[AppPosition - 1]);
                RootFrame.Navigate(typeof(Pages.MainPage));
            }
            else
            {
                // Maximum apps reached, ask the user confirmation to replace one.
                PendingApp = link;
                DisplayMaximumAppDialog();
            }
        }

        /// <summary>
        /// Navigate to an app directly from its instance
        /// </summary>
        /// <param name="app">The app we want navigate to</param>
        public static void LaunchApp(App app)
        {
            Window.Current.Content = app.Frame;
            Window.Current.Activate();
            RootFrame.Navigate(typeof(Pages.MainPage));
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
                Window.Current.Content = RootFrame;
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
                Window.Current.Content = RootFrame;
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
            RootFrame.Navigate(typeof(Pages.MainPage));
            Window.Current.Content = RootFrame;
            Window.Current.Activate();
        }

        /// <summary>
        /// Go to the AppsPage
        /// </summary>
        public static void GoToApps()
        {
            AppPosition = 0;
            RootFrame.Navigate(typeof(Pages.AppsPage));
            Window.Current.Content = RootFrame;
            Window.Current.Activate();
        }

        /// <summary>
        /// When we reach the maximum of opened apps, display a dialog to ask the user to close or not an existing
        /// to be able to open a new one
        /// </summary>
        private async static void DisplayMaximumAppDialog()
        {
            ContentDialog contentDialog = new ContentDialog
            {
                Title = "Maximum d'application ouvertes atteint !",
                Content = "Vous avez atteint le maximum d'applications ouvertes simultanément, vous devez choisir quelle application vous voulez fermer.",
                PrimaryButtonText = "Continuer",
                CloseButtonText = "Pas maintenant"
            };

            ContentDialogResult result = await contentDialog.ShowAsync();

            // Annonce à l'utilisateur qu'il doit choisir une application à fermer.
            /// Ne rien faire sinon.
            if (result == ContentDialogResult.Primary)
            {
                // Afficher les applications pour que l'utilisateur puisse choisir.
                RootFrame.Navigate(typeof(Pages.CloseAppPage));
            }
            else
            {
                // Ne rien faire, l'utilisateur à pressé sur le boutton CloseButton.
            }
        }
    }
}
