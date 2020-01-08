using DataAccessLibrary;
using DataAccessLibrary.API;
using DataAccessLibrary.Entitites;
using NeoMir.Classes;
using NeoMir.UserManagment;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

using static NeoMir.Classes.GlobalStatusManager;

namespace NeoMir.Pages
{
    public sealed partial class MirorLink : Page
    {
        #region PROPERTIES

        private string id;

        #endregion

        #region METHODS

        public MirorLink()
        {
            this.InitializeComponent();
            CheckStatus();
            ShowId();
            // Logo.Source = new BitmapImage(new Uri("ms-appx:///Assets/vignette.scale-400.png"));
        }

        // Montre l'identifiant à l'utilisateur
        private void ShowId()
        {
            MirorID.Text = DataAccess.GetMiror()?.Token;
        }

        // Récupère l'ID depuis l'API
        private async Task GetIdFromDatabase()
        {
            id = await APIManager.GetMirorId();
        }

        // Attente de la validation du lient entre le miroir et l'application par l'API
        private async void WaitForLink()
        {
            Miror miror = DataAccess.GetMiror();

            while (true)
            {
                 miror = await APIManager.GetIsLinked(DataAccess.GetMiror().Id);
                if (miror.IsLinked)
                {
                    //BackHomePanel.Visibility = Visibility.Visible;
                    LinkDone.Visibility = Visibility.Visible;
                    DataAccess.UpdateEntity(miror);
                    GlobalStatusManager.Instance.GlobalStatus = EGlobalStatus.Paired;
                    UserManager.Instance.CurrentUser = miror.UserParent;
                    await UserManager.Instance.Init();
                    await Task.Delay(2000);
                    break;
                }
                await Task.Delay(2000);
                //id = "5b8f";
                //TODO : Appel API popur savoir si on est pair a un compte
                //await Task.Delay(1000);
            }
            Classes.FrameManager.GoTo(Classes.FrameManager.LockPageFrame);
        }

        private void CheckStatus()
        {
            if (GlobalStatusManager.Instance.GlobalStatus == EGlobalStatus.FirstLaunch)
            {
                // BackHomePanel.Visibility = Visibility.Collapsed;
                LinkDone.Visibility = Visibility.Collapsed;
                WaitForLink();
            }
            else if (GlobalStatusManager.Instance.GlobalStatus == EGlobalStatus.Paired)
            {
                BackHomePanel.Visibility = Visibility.Visible;
                LinkDone.Visibility = Visibility.Visible;
            }
        }

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (GlobalStatusManager.Instance.GlobalStatus == EGlobalStatus.FirstLaunch)
            {
                FrameManager.GoTo(FrameManager.LockPageFrame);
            }
            else
            {
                FrameManager.GoTo(FrameManager.MainPageFrame);
            }
        }

        #endregion
    }
}
