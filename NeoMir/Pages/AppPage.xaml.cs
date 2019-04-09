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
        }

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Window.Current.Content = Classes.AppManager.RootFrame;
            Window.Current.Activate();
        }

        private void NextAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.AppManager.NextApp();
        }

        private void PrevAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.AppManager.PrevApp();
        }

        private void HomeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.AppManager.GoToHome();
        }

        private void AppsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.AppManager.GoToApps();
        }
    }
}
