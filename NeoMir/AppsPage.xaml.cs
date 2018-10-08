using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace NeoMir
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class AppsPage : Page
    {
        public static int ImageSize { get; private set; }
        public static int ImageHoverSize { get; private set; }
        public static Thickness ImageMargin { get; private set; }

        public AppsPage()
        {
            this.InitializeComponent();
            ImageSize = 350;
            ImageHoverSize = 330;
            ImageMargin = new Thickness(10);
            ListApps();
        }

        private void ListApps()
        {
            int numberOfApps = 20;

            // test for images
            int numberImage = 0;

            for (int i = 0; i < numberOfApps; i++)
            {
                Image img = new Image();
                if (numberImage == 0)
                {
                    img.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple.png"));
                    img.Name = "mailto:";
                    numberImage++;
                }
                else if (numberImage == 1)
                {
                    img.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple1.png"));
                    img.Name = "ms-settings:";
                    numberImage++;
                }
                else if (numberImage == 2)
                {
                    img.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple2.png"));
                    img.Name = "ms-windows-store://home/";
                    numberImage++;
                }
                else if (numberImage == 3)
                {
                    img.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple3.png"));
                    img.Name = "bingmaps:";
                    numberImage = 0;
                }
                img.Height = ImageSize;
                img.Width = ImageSize;
                img.Margin = ImageMargin;
                img.PointerEntered += new PointerEventHandler(image_PointerEntered);
                img.PointerExited += new PointerEventHandler(image_PointerExited);
                img.Tapped += new TappedEventHandler(image_Tapped);
                rectangleItems.Items.Add(img);
            }
        }

        private void image_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Image img = (Image)sender;
            img.Height = ImageSize;
            img.Width = ImageSize;
        }

        private void image_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Image img = (Image)sender;
            img.Height = ImageHoverSize;
            img.Width = ImageHoverSize;
        }

        private async void image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image img = (Image)sender;
            img.Height = ImageSize;

            // We can use custom URI of the App that we want to launch
            string uriString = string.Format(img.Name);
            Uri uri = new Uri(uriString);
            await Launcher.LaunchUriAsync(uri);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("goToApps");
            if (imageAnimation != null)
            {
                imageAnimation.TryStart(cube_logo);
            }
        }

        private void cube_logo_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            cube_logo.Height = 110;
        }

        private void cube_logo_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            cube_logo.Height = 100;
        }

        private void cube_logo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("goToMain", cube_logo);
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
