using NeoMir.Classes;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using DataAccessLibrary;
using DataAccessLibrary.Entitites;
using DataAccessLibrary.API;
using NeoMir.UserManagment;
using NeoMir.Helpers;
using System.Threading.Tasks;
using static NeoMir.Classes.GlobalStatusManager;

namespace NeoMir
{
    /// <summary>
    /// Fournit un comportement spécifique à l'application afin de compléter la classe Application par défaut.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initialise l'objet d'application de singleton.  Il s'agit de la première ligne du code créé
        /// à être exécutée. Elle correspond donc à l'équivalent logique de main() ou WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoqué lorsque l'application est lancée normalement par l'utilisateur final.  D'autres points d'entrée
        /// seront utilisés par exemple au moment du lancement de l'application pour l'ouverture d'un fichier spécifique.
        /// </summary>
        /// <param name="e">Détails concernant la requête et le processus de lancement.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            await SetStatus();
            await UserManager.Instance.Init();
            UserAppsManager.Instance.Init();
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

            Frame rootFrame = Window.Current.Content as Frame;

            // Ne répétez pas l'initialisation de l'application lorsque la fenêtre comporte déjà du contenu,
            // assurez-vous juste que la fenêtre est active
            if (rootFrame == null)
            {
                // Créez un Frame utilisable comme contexte de navigation et naviguez jusqu'à la première page
                rootFrame = new Frame();
                Classes.FrameManager.MainPageFrame = rootFrame;
                Classes.FrameManager.AppsPageFrame = new Frame();
                Classes.FrameManager.LockPageFrame = new Frame();
                Classes.FrameManager.PairPageFrame = new Frame();
                Classes.FrameManager.CapturePage = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: chargez l'état de l'application précédemment suspendue
                }

                // Placez le frame dans la fenêtre active
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Quand la pile de navigation n'est pas restaurée, accédez à la première page,
                    // puis configurez la nouvelle page en transmettant les informations requises en tant que
                    // paramètre
                    rootFrame.Navigate(typeof(Pages.MainPage), e.Arguments);
                    Classes.FrameManager.AppsPageFrame.Navigate(typeof(Pages.AppsPage));
                    Classes.FrameManager.PairPageFrame.Navigate(typeof(Pages.ConnectToApi));
                    Classes.FrameManager.LockPageFrame.Navigate(typeof(Pages.LockScreenPage));
                    Classes.FrameManager.CapturePage.Navigate(typeof(Pages.TakePicturePage));
                }
                // Vérifiez que la fenêtre actuelle est active
                if (GlobalStatusManager.Instance.GlobalStatus == EGlobalStatus.FirstLaunch)
                {
                    Classes.FrameManager.GoTo(Classes.FrameManager.PairPageFrame);
                }
                else
                {
                    Classes.FrameManager.GoTo(Classes.FrameManager.LockPageFrame);
                }
                // Création du detecteur d'activité
                Classes.ActivityDetector activityDectector = new Classes.ActivityDetector();
            }
        }

        /// <summary>
        /// Appelé lorsque la navigation vers une page donnée échoue
        /// </summary>
        /// <param name="sender">Frame à l'origine de l'échec de navigation.</param>
        /// <param name="e">Détails relatifs à l'échec de navigation</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Appelé lorsque l'exécution de l'application est suspendue.  L'état de l'application est enregistré
        /// sans savoir si l'application pourra se fermer ou reprendre sans endommager
        /// le contenu de la mémoire.
        /// </summary>
        /// <param name="sender">Source de la requête de suspension.</param>
        /// <param name="e">Détails de la requête de suspension.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: enregistrez l'état de l'application et arrêtez toute activité en arrière-plan
            /*deferral = e.SuspendingOperation.GetDeferral();
            string navstate = Classes.AppManager.Apps[Classes.AppManager.AppPosition - 1].Frame.GetNavigationState();
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["nav"] = navstate;*/
            deferral.Complete();
        }

        private async Task SetStatus()
        {
            Miror miror = DataAccess.GetMiror();
            if (miror != null && miror.IsPaired)
            {
                GlobalStatusManager.Instance.GlobalStatus = EGlobalStatus.Paired;
            }
            else if (miror != null && !miror.IsPaired)
            {
                GlobalStatusManager.Instance.GlobalStatus = EGlobalStatus.FirstLaunch;
            }
            else
            {
                GlobalStatusManager.Instance.GlobalStatus = EGlobalStatus.FirstLaunch;
                string id = await APIManager.GetMirorId();
                DataAccess.AddEntity<Miror>(new Miror() { Id = id, IsPaired = false });
            }
        }
    }
}
