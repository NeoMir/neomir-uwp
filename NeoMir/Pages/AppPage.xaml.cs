using NeoMir.Classes.Com;
using NeoMir.Classes.Communication;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using NeoMir.Classes;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace NeoMir.Pages
{
    public sealed partial class AppPage : Page
    {
        #region PROPERTIES

        private GestureCollector gestureCollector;
        private bool isLock;
        private Dictionary<EGestures, Action> gestActions;
        public string Link { get; private set; }

        #endregion

        #region CONSTRUCTOR

        public AppPage()
        {
            this.InitializeComponent();
            isLock = false;
            AppView.ScriptNotify += Classes.Communicate.ScriptNotify;
            AppView.NavigationCompleted += AppView_NavigationCompleted;
            StartAnimations();
            GestureSetup();
        }

        #endregion

        #region METHODS

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
            InitGestureBehavior();
            gestureCollector.RegisterToGestures(this, ApplyGesture);
            gestureCollector.RegisterToGestureIcone(GestureIcone);
        }


        // Initialise un dictionnaire d'action qui serontt invoqué selon le geste détécté 
        private void InitGestureBehavior()
        {
            gestActions = new Dictionary<EGestures, Action>();
            gestActions.Add(EGestures.NextLeft, () => PrevAppButton_Tapped(null, null));
            gestActions.Add(EGestures.NextRight, () => NextAppButton_Tapped(null, null));
            gestActions.Add(EGestures.Lock, () => GoToLockPage());
            gestActions.Add(EGestures.Validate, () => AppsButton_Tapped(null, null));
            gestActions.Add(EGestures.Back, () => GoToMainPage());
            gestActions.Add(EGestures.Stop, () => IsLock.Visibility = IsLock.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        // Applique les gestes
        private void ApplyGesture(Gesture gesture)
        {
            EGestures eg = (EGestures)Enum.Parse(typeof(EGestures), gesture.Name);
            if (gestActions.ContainsKey(eg))
            {
                gestActions[eg].Invoke();
            }
        }

        #endregion

        #region EVENTS

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

        // Va a la page de verouillage
        private void GoToLockPage()
        {
            FrameManager.GoTo(FrameManager.LockPageFrame);
        }

        // Va a la page de verouillage
        private void GoToMainPage()
        {
            FrameManager.GoTo(FrameManager.MainPageFrame);
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

        #endregion
    }
}
