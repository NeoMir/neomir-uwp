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
        public static int ImageHoverSize;
        public static Thickness ImageMargin;
        private static int textMargin;
        private static int scrollViewerMargin;
        private string textRowName = "Pré-configuration ";
        private static int ConfNumber;
        private int lag = 300;
        private int transitionHorizontaloffset = 200;
        private bool removeState;

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

        private void InitializeVariables()
        {
            ImageSize = 210;
            ImageHoverSize = 20;
            ImageMargin = new Thickness(10);
            textMargin = 300;
            scrollViewerMargin = 350;
            ConfNumber = 0;
            removeState = false;
        }

        // Fonction qui affiche les applications ouvertes dans le sélecteur
        private void ListOpenApps()
        {
            try
            {
                int numberOfApps = Classes.AppManager.Apps.Count;

                if (numberOfApps == 0)
                {
                    // Création du texte si aucune application ouverte
                    TextBlock textBlock = new TextBlock();
                    textBlock.VerticalAlignment = VerticalAlignment.Top;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlock.Margin = new Thickness(0, 150, 0, 0);
                    textBlock.FontSize = 30;
                    textBlock.Text = "Aucune application ouverte";
                    AppsRows.Children.Add(textBlock);
                }
                else
                {
                    // Création de la liste XAML
                    ItemsControl itemsControl = CreateOpenAppsList();

                    // Ajout de chaque applications ouvertes dans la liste crée
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

                        itemsControl.Items.Add(image);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
        }

        // Fonction qui crée la première ligne d'applications => Applications ouvertes
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

            // Add to the grid
            AppsRows.Children.Add(scrollViewer);

            return ((ItemsControl)scrollViewer.Content);
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
                    img.Tag = "https://facebook.github.io/react-native/";
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

        // [Important] Pour le bon affichage des applications
        private ItemsPanelTemplate CreateTemplate()
        {
            string xaml = "<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'><WrapGrid Height='250'/></ItemsPanelTemplate>";
            return XamlReader.Load(xaml) as ItemsPanelTemplate;
        }

        //
        // EVENTS
        //

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("goToApps");
            if (imageAnimation != null)
            {
                imageAnimation.TryStart(BackButton);
            }
        }

        private void image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image img = (Image)sender;
            Classes.AppManager.AppPosition = (int)img.Tag + 1;
            Classes.AppManager.LaunchApp(Classes.AppManager.Apps[(int)img.Tag]);
        }

        private void image_Tapped2(object sender, TappedRoutedEventArgs e)
        {
            Image img = (Image)sender;
            Classes.AppManager.CreateApp((string)img.Tag);
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
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("goToMain", BackButton);
            Classes.AppManager.RootFrame.Navigate(typeof(MainPage));
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

        private void RemoveAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Classes.AppManager.Apps.Count > 0)
            {
                Classes.AppManager.PendingApp = "None";
                Classes.AppManager.RootFrame.Navigate(typeof(CloseAppPage));
            }
        }
    }
}
