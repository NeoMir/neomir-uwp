using NeoMir.Classes.Communication;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace NeoMir.Pages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class AppPage : Page
    {
        private GestureCollector gestureCollector;
        //
        // PROPERTIES
        //

        public string Link { get; private set; }

        //
        // CONSTRUCTOR
        //

        public AppPage()
        {
            this.InitializeComponent();
            AppView.ScriptNotify += Classes.Communicate.ScriptNotify;
            GestureSetup();
        }

        //
        // METHODS
        //

        // Nothing for the moment //

        //
        // EVENTS
        //

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Link = (string)e.Parameter;
            Uri uri = new Uri(this.Link);
            AppView.Source = uri;
            gestureCollector.GestureCollected += ApplyGesture;
        }

        private void GestureSetup()
        {
            gestureCollector = GestureCollector.Instance;
        }

        private async void ApplyGesture(string gesture)
        {

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (this == Classes.AppManager.GetCurrentPage())
                {
                    if (gesture == "Next Right")
                    {
                        NextAppButton_Tapped(null, null);
                    }
                    else if (gesture == "Back")
                    {
                        PrevAppButton_Tapped(null, null);
                    }
                    else if (gesture == "Validate")
                    {
                        HomeButton_Tapped(null, null);
                    }
                }
            });
        }

        private void NextAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gestureCollector.GestureCollected -= ApplyGesture;
            Classes.AppManager.NextApp();
        }

        private void PrevAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gestureCollector.GestureCollected -= ApplyGesture;
            Classes.AppManager.PrevApp();
        }

        private void HomeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gestureCollector.GestureCollected -= ApplyGesture;
            Classes.AppManager.GoToHome();
        }

        private void AppsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gestureCollector.GestureCollected -= ApplyGesture;
            Classes.AppManager.GoToApps();
        }
    }
}
