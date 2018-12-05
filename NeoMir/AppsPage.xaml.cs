using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
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
            ImageSize = 210;
            ImageHoverSize = 20;
            ImageMargin = new Thickness(10);
            FillApps(OpenApps);
        }

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
                    numberImage++;
                }
                else if (numberImage == 1)
                {
                    img.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple1.png"));
                    numberImage++;
                }
                else if (numberImage == 2)
                {
                    img.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple2.png"));
                    numberImage++;
                }
                else if (numberImage == 3)
                {
                    img.Source = new BitmapImage(new Uri("ms-appx:///Assets/AppsPage/exemple3.png"));
                    numberImage = 0;
                }

                img.Height = ImageSize;
                img.Width = ImageSize;
                img.Margin = ImageMargin;
                img.PointerEntered += new PointerEventHandler(image_PointerEntered);
                img.PointerExited += new PointerEventHandler(image_PointerExited);

                itemsControl.Items.Add(img);
            }
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("goToApps");
            if (imageAnimation != null)
            {
                imageAnimation.TryStart(BackButton);
            }
        }

        private void BackButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            BackButton.Height += 10;
            BackButtonName.FontSize += 3;
        }

        private void BackButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
           BackButton.Height -= 10;
            BackButtonName.FontSize -= 3;
        }

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            textMargin = 300;
            scrollViewerMargin = 350;
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("goToMain", BackButton);
            this.Frame.Navigate(typeof(MainPage));
        }

        //
        // BEGIN test dynamic adding
        //
        private static int textMargin = 300;
        private static int scrollViewerMargin = 350;
        private string textRowName = "Pré-configuration";
        private static int ConfNumber = 0;
        private int lag = 300;
        private int transitionHorizontaloffset = 200;
        private int rowsSize = 250;

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

        // Trick pour ajouter le panel wrapgrid difficile autrement
        private ItemsPanelTemplate CreateTemplate()
        {
            string xaml = "<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'><WrapGrid Height='250'/></ItemsPanelTemplate>";
            return XamlReader.Load(xaml) as ItemsPanelTemplate;
        }
        //
        // END test dynamic adding
        //

        private void AddRowButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AddRowButton.Height += 10;
        }

        private void AddRowButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            AddRowButton.Height -= 10;
        }
    }
}
