using DataAccessLibrary.Entitites;
using Microsoft.Toolkit.Uwp.UI.Controls;
using NeoMir.Classes;
using NeoMir.Classes.Com;
using NeoMir.Classes.Communication;
using NeoMir.UserManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace NeoMir.Pages
{
    public sealed partial class LockScreenPage : Page
    {
        #region PROPERTIES

        public static int HoverSize = 20;
        public static Thickness BoxMargin = new Thickness(10);
        private GestureCollector gestureCollector;
        private FaceCollector faceCollector;
        private Dictionary<EGestures, Action> gestActions;
        Carousel carousel;

        #endregion

        #region CONSTRUCTOR

        public LockScreenPage()
        {
            this.InitializeComponent();
            FrameManager.NavigatedEvent += NavigateOn;
            StartBackgroundMedia();
            CollectorSetup();
        }

        #endregion

        #region METHODS

        private async void NavigateOn(Page page)
        {
            if (page == this)
            {
                Users.Visibility = Visibility.Collapsed;
                await UserManager.Instance.Init();
                DisplayUsers();
                Users.Visibility = Visibility.Visible;
                await GlobalMessageManager.Instance.SendMessageAsync(Protocol.StartFace);
            }
        }

        private void CollectorSetup()
        {
            gestureCollector = GestureCollector.Instance;
            gestureCollector.RegisterToGestures(this, ApplyGesture);
            gestureCollector.RegisterToGestureIcone(GestureIcone);
            InitGestureBehavior();
            faceCollector = FaceCollector.Instance;
            faceCollector.RegisterToFace(this, FaceDetected);
        }

        private void StartBackgroundMedia()
        {
            var mediaPlayer = new MediaPlayer();
            mediaPlayer.IsLoopingEnabled = true;
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/LockPage/smoke.mp4"));
            _mediaPlayerElement.SetMediaPlayer(mediaPlayer);
            mediaPlayer.Play();
        }

        private void DisplayUsers()
        {
            carousel = new Carousel();

            carousel.InvertPositive = false;
            carousel.ItemDepth = 400;
            carousel.ItemMargin = 0;
            carousel.ItemRotationX = 0;
            carousel.ItemRotationY = 0;
            carousel.ItemRotationZ = 0;
            carousel.Orientation = Orientation.Horizontal;



            foreach (UserProfile profile in UserManager.Instance.Profiles)
            {
                Button button = new Button();
                button.Content = profile.Name;
                button.Tag = profile;
                button.FontSize = 50;
                button.FontStyle = Windows.UI.Text.FontStyle.Italic;
                button.FontWeight = Windows.UI.Text.FontWeights.Bold;
                button.Tapped += new TappedEventHandler(button_Tapped);
                button.Height = 300;
                button.Width = 1000;
                carousel.Items.Add(button);
            }

            Users.Children.Clear();
            Users.Children.Add(carousel);
            carousel.SelectedIndex = 0;
        }

        #endregion

        #region EVENTS

        private void button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = (Button)sender;
            UserManager.Instance.CurrentProfile = (UserProfile)button.Tag;
            FrameManager.GoTo(FrameManager.MainPageFrame);
        }

        // Initialise un dictionnaire d'action qui serontt invoqué selon le geste détécté 
        private void InitGestureBehavior()
        {
            gestActions = new Dictionary<EGestures, Action>();
            gestActions.Add(EGestures.NextLeft, () => PreviousProfile());
            gestActions.Add(EGestures.NextRight, () => NextProfile());
            gestActions.Add(EGestures.Validate, () => OpenProfile());
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

        // Ouvre le profil dont la face a été detécté
        private async void FaceDetected(Face face)
        {
            if (!face.IsConsumed)
            {

                if (this == Classes.FrameManager.GetCurrentPage())
                {
                    UserProfile profile = UserManager.Instance.Profiles.Where((u) => u.Name == face.Name).FirstOrDefault();
                    if (profile != null)
                    {
                        await GlobalMessageManager.Instance.SendMessageAsync(Protocol.StopFace);
                        this.DetectedMessage.Text = string.Format("{0} has been detected", face.Name);
                        this.DetectedMessage.Text = string.Empty;
                        UserManager.Instance.CurrentProfile = profile;
                        face.IsConsumed = true;
                        await Task.Delay(2000);
                        Classes.FrameManager.GoTo(Classes.FrameManager.MainPageFrame);
                    }
                    else
                    {
                        this.DetectedMessage.Foreground = new SolidColorBrush(Colors.Red);
                        this.DetectedMessage.Text = string.Format("{0} is not a valid user", face.Name);
                        await Task.Delay(2000);
                        this.DetectedMessage.Text = string.Empty;
                        this.DetectedMessage.Foreground = new SolidColorBrush(Colors.ForestGreen);
                    }
                }
            }
        }

        private async void NextProfile()
        {
            if (carousel.Items.Count > 0)
            {
                carousel.SelectedIndex = (carousel.SelectedIndex + 1) % carousel.Items.Count;
            }
            else
            {
                await RefreshProfiles();
            }
        }

        private async void PreviousProfile()
        {
            if (carousel.Items.Count > 0)
            {
                if (carousel.SelectedIndex == 0)
                {
                    carousel.SelectedIndex = -1;
                    await RefreshProfiles();
                }
                else
                {
                    carousel.SelectedIndex = (carousel.SelectedIndex - 1) % carousel.Items.Count;
                }
            }
            else
            {
                await RefreshProfiles();
            }
        }

        private async Task RefreshProfiles()
        {
            await UserManager.Instance.Init();
            DisplayUsers();
            carousel.SelectedIndex = 0;
        }

        private async void OpenProfile()
        {
            UserProfile profile = UserManager.Instance.Profiles.Where(p => p.Name == (string)(carousel.Items[carousel.SelectedIndex] as Button).Content).FirstOrDefault();
            if (profile != null && !profile.IsFaceLinked)
            {
                await GlobalMessageManager.Instance.SendMessageAsync(Protocol.StopFace);
                button_Tapped(carousel.SelectedItem, null);
            }
        }

        #endregion
    }
}
