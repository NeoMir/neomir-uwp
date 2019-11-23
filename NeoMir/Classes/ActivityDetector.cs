using System;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NeoMir.Classes
{
    // Class to detect activity based on a Periodic timer.
    public class ActivityDetector
    {
        #region PROPERTIES

        // Interval de temps (en secondes) où l'on veut verrouiller l'application lorsque l'utilisateur ne fait rien.
        private static int Period = 20000;
        // Même variable mais pour la boucle de détection
        private TimeSpan PeriodToLock;
        // Le timer périodique pour la boucle de détection
        private ThreadPoolTimer PeriodicTimer;
        // La date de dernière activité de l'utilisateur
        private DateTime LastActivity;
        // La frame de verrrouillage
        private Frame LockFrame;
        // Difference de temps entre maintenant et la dernière activité
        private double Difference;
        // Si l'application est verrouillé ou pas.
        private bool isLock;

        #endregion

        #region CONSTRUCTOR

        public ActivityDetector()
        {
            Initialize();
            StartDetection();
        }

        #endregion

        #region METHODS

        // Initialise les variables
        private void Initialize()
        {
            Window.Current.CoreWindow.PointerPressed += UpdateLastActivity;
            LastActivity = DateTime.Now;
            this.LockFrame = FrameManager.LockPageFrame;
            this.PeriodToLock = TimeSpan.FromSeconds(Period);
            this.isLock = true;
        }

        // Commence une boucle à intervalle de temps régulier si l'on doit verouiller ou pas
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

        // Met à jour la date et l'heure de la dernière activité
        private void UpdateLastActivity(CoreWindow sender, PointerEventArgs e)
        {
            if (isLock)
                isLock = !isLock;
            LastActivity = DateTime.Now;
        }

        #endregion
    }
}
