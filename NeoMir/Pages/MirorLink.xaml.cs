﻿using DataAccessLibrary;
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

        private async void getID()
        {

            var http = new HttpClient();
            var url = String.Format("http://www.martinbaud.com/V1/gid.php?id");
            var response = await http.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

        }

        // Montre l'identifiant à l'utilisateur
        private void ShowId()
        {
            id = DataAccess.GetMiror()?.Id;
            MirorID.Text = id;
            //WaitForLink();
        }

        // Récupère l'ID depuis l'API
        private async Task GetIdFromDatabase()
        {
            id = await APIManager.GetMirorId();
        }

        // Attente de la validation du lient entre le miroir et l'application par l'API
        private async void WaitForLink()
        {
            while (true)
            {
                Tuple<bool, string> status = await APIManager.GetIsLinked(DataAccess.GetMiror().Id);
                if (status.Item1 && !string.IsNullOrEmpty(status.Item2))
                {
                    //BackHomePanel.Visibility = Visibility.Visible;
                    LinkDone.Visibility = Visibility.Visible;
                    Miror miror = DataAccess.GetMiror();
                    miror.Usermail = status.Item2;
                    miror.IsPaired = true;
                    DataAccess.UpdateEntity(miror);
                    GlobalStatusManager.Instance.GlobalStatus = EGlobalStatus.Paired;
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
