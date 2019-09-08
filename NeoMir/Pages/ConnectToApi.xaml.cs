using DataAccessLibrary;
using DataAccessLibrary.API;
using NeoMir.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace NeoMir.Pages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ConnectToApi : Page
    {
        private string id;

        public ConnectToApi()
        {
            this.InitializeComponent();
            CheckStatus();
            ShowId();
        }

        private async void getID()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var http = new HttpClient();
                var url = String.Format("http://www.martinbaud.com/V1/gid.php?id");
                var response = await http.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();

            });
        }

        /// <summary>
        /// How the Mirror ID to the user
        /// </summary>
        private void ShowId()
        {
            id = DataAccess.GetMiror()?.Id;
            MirorID.Text = id;
            WaitForLink();
        }

        /// <summary>
        /// Get the Id from the db through API request
        /// </summary>
        /// <returns></returns>
        private async Task GetIdFromDatabase()
        {
            id = await APIManager.GetMirorId();
        }

        /// <summary>
        /// Wait for the API to validate a link between this miror and a user account
        /// </summary>
        private async void WaitForLink()
        {
            while (FrameManager.GetCurrentPage() == this)
            {
                if (await APIManager.GetIsLinked(id))
                {
                    //BackHomePanel.Visibility = Visibility.Visible;
                    LinkDone.Visibility = Visibility.Visible;
                    break;
                }
                await Task.Delay(1000);
                //id = "5b8f";
                //TODO : Appel API popur savoir si on est pair a un compte
                //await Task.Delay(1000);
            }
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
                Classes.FrameManager.GoTo(Classes.FrameManager.LockPageFrame);
            }
            else
            {
                Classes.FrameManager.GoTo(Classes.FrameManager.MainPageFrame);
            }
        }
    }
}
