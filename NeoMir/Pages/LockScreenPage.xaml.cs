using DataAccessLibrary.Entitites;
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
using Windows.UI.Xaml.Navigation;

namespace NeoMir.Pages
{
    public sealed partial class LockScreenPage : Page
    {
        //
        // PROPERTIES
        //

        public static int HoverSize = 20;
        public static Thickness BoxMargin = new Thickness(10);
        private GestureCollector gestureCollector;
        private FaceCollector faceCollector;

        //
        // CONSTRUCTOR
        //
        public LockScreenPage()
        {
            this.InitializeComponent();
            StartBackgroundMedia();
            CollectorSetup();
            ListUsers();
        }

        //
        // METHODS
        //

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
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/MainPage/smoke.mp4"));
            _mediaPlayerElement.SetMediaPlayer(mediaPlayer);
            mediaPlayer.Play();
        }

        private void ListUsers()
        {
            Users.Items.Clear();
            foreach (UserProfile profile in UserManager.Instance.Profiles)
            {
                Button button = new Button();
                button.Content = profile.Name;
                button.Tag = profile;
                button.FontSize = 50;
                button.FontStyle = Windows.UI.Text.FontStyle.Italic;
                button.FontWeight = Windows.UI.Text.FontWeights.Bold;
                button.Height = 200;
                button.Width = 800;
                button.Opacity = 1;
                button.Margin = BoxMargin;
                button.PointerEntered += new PointerEventHandler(button_PointerEntered);
                button.PointerExited += new PointerEventHandler(button_PointerExited);
                button.Tapped += new TappedEventHandler(button_Tapped);

                Users.Items.Add(button);
            }
        }

        //
        // EVENTS
        //

        private void button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = (Button)sender;
            UserManager.Instance.CurrentProfile = (UserProfile)button.Tag;
            Classes.FrameManager.GoTo(Classes.FrameManager.MainPageFrame);
        }

        private void button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Height += HoverSize;
            button.Width += HoverSize;
        }

        private void button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Height -= HoverSize;
            button.Width -= HoverSize;
        }

        private async void ApplyGesture(Gesture gesture)
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
                        //MainScroll.Visibility = Visibility.Collapsed;
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
    }
}
