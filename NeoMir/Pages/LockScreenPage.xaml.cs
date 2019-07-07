using NeoMir.Classes.Com;
using NeoMir.Classes.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
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
        public static bool IsShowed = false;
        GestureCollector gestureCollector;

        //
        // CONSTRUCTOR
        //
        public LockScreenPage()
        {
            this.InitializeComponent();
            StartBackgroundMedia();
            Classes.UserManager.Users.Clear();
            Classes.UserManager.Users.Add(new Classes.User("Robin", "DACALOR"));
            Classes.UserManager.Users.Add(new Classes.User("Ambroise", "DAMIER"));
            Classes.UserManager.Users.Add(new Classes.User("Martin", "BAUD"));
            GestureSetup();
        }

        //
        // METHODS
        //

        private void GestureSetup()
        {
            gestureCollector = GestureCollector.Instance;
            gestureCollector.RegisterToGestures(this, ApplyGesture);
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
            int numberOfUsers = Classes.UserManager.Users.Count;

            for (int i = 0; i < numberOfUsers; i++)
            {
                Button button = new Button();

                button.Content = Classes.UserManager.Users[i].FirstName + ' ' + Classes.UserManager.Users[i].LastName;
                button.Tag = Classes.UserManager.Users[i];
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
            Classes.UserManager.currentUser = (Classes.User)button.Tag;
            IsShowed = false;
            MainScroll.Visibility = Visibility.Collapsed;
            Users.Items.Clear();
            Classes.AppManager.GoTo(Classes.AppManager.MainPageFrame);
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

        private void _mediaPlayerElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainScroll.Visibility = Visibility.Visible;
            if (!IsShowed)
                ListUsers();
            IsShowed = true;

        }

        private async void ApplyGesture(Gesture gesture)
        {
            if (!gesture.IsConsumed)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (this == Classes.AppManager.GetCurrentPage())
                    {
                        if (gesture.Name == "Lock" && !gesture.IsConsumed)
                        {
                            IsShowed = false;
                            MainScroll.Visibility = Visibility.Collapsed;
                            Users.Items.Clear();
                            Classes.AppManager.GoTo(Classes.AppManager.MainPageFrame);
                            gesture.IsConsumed = true;
                        }
                    }
                });
            }
        }
    }
}
