using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace NeoMir.Pages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
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

            void image_Tapped(object sender, TappedRoutedEventArgs e)
            {
                Image img = (Image)sender;
                DisplayCloseAppDialog(img.Tag as string);
            }

            void image_PointerExited(object sender, PointerRoutedEventArgs e)
            {
                Image img = (Image)sender;
                img.Height += ImageHoverSize;
                img.Width += ImageHoverSize;
            }

            void image_PointerEntered(object sender, PointerRoutedEventArgs e)
            {
                Image img = (Image)sender;
                img.Height -= ImageHoverSize;
                img.Width -= ImageHoverSize;
            }

            async void DisplayCloseAppDialog(string link)
            {
                ContentDialog contentDialog = new ContentDialog
                {
                    Title = "Fermer l'application",
                    Content = "Voulez-vous vraiment fermer cette application ?",
                    PrimaryButtonText = "Oui",
                    CloseButtonText = "Non"
                };

                ContentDialogResult result = await contentDialog.ShowAsync();

                // Annonce à l'utilisateur qu'il est sur le point de fermer une application
                /// Ne rien faire sinon.
                if (result == ContentDialogResult.Primary)
                {
                    // Fermer l'application choisie.
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
                    // Ne rien faire, l'utilisateur à pressé sur le boutton CloseButton.
                }
            }
        }

        private void CancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.AppManager.GoToApps();
        }
    }
}
