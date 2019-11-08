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
using Windows.Media.SpeechRecognition;
using System.Text;

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

        private async void AskId_Tapped(object sender, TappedRoutedEventArgs e)
        {
            OnListenAsync(sender, e);
        }
        async void OnListenAsync(object sender, RoutedEventArgs e)
        {
            this.recognizer = new SpeechRecognizer();
            await this.recognizer.CompileConstraintsAsync();

            this.recognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
            this.recognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(20);

            this.recognizer.UIOptions.AudiblePrompt = "Say whatever you like, I'm listening";
            this.recognizer.UIOptions.ShowConfirmation = true;
            this.recognizer.UIOptions.IsReadBackEnabled = true;
            this.recognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(5);

            var result = await this.recognizer.RecognizeWithUIAsync();

            if (result != null)
            {
                this.txtResults.Text = result.Text.ToString();
                /*StringBuilder builder = new StringBuilder();

                builder.AppendLine(
                  $"I have {result.Confidence} confidence that you said [{result.Text}] " +
                  $"and it took {result.PhraseDuration.TotalSeconds} seconds to say it " +
                  $"starting at {result.PhraseStartTime:g}");

                var alternates = result.GetAlternates(10);

                builder.AppendLine(
                  $"There were {alternates?.Count} alternates - listed below (if any)");

                if (alternates != null)
                {
                    foreach (var alternate in alternates)
                    {
                        builder.AppendLine(
                          $"Alternate {alternate.Confidence} confident you said [{alternate.Text}]");
                    }
                }
                this.txtResults.Text = builder.ToString();*/
            }
        }
        SpeechRecognizer recognizer;

    }
}

