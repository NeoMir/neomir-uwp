using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace NeoMir.Pages
{
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
            AppView.NavigationCompleted += AppView_NavigationCompleted;
        }

        //
        // METHODS
        //

        // Nothing for the moment //

        //
        // EVENTS
        //

        private void AppView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            //Classes.Communicate.CallFunction(sender, "Hello", null);
            //Classes.Communicate.InjectContent(sender, "document.getElementById('usernametest').innerHTML = 'Injection I. !!' ;");
            //Classes.Communicate.InjectContent(sender, "document.getElementById('passwordtest').innerHTML = 'Injection II. !!' ;");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Link = (string)e.Parameter;
            Uri uri = new Uri(this.Link);
            AppView.Source = uri;
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
            Classes.AppManager.GoTo(Classes.AppManager.MainPageFrame);
        }

        private void AppsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.AppManager.GoTo(Classes.AppManager.AppsPageFrame);
        }
    }
}
