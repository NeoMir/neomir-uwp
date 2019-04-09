using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace NeoMir.Pages
{
    /// <summary>
    /// Allow to close an app already opened
    /// </summary>
    public sealed partial class CloseAppPage : Page
    {

        //
        // PROPERTIES
        //

        public static int ImageSize = 500;
        public static int ImageHoverSize = 20;
        public static Thickness ImageMargin = new Thickness(10);

        //
        // CONSTRUCTORS
        //

        public CloseAppPage()
        {
            this.InitializeComponent();
            ListOpenApps(OpenApps);
        }

        //
        // METHODS
        //


        /// <summary>
        /// List the open apps
        /// </summary>
        /// <param name="itemsControl">The XAML Controls that receives the apps for display</param>
        private void ListOpenApps(ItemsControl itemsControl)
        {
            int numberOfApps = Classes.AppManager.Apps.Count;

            for (int i = 0; i < numberOfApps; i++)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple1.png"));
                img.Tag = Classes.AppManager.Apps[i].Link;
                img.Height = ImageSize;
                img.Width = ImageSize;
                img.Margin = ImageMargin;
                img.VerticalAlignment = VerticalAlignment.Top;
                img.PointerEntered += new PointerEventHandler(image_PointerEntered);
                img.PointerExited += new PointerEventHandler(image_PointerExited);
                img.Tapped += new TappedEventHandler(image_Tapped);

                itemsControl.Items.Add(img);
            }
        }

        /// <summary>
        /// Display a dialog to prevent the user that he is about to close an app
        /// </summary>
        /// <param name="link">The link of the chosen app</param>
        async private void DisplayCloseAppDialog(string link)
        {
            ContentDialog contentDialog = new ContentDialog
            {
                Title = "Fermer l'application",
                Content = "Voulez-vous vraiment fermer cette application ?",
                PrimaryButtonText = "Oui",
                CloseButtonText = "Non"
            };

            ContentDialogResult result = await contentDialog.ShowAsync();

            // Display to the user that he is about to close an app
            if (result == ContentDialogResult.Primary)
            {
                // Close the chosen app
                foreach (Classes.App app in Classes.AppManager.Apps)
                {
                    if (app.Link == link)
                    {
                        Classes.AppManager.Apps.Remove(app);
                        if (Classes.AppManager.PendingApp == "None")
                        {
                            Classes.AppManager.GoToApps();
                        }
                        else
                        {
                            Classes.AppManager.CreateApp(Classes.AppManager.PendingApp);
                        }
                        break;
                    }
                }
            }
            else
            {
                // DO nothing, the use pressed Cancel Button.
            }
        }

        //
        // EVENTS
        //

        private void image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image img = (Image)sender;
            DisplayCloseAppDialog(img.Tag as string);
        }

        private void image_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Image img = (Image)sender;
            img.Height += ImageHoverSize;
            img.Width += ImageHoverSize;
        }

        private void image_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Image img = (Image)sender;
            img.Height -= ImageHoverSize;
            img.Width -= ImageHoverSize;
        }

        private void CancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.AppManager.GoToApps();
        }
    }
}
