using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public ConnectToApi()
        {
            this.InitializeComponent();
        }
        private void HomeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void AskId_Tapped(object sender, TappedRoutedEventArgs e)
        {
            getID();
        }

        private async void getID()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var http = new HttpClient();
                var url = String.Format("http://www.martinbaud.com/V1/gid.php?id");
                var response = await http.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                ID.Text = result.ToString();
                msgconnexion.Text = "En attente de connexion";

            });
        }
    }
}
