using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace NeoMir.Pages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class AppsPage : Page
    {
        //
        // PROPERTIES
        //

        public static int ImageSize;
        public static int CloseAppImageSize;
        public static int ImageHoverSize;
        public static Thickness ImageMargin;
        private static int textMargin;
        private static int scrollViewerMargin;
        private string textRowName = "Pré-configuration ";
        private static int ConfNumber;
        private ItemsControl OpenAppsControl;
        private int lag = 300;
        private int transitionHorizontaloffset = 200;

        //
        // CONSTRUCTOR
        //

        public AppsPage()
        {
            this.InitializeComponent();
            this.InitializeVariables();
            ListOpenApps();
        }

        //
        // METHODS
        //

        /// <summary>
        /// Initialize the variables
        /// </summary>
        private void InitializeVariables()
        {
            ImageSize = 210;
            CloseAppImageSize = 500;
            ImageHoverSize = 20;
            ImageMargin = new Thickness(10);
            textMargin = 300;
            scrollViewerMargin = 350;
            ConfNumber = 0;
            OpenAppsControl = CreateOpenAppsList();
        }

        /// <summary>
        /// Display the open apps in the AppsPage
        /// </summary>
        private void ListOpenApps()
        {
            try
            {
                // Clear the first row
                OpenAppsControl.Items.Clear();
                int numberOfApps = Classes.AppManager.Apps.Count;

                if (numberOfApps == 0)
                {
                    NoOpenedAppsTitle.Visibility = Visibility.Visible;
                }
                else
                {
                    // Ajout de chaque applications ouvertes dans la liste crée
                    NoOpenedAppsTitle.Visibility = Visibility.Collapsed;
                    for (int i = 0; i < numberOfApps; i++)
                    {
                        Image image = new Image();
                        image.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple.png"));
                        image.Tag = i;
                        image.Height = ImageSize;
                        image.Width = ImageSize;
                        image.Margin = ImageMargin;
                        image.PointerEntered += new PointerEventHandler(image_PointerEntered);
                        image.PointerExited += new PointerEventHandler(image_PointerExited);
                        image.Tapped += new TappedEventHandler(image_Tapped);

                        OpenAppsControl.Items.Add(image);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
        }

        /// <summary>
        /// List the open apps for the close process
        /// </summary>
        /// <param name="itemsControl">The XAML Controls that receives the apps for display</param>
        private void ListOpenApps(ItemsControl itemsControl)
        {
            itemsControl.Items.Clear();
            int numberOfApps = Classes.AppManager.Apps.Count;

            for (int i = 0; i < numberOfApps; i++)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple1.png"));
                img.Tag = Classes.AppManager.Apps[i].Link;
                img.Height = CloseAppImageSize;
                img.Width = CloseAppImageSize;
                img.Margin = ImageMargin;
                img.VerticalAlignment = VerticalAlignment.Top;
                img.PointerEntered += new PointerEventHandler(image_PointerEntered);
                img.PointerExited += new PointerEventHandler(image_PointerExited);
                img.Tapped += new TappedEventHandler(image_Tapped3);

                itemsControl.Items.Add(img);
            }
        }

        /// <summary>
        /// Create the controls for the open apps
        /// </summary>
        /// <returns></returns>
        private ItemsControl CreateOpenAppsList()
        {
            ScrollViewer scrollViewer = new ScrollViewer();
            ItemsControl itemsControl = new ItemsControl();
            TransitionCollection transitions = new TransitionCollection();
            EntranceThemeTransition entranceThemeTransition = new EntranceThemeTransition();

            // ScrollViewer Configuration
            scrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
            scrollViewer.VerticalAlignment = VerticalAlignment.Top;
            scrollViewer.Margin = new Thickness(0, 50, 0, 0);

            entranceThemeTransition.FromHorizontalOffset = transitionHorizontaloffset;
            entranceThemeTransition.IsStaggeringEnabled = true;
            transitions.Add(entranceThemeTransition);
            itemsControl.ItemContainerTransitions = transitions;
            itemsControl.ItemsPanel = CreateTemplate();

            scrollViewer.Content = itemsControl;
            AppsRows.Children.Add(scrollViewer);

            return ((ItemsControl)scrollViewer.Content);
        }

        /// <summary>
        /// [Important] For the good display of the applications (Trick)
        /// </summary>
        /// <returns></returns>
        private ItemsPanelTemplate CreateTemplate()
        {
            string xaml = "<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'><WrapGrid Height='250'/></ItemsPanelTemplate>";
            return XamlReader.Load(xaml) as ItemsPanelTemplate;
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
                        backgroundCloseApp.Visibility = Visibility.Collapsed;
                        cancelButtonCloseApp.Visibility = Visibility.Collapsed;
                        titleCloseApp.Visibility = Visibility.Collapsed;
                        scrollviewerCloseApp.Visibility = Visibility.Collapsed;
                        if (Classes.AppManager.PendingApp != "None")
                        {
                            Classes.AppManager.CreateApp(Classes.AppManager.PendingApp);
                        }
                        ListOpenApps();
                        break;
                    }
                }
            }
            else
            {
                // DO nothing, the use pressed Cancel Button.
            }
        }

        /// <summary>
        /// When we reach the maximum of opened apps, display a dialog to ask the user to close or not an existing
        /// to be able to open a new one
        /// </summary>
        private async void DisplayMaximumAppDialog()
        {
            ContentDialog contentDialog = new ContentDialog
            {
                Title = "Maximum d'application ouvertes atteint !",
                Content = "Vous avez atteint le maximum d'applications ouvertes simultanément, vous devez choisir quelle application vous voulez fermer.",
                PrimaryButtonText = "Continuer",
                CloseButtonText = "Pas maintenant"
            };

            ContentDialogResult result = await contentDialog.ShowAsync();

            // Annonce à l'utilisateur qu'il doit choisir une application à fermer.
            /// Ne rien faire sinon.
            if (result == ContentDialogResult.Primary)
            {
                // Afficher les applications pour que l'utilisateur puisse choisir.
                backgroundCloseApp.Visibility = Visibility.Visible;
                cancelButtonCloseApp.Visibility = Visibility.Visible;
                titleCloseApp.Visibility = Visibility.Visible;
                scrollviewerCloseApp.Visibility = Visibility.Visible;
                ListOpenApps(itemsControlCloseApp);
            }
            else
            {
                // Ne rien faire, l'utilisateur à pressé sur le boutton CloseButton.
            }
        }

        // [Test] Fonction qui remplie des lignes d'applications avec de fausses applications
        private void FillApps(ItemsControl itemsControl)
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
                    img.Tag = "http://agls-app/";
                    numberImage++;
                }
                else if (numberImage == 1)
                {
                    img.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple1.png"));
                    img.Tag = "https://electronjs.org/";
                    numberImage++;
                }
                else if (numberImage == 2)
                {
                    img.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple2.png"));
                    img.Tag = "https://unity3d.com/fr";
                    numberImage++;
                }
                else if (numberImage == 3)
                {
                    img.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple3.png"));
                    img.Tag = "https://ionicframework.com/";
                    numberImage = 0;
                }

                img.Height = ImageSize;
                img.Width = ImageSize;
                img.Margin = ImageMargin;
                img.PointerEntered += new PointerEventHandler(image_PointerEntered);
                img.PointerExited += new PointerEventHandler(image_PointerExited);
                img.Tapped += new TappedEventHandler(image_Tapped2);

                itemsControl.Items.Add(img);
            }
        }

        // [Test] Fonction qui rajoute de manière automatique les différentes lignes de fausses configurations d'applications
        private void AddRowButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock textBlock = new TextBlock();
            ScrollViewer scrollViewer = new ScrollViewer();
            ItemsControl itemsControl = new ItemsControl();
            TransitionCollection transitions = new TransitionCollection();
            EntranceThemeTransition entranceThemeTransition = new EntranceThemeTransition();

            // TextBlock Configuration
            textBlock.Text = textRowName + ConfNumber;
            textBlock.FontSize = 25;
            textBlock.FontStyle = Windows.UI.Text.FontStyle.Italic;
            textBlock.Margin = new Thickness(20, textMargin, 0, 0);
            textBlock.VerticalAlignment = VerticalAlignment.Top;
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            ConfNumber++;
            textMargin += lag;

            // ScrollViewer Configuration
            scrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
            scrollViewer.VerticalAlignment = VerticalAlignment.Top;
            scrollViewer.Margin = new Thickness(0, scrollViewerMargin, 0, 0);
            scrollViewerMargin += lag;

            entranceThemeTransition.FromHorizontalOffset = transitionHorizontaloffset;
            entranceThemeTransition.IsStaggeringEnabled = true;
            transitions.Add(entranceThemeTransition);
            itemsControl.ItemContainerTransitions = transitions;
            itemsControl.ItemsPanel = CreateTemplate();

            FillApps(itemsControl);

            scrollViewer.Content = itemsControl;

            // Add all to the grid
            AppsRows.Children.Add(textBlock);
            AppsRows.Children.Add(scrollViewer);
        }

        //
        // EVENTS
        //

        private void image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image img = (Image)sender;
            Classes.AppManager.AppPosition = (int)img.Tag + 1;
            Classes.AppManager.LaunchApp(Classes.AppManager.Apps[(int)img.Tag]);
        }

        private void image_Tapped2(object sender, TappedRoutedEventArgs e)
        {
            Image img = (Image)sender;
            if (Classes.AppManager.Apps.Count < Classes.AppManager.MaxApp)
            {
                Classes.AppManager.CreateApp((string)img.Tag);
                ListOpenApps();
            }
            else
            {
                // Maximum apps reached, ask the user confirmation to replace one.
                Classes.AppManager.PendingApp = (string)img.Tag;
                DisplayMaximumAppDialog();
            }
        }

        // For the close process
        private void image_Tapped3(object sender, TappedRoutedEventArgs e)
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

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.AppManager.GoToHome();
        }

        private void RemoveAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Classes.AppManager.Apps.Count > 0)
            {
                Classes.AppManager.PendingApp = "None";
                backgroundCloseApp.Visibility = Visibility.Visible;
                cancelButtonCloseApp.Visibility = Visibility.Visible;
                titleCloseApp.Visibility = Visibility.Visible;
                scrollviewerCloseApp.Visibility = Visibility.Visible;
                ListOpenApps(itemsControlCloseApp);
            }
        }

        private void CancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            backgroundCloseApp.Visibility = Visibility.Collapsed;
            cancelButtonCloseApp.Visibility = Visibility.Collapsed;
            titleCloseApp.Visibility = Visibility.Collapsed;
            scrollviewerCloseApp.Visibility = Visibility.Collapsed;
        }

    }
}
