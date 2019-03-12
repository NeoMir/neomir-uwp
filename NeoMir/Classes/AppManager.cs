using System;
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
        private static int MaxApp = 3;
        public static int AppPosition = 0; // 0 = Accueil, 1 = App rang 1, 2 = App rang 2, etc..
        public static string PendingApp;

        // METHODS
        public static void CreateApp(string link)
        {
            if (Apps.Count < MaxApp)
            {
                App app = new App(link);
                Apps.Add(app);
                AppPosition = Apps.Count;
                LaunchApp(Apps[Apps.Count - 1]);
                RootFrame.Navigate(typeof(Pages.MainPage));
            }
            else
            {
                // Maximum apps reached, ask the user confirmation to replace one.
                PendingApp = link;
                DisplayMaximumAppDialog();
            }
        }

        public static void LaunchApp(App app)
        {
            app.Frame.Navigate(typeof(Pages.AppPage), app.Link);
            Window.Current.Content = app.Frame;
            Window.Current.Activate();
            RootFrame.Navigate(typeof(Pages.MainPage));
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
                LaunchApp(Apps[AppPosition - 1]);
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
