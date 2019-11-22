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
            }
        }

        private void CollectorSetup()
        {
            gestureCollector = GestureCollector.Instance;
            gestureCollector.RegisterToGestures(this, ApplyGesture);
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
            Carousel carousel = new Carousel();

            carousel.InvertPositive = false;
            carousel.ItemDepth = 400;
            carousel.ItemMargin = 0;
            carousel.ItemRotationX = 0;
            carousel.ItemRotationY = 0;
            carousel.ItemRotationZ = 0;
            carousel.SelectedIndex = 0;
            carousel.Orientation = Orientation.Horizontal;

            foreach (UserProfile profile in UserManager.Instance.Profiles)
            {
                Button button = new Button();
                button.Content = profile.Name;
                button.Tag = profile;
                button.FontSize = 50;
                button.FontStyle = Windows.UI.Text.FontStyle.Italic;
                button.FontWeight = Windows.UI.Text.FontWeights.Bold;
                button.Height = 300;
                button.Width = 1000;
                carousel.Items.Add(button);
            }

            Users.Children.Clear();
            Users.Children.Add(carousel);
        }

        #endregion

        #region EVENTS

        private void button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = (Button)sender;
            UserManager.Instance.CurrentProfile = (UserProfile)button.Tag;
            FrameManager.GoTo(FrameManager.MainPageFrame);
        }

        private void ApplyGesture(Gesture gesture)
        {
            if (!gesture.IsConsumed)
            {
                if (this == Classes.FrameManager.GetCurrentPage())
                {
                    if (gesture.Name == "Lock" && !gesture.IsConsumed)
                    {
                        if (UserManager.Instance.CurrentProfile == null)
                        {
                            UserManager.Instance.CurrentProfile = UserManager.Instance.Profiles[0];
                        }
                        Classes.FrameManager.GoTo(Classes.FrameManager.MainPageFrame);
                        gesture.IsConsumed = true;

                    }
                }
            }
        }

        private async void FaceDetected(Face face)
        {
            if (!face.IsConsumed)
            {

                if (this == Classes.FrameManager.GetCurrentPage())
                {
                    UserProfile profile = UserManager.Instance.Profiles.Where((u) => u.Name == face.Name).FirstOrDefault();
                    if (profile != null)
                    {
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

        #endregion
    }
}
