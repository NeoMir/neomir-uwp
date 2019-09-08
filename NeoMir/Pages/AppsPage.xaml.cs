using NeoMir.Classes.Com;
using NeoMir.Classes.Communication;
using NeoMir.Classes;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using DataAccessLibrary;
using DataAccessLibrary.Entitites;
using NeoMir.Helpers;
using NeoMir.UserManagment;

namespace NeoMir.Pages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    /// 
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
        private string textRowName = "Mes Applications";
        private static int ConfNumber;
        private ItemsControl openAppsControl;
        private int lag = 300;
        private int transitionHorizontaloffset = 200;
        private ItemsControl installedAppControl;
        GestureCollector gestureCollector;
        bool isLock;

        private static bool flag = false;
        //private static ItemsControl itemsControl = new ItemsControl();
        //
        // CONSTRUCTOR
        //

        public AppsPage()
        {
            this.InitializeComponent();
            this.InitializeVariables();
            isLock = false;
            //ListOpenApps();
            InitInstalledAppsLayout();
            GestureSetup();
            GetAllInstalledApplication();
            UserManager.Instance.ProfileChanged += LoadProfilApps;
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
            openAppsControl = CreateOpenAppsList();
        }

        private async Task GetAllInstalledApplication()
        {
            var mirorId = DataAccess.GetMiror().Id;
            Classes.FrameManager.InstalledApps.Clear();
            foreach (UserApp app in DataAccess.GetEntities<UserApp>())
            {
                FrameManager.CreateInstalledApp(app);
            }
            //var id = "";
            //var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///id/id.txt"));
            //using (var inputStream = await file.OpenReadAsync())
            //using (var classicStream = inputStream.AsStreamForRead())
            //using (var streamReader = new StreamReader(classicStream))
            //{
            //    id = streamReader.ReadToEnd();
            //}
            //var http = new HttpClient();
            ////var url = String.Format("http://www.martinbaud.com/V1/getAppInfo.php?id_mirror=" + id);
            //var url = String.Format("http://www.martinbaud.com/V1/getAppListFromProfil.php?email=test@test.com&id_profil=2");
            //var response = await http.GetAsync(url);
            //var result = await response.Content.ReadAsStringAsync();
            //string[] links = result.Split(' ');
            //Classes.FrameManager.InstalledApps.Clear();

            //foreach (string link in links)
            //{
            //    if (link != "")
            //    {
            //        FrameManager.CreateInstalledApp(link);
            //    }
            //}
        }


        /// <summary>
        /// Display the open apps in the AppsPage
        /// </summary>
        private void ListOpenApps()
        {
            try
            {
                // Clear the first row
                openAppsControl.Items.Clear();
                int numberOfApps = Classes.FrameManager.Apps.Count;

                if (numberOfApps == 0)
                {
                    NoOpenedAppsTitle.Visibility = Visibility.Visible;
                }
                else
                {
                    // Ajout de chaque applications ouvertes dans la liste crée
                    NoOpenedAppsTitle.Visibility = Visibility.Collapsed;
                    foreach (Classes.App app in FrameManager.Apps)
                    {
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri(app.UserApp.AppIconLink));
                        img.Height = ImageSize;
                        img.Width = ImageSize;
                        img.Margin = ImageMargin;
                        img.PointerEntered += new PointerEventHandler(image_PointerEntered);
                        img.PointerExited += new PointerEventHandler(image_PointerExited);
                        img.Tapped += new TappedEventHandler(OpenAppTaped);
                        img.Tag = app;

                        openAppsControl.Items.Add(img);
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
            int numberOfApps = Classes.FrameManager.Apps.Count;

            foreach (Classes.App app in FrameManager.Apps)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(app.UserApp.AppIconLink));
                img.Height = ImageSize;
                img.Width = ImageSize;
                img.Margin = ImageMargin;
                img.PointerEntered += new PointerEventHandler(image_PointerEntered);
                img.PointerExited += new PointerEventHandler(image_PointerExited);
                img.Tapped += new TappedEventHandler(CloseAppTapped);
                img.Tag = app;

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
        async private void DisplayCloseAppDialog(Classes.App app)
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
                foreach (Classes.App appli in Classes.FrameManager.Apps)
                {
                    if (appli == app)
                    {
                        Classes.FrameManager.Apps.Remove(app);
                        backgroundCloseApp.Visibility = Visibility.Collapsed;
                        cancelButtonCloseApp.Visibility = Visibility.Collapsed;
                        titleCloseApp.Visibility = Visibility.Collapsed;
                        scrollviewerCloseApp.Visibility = Visibility.Collapsed;
                        if (Classes.FrameManager.PendingApp != "None")
                        {
                            Classes.FrameManager.CreateApp(Classes.FrameManager.PendingApp);
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
            itemsControl.Items.Clear();
            foreach (Classes.App app in FrameManager.InstalledApps)
            {
                if (app.UserApp.ProfileId == 0 || app.UserApp.ProfileId == UserManager.Instance.CurrentProfile.Id)
                {
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(app.UserApp.AppIconLink));
                    img.Height = ImageSize;
                    img.Width = ImageSize;
                    img.Margin = ImageMargin;
                    img.PointerEntered += new PointerEventHandler(image_PointerEntered);
                    img.PointerExited += new PointerEventHandler(image_PointerExited);
                    img.Tapped += new TappedEventHandler(InstaledAppTaped);
                    img.Tag = app;

                    itemsControl.Items.Add(img);
                }
            }
        }

        private void GestureSetup()
        {
            gestureCollector = GestureCollector.Instance;
            gestureCollector.RegisterToGestures(this, ApplyGesture);
        }

        private async void ApplyGesture(Gesture gesture)
        {
            if (!gesture.IsConsumed)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (this == Classes.FrameManager.GetCurrentPage() && !isLock)
                    {
                        if (gesture.Name == "Next Left" && !gesture.IsConsumed)
                        {
                            BackButton_Tapped(null, null);
                            gesture.IsConsumed = true;
                        }
                        else if (gesture.Name == "Next Right" && ConfNumber >= 1)
                        {
                            OpenAppWithGesture();
                            gesture.IsConsumed = true;
                        }
                    }
                    if (gesture.Name == "Lock")
                    {
                        isLock = !isLock;
                    }
                });
            }
        }

        //
        // EVENTS
        //

        private void image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image img = (Image)sender;
            Classes.FrameManager.AppPosition = (int)img.Tag + 1;
            Classes.FrameManager.LaunchApp(Classes.FrameManager.Apps[(int)img.Tag]);
        }

        private void InstaledAppTaped(object sender, TappedRoutedEventArgs e)
        {
            Image img = (Image)sender;
            if (Classes.FrameManager.Apps.Count < Classes.FrameManager.MaxApp)
            {

                Classes.FrameManager.LaunchInstalledApp((Classes.App)img.Tag);
                ListOpenApps();
            }
            else
            {
                Classes.App app = (Classes.App)img.Tag;
                // Maximum apps reached, ask the user confirmation to replace one.
                Classes.FrameManager.PendingApp = app.UserApp.AppLink;
                DisplayMaximumAppDialog();
            }
        }

        private void OpenAppTaped(object sender, TappedRoutedEventArgs e)
        {
            Image img = (Image)sender;
            Classes.FrameManager.LaunchInstalledApp((Classes.App)img.Tag);
        }



        private void OpenAppWithGesture()
        {
            string tag = "https://unity3d.com/fr";
            if (Classes.FrameManager.Apps.Count < Classes.FrameManager.MaxApp)
            {
                Classes.FrameManager.CreateApp(tag);
                ListOpenApps();
            }
            else
            {
                // Maximum apps reached, ask the user confirmation to replace one.
                Classes.FrameManager.PendingApp = tag;
                DisplayMaximumAppDialog();
            }
        }

        // For the close process
        private void CloseAppTapped(object sender, TappedRoutedEventArgs e)
        {
            Image img = (Image)sender;
            DisplayCloseAppDialog(img.Tag as Classes.App);
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
            Classes.FrameManager.GoTo(Classes.FrameManager.MainPageFrame);
        }

        private void RemoveAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Classes.FrameManager.Apps.Count > 0)
            {
                Classes.FrameManager.PendingApp = "None";
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

        private void SynchronizeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //TODO call API to get profile apps again
        }

        private void LoadProfilApps()
        {
            FrameManager.Apps.Clear();
            openAppsControl.Items.Clear();
            FillApps(installedAppControl);
        }

        /// <summary>
        /// Init the scroolviewer for display all installed apps
        /// </summary>
        private void InitInstalledAppsLayout()
        {
            TextBlock textBlock = new TextBlock();
            ScrollViewer scrollViewer = new ScrollViewer();
            installedAppControl = new ItemsControl();
            TransitionCollection transitions = new TransitionCollection();
            EntranceThemeTransition entranceThemeTransition = new EntranceThemeTransition();

            // TextBlock Configuration
            textBlock.Text = textRowName;
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
            installedAppControl.ItemContainerTransitions = transitions;
            installedAppControl.ItemsPanel = CreateTemplate();

            scrollViewer.Content = installedAppControl;

            // Add all to the grid
            AppsRows.Children.Add(textBlock);
            AppsRows.Children.Add(scrollViewer);
        }
    }
}
