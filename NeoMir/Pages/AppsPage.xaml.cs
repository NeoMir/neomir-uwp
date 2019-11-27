using NeoMir.Classes.Com;
using NeoMir.Classes.Communication;
using NeoMir.Classes;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using DataAccessLibrary;
using DataAccessLibrary.Entitites;
using NeoMir.UserManagment;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoMir.Pages
{
    public sealed partial class AppsPage : Page
    {
        #region PROPERTIES

        private static int ConfNumber;
        private bool DoinitApp;
        private bool isLock;
        GestureCollector gestureCollector;
        private Dictionary<EGestures, Action> gestActions;
        private Carousel carousel;

        #endregion

        #region CONSTRUCTORS

        // Constructeur de la page
        public AppsPage()
        {
            this.InitializeComponent();
            this.InitializeVariables();
            UserManager.Instance.ProfileChanged += ProfileChanged;
            FrameManager.NavigatedEvent += NavigateOn;
            GestureSetup();
            StartAnimations();
        }

        #endregion

        #region METHODS

        // Débute les animations de la page
        private void StartAnimations()
        {
            new Animation(BackButton, 5000);
        }

        // Détecte s'il y a un changement de profil
        private void ProfileChanged()
        {
            FrameManager.InstalledApps.Clear();
            DoinitApp = true;
        }

        // Initialise les variables
        private void InitializeVariables()
        {
            ConfNumber = 0;
            isLock = false;
        }

        // Récupère toutes les applications installées
        private void GetAllInstalledApplication()
        {
            var mirorId = DataAccess.GetMiror().Id;
            foreach (UserApp app in DataAccess.GetEntities<UserApp>())
            {
                if (FrameManager.InstalledApps.Where((a) => a.UserApp.AppName == app.AppName).FirstOrDefault() == null)
                {
                    FrameManager.CreateApp(app);
                }
            }
        }

        // Affiche les applications
        private void DisplayApps()
        {
            int numberOfApps = FrameManager.InstalledApps.Count;
            carousel = new Carousel();

            carousel.InvertPositive = false;
            carousel.ItemDepth = 400;
            carousel.ItemMargin = 0;
            carousel.ItemRotationX = 0;
            carousel.ItemRotationY = 0;
            carousel.ItemRotationZ = 0;
            carousel.SelectedIndex = 0;
            carousel.Orientation = Orientation.Horizontal;

            if (numberOfApps == 0)
            {
                NoAppsTitle.Visibility = Visibility.Visible;
            }
            else
            {
                NoAppsTitle.Visibility = Visibility.Collapsed;
                foreach (Classes.App app in FrameManager.InstalledApps)
                {
                    Ellipse ellipse = new Ellipse();
                    ImageBrush imageBrush = new ImageBrush();

                    ellipse.Height = 700;
                    ellipse.Width = 700;
                    imageBrush.ImageSource = new BitmapImage(new Uri(app.UserApp.AppIconLink));
                    ellipse.Fill = imageBrush;
                    ellipse.Tapped += new TappedEventHandler(OpenAppTapped);
                    ellipse.Tag = app;
                    /*ellipse.Stroke = new SolidColorBrush(Colors.White);
                    ellipse.Stroke.Opacity = 0.8;
                    ellipse.StrokeThickness = 5;*/
                    carousel.Items.Add(ellipse);
                }
            }
            AppsCarousel.Children.Clear();
            AppsCarousel.Children.Add(carousel);
        }

        // Crétation d'une instance de collection de gestes
        private void GestureSetup()
        {
            gestureCollector = GestureCollector.Instance;
            gestureCollector.RegisterToGestures(this, ApplyGesture);
            gestureCollector.RegisterToGestureIcone(GestureIcone);
            InitGestureBehavior();
        }

        // Initialise un dictionnaire d'action qui serontt invoqué selon le geste détécté 
        private void InitGestureBehavior()
        {
            gestActions = new Dictionary<EGestures, Action>();
            gestActions.Add(EGestures.NextLeft, () => PreviousApp());
            gestActions.Add(EGestures.NextRight, () => NextApp());
            gestActions.Add(EGestures.Back, () => BackButton_Tapped(null, null));
            gestActions.Add(EGestures.Lock, () => GoToLockPage());
            gestActions.Add(EGestures.Validate, () => OpenApp());
        }

        // Applique les gestes
        private void ApplyGesture(Gesture gesture)
        {
            EGestures eg = (EGestures)Enum.Parse(typeof(EGestures), gesture.Name);
            if (gestActions.ContainsKey(eg))
            {
                gestActions[eg].Invoke();
            }
        }

        // Charge les applications du profil
        private void LoadProfilApps()
        {
            if (DoinitApp)
            {
                FrameManager.InstalledApps.Clear();
                GetAllInstalledApplication();
                DoinitApp = false;
            }
        }

        #endregion

        #region EVENTS

        // Evenement pour ouvrir une application
        private void OpenAppTapped(object sender, TappedRoutedEventArgs e)
        {
            Ellipse img = (Ellipse)sender;
            FrameManager.LaunchApp((Classes.App)img.Tag);
        }

        // Evenement pour retourner sur la page d'accueil
        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FrameManager.GoTo(FrameManager.MainPageFrame); 
        }

        // Evenement de navigation
        private void NavigateOn(Page page)
        {
            if (page == this)
            {
                LoadProfilApps();
                DisplayApps();
            }
        }


        // Va a la page de verouillage
        private void GoToLockPage()
        {
            FrameManager.GoTo(FrameManager.LockPageFrame);
        }


        private async void NextApp()
        {
            if (carousel.Items.Count > 0)
            {
                carousel.SelectedIndex = (carousel.SelectedIndex + 1) % carousel.Items.Count;
            }
            else
            {
                await RefreshApps();
            }
        }

        private async void PreviousApp()
        {
            if (carousel.Items.Count > 0)
            {
                if (carousel.SelectedIndex == 0)
                {
                    carousel.SelectedIndex = -1;
                    await RefreshApps();
                }
                else
                {
                    carousel.SelectedIndex = (carousel.SelectedIndex - 1) % carousel.Items.Count;
                }
            }
            else
            {
                await RefreshApps();
            }
        }



        private async Task RefreshApps()
        {
            DoinitApp = true;
            RefreshTxt.Visibility = Visibility.Visible;
            await UserAppsManager.Instance.GetAppsForProfil();
            LoadProfilApps();
            DisplayApps();
            carousel.SelectedIndex = 0;
            RefreshTxt.Visibility = Visibility.Collapsed;
        }


        private void OpenApp()
        {
            if (carousel.Items.Count > 0)
            {
                Ellipse img = (Ellipse)carousel.Items[carousel.SelectedIndex];
                FrameManager.LaunchApp((Classes.App)img.Tag);
            }
        }
        #endregion
    }
}
