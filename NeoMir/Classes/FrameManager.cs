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
    // Classe de gestion des Frames
    public class FrameManager
    {
        #region PROPERTIES

        // La Frame de la page MainPage
        public static Frame MainPageFrame { get; set; }
        // La Frame de la page AppsPage
        public static Frame AppsPageFrame { get; set; }
        // La Frame de la page LockPage
        public static Frame LockPageFrame { get; set; }
        // La Frame de la page PairPage
        public static Frame PairPageFrame { get; set; }
        // Liste des applications installées
        public static List<App> InstalledApps = new List<App>();
        // Liste des applications ouvertes
        public static List<App> OpenedApps = new List<App>();
        // Le maximum d'applications ouvertes autorisé
        public static int MaxApp = 3;
        // La position actuelle de navigation dans OpenedApps
        public static int AppPosition = 0; // Position 0 = Accueil, Position 1 = App 1, Position 2 = App 2, etc..
        // Retourne la page courante
        public delegate void NavigateToEventHandler(Page page);
        // Evenement de navigation
        public static event NavigateToEventHandler NavigatedEvent;

        #endregion

        #region Methods

        // Crée une application
        public static void CreateApp(UserApp userApp)
        {
            InstalledApps.Add(new App(userApp));
        }

        // Ouvre une application
        public static void LaunchApp(App app)
        {
            if (OpenedApps.Contains(app) == true)
            {
                var appIndex = OpenedApps.FindIndex(x => x.UserApp.AppId == app.UserApp.AppId);
                AppPosition = appIndex + 1;
                GoTo(OpenedApps[appIndex].Frame, true);
                return;
            }
            if (OpenedApps.Count == MaxApp)
            {
                OpenedApps.Reverse();
                OpenedApps[OpenedApps.Count - 1].Reset();
                OpenedApps.RemoveAt(OpenedApps.Count - 1);
                OpenedApps.Reverse();
                OpenedApps.Add(app);
            }
            else
            {
                OpenedApps.Add(app);
            }
            AppPosition = OpenedApps.Count;
            app.Frame.Navigate(typeof(Pages.AppPage), app.UserApp.AppLink);
            Window.Current.Content = app.Frame;
            Window.Current.Activate();
        }

        // Naviguer vers l'application suivante.
        public static void NextApp()
        {
            if (OpenedApps.Count == 0)
            {
                return;
            }
            else if (AppPosition == OpenedApps.Count)
            {
                GoTo(MainPageFrame);
                AppPosition = 0;
            }
            else
            {
                AppPosition += 1;
                GoTo(OpenedApps[AppPosition - 1].Frame, true);
            }
        }

        // Naviguer ver l'application précédente.
        public static void PrevApp()
        {
            if (OpenedApps.Count == 0)
            {
                return;
            }
            else if (AppPosition == 0 )
            {
                AppPosition = OpenedApps.Count;
                GoTo(OpenedApps[AppPosition - 1].Frame, true);
            }
            else if (AppPosition == 1)
            {
                AppPosition = 0;
                GoTo(MainPageFrame);
            }
            else
            {
                AppPosition -= 1;
                GoTo(OpenedApps[AppPosition - 1].Frame, true);
            }
        }

        // Naviguer vers une Frame spécifique. (Si ce n'est pas une app, on reinitialise la position à 0)
        public static void GoTo(Frame destination, bool isApp = false)
        {
            if (destination != Window.Current.Content)
            {
                if (!isApp)
                    AppPosition = 0;
                Window.Current.Content = destination;
                Window.Current.Activate();
                NavigatedEvent?.Invoke(GetCurrentPage());
            }
        }

        // Obtient la page courante
        public static Page GetCurrentPage()
        {
            Frame currentWindow = Window.Current.Content as Frame;
            return currentWindow.Content as Page;
        }

        #endregion
    }
}
