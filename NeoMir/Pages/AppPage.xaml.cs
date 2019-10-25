using NeoMir.Classes.Com;
using NeoMir.Classes.Communication;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using NeoMir.Classes;

namespace NeoMir.Pages
{
    public sealed partial class AppPage : Page
    {
        //
        // PROPERTIES
        //

        private GestureCollector gestureCollector;
        private bool isLock;
        public string Link { get; private set; }

        //
        // CONSTRUCTOR
        //

        public AppPage()
        {
            this.InitializeComponent();
            isLock = false;
            AppView.ScriptNotify += Classes.Communicate.ScriptNotify;
            AppView.NavigationCompleted += AppView_NavigationCompleted;
            StartAnimations();
            GestureSetup();
        }

        //
        // METHODS
        //

        private void StartAnimations()
        {
            new Animation(NextAppButton, 5000);
            new Animation(HomeButton, 5000);
            new Animation(AppsButton, 5000);
            new Animation(PrevAppButton, 5000);
        }

        private void GestureSetup()
        {
            gestureCollector = GestureCollector.Instance;
            gestureCollector.RegisterToGestures(this, ApplyGesture);
        }

        private async void ApplyGesture(Gesture gesture)
        {
            if (this == Classes.FrameManager.GetCurrentPage() && !isLock)
            {
                if (gesture.Name == "Next Right" && !gesture.IsConsumed)
                {
                    NextAppButton_Tapped(null, null);
                    gesture.IsConsumed = true;
                }
                else if (gesture.Name == "Back" && !gesture.IsConsumed)
                {
                    PrevAppButton_Tapped(null, null);
                    gesture.IsConsumed = true;
                }
                else if (gesture.Name == "Validate" && !gesture.IsConsumed)
                {
                    HomeButton_Tapped(null, null);
                    gesture.IsConsumed = true;
                }
            }
            if (gesture.Name == "Lock")
            {
                isLock = !isLock;
            }
        }

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
            Classes.FrameManager.NextApp();
        }

        private void PrevAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.FrameManager.PrevApp();
        }

        private void HomeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.FrameManager.GoTo(Classes.FrameManager.MainPageFrame);
        }

        private void AppsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.FrameManager.GoTo(Classes.FrameManager.AppsPageFrame);
        }
    }
}
