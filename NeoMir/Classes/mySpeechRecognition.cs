using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;

namespace NeoMir.Classes
{
    public class mySpeechRecognition
    {
        private CoreDispatcher _dispatcher;
        SpeechRecognizer recognizer;
        public mySpeechRecognition()
        {
        }

        async public void StartContinuousRecognition()
        {
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            await MyContinuousRecognition();
            await recognizer.ContinuousRecognitionSession.StartAsync();

        }


        async public Task MyContinuousRecognition()
        {
            this.recognizer = new SpeechRecognizer();
            var expectedResonses = new String[] { "application", "accueil", "verouille" };
            var listConstraint = new SpeechRecognitionListConstraint(expectedResonses, "Contrainte");
            this.recognizer.Constraints.Add(listConstraint);
            await this.recognizer.CompileConstraintsAsync();

            this.recognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
            this.recognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
        }

        private async void ContinuousRecognitionSession_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
        {
            if (this.recognizer.State == SpeechRecognizerState.Idle)
            {
                await this.recognizer.ContinuousRecognitionSession.StartAsync();
            }
        }

        private async void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            if (args.Result.Confidence == SpeechRecognitionConfidence.Medium || args.Result.Confidence == SpeechRecognitionConfidence.High)
            {
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await DoAction(args.Result.Text);
                });
            }
        }

        private async Task<Task> DoAction(string phrase)
        {
            var cleanedPhrase = phrase.ToLower().Replace(".", string.Empty);
            var list = new String[] { "application", "accueil", "verouille" };


            if (list.Contains(cleanedPhrase))
            {
                if (cleanedPhrase.Contains("application"))
                {
                    FrameManager.GoTo(FrameManager.AppsPageFrame);
                }
                if (cleanedPhrase.Contains("accueil"))
                {
                    FrameManager.GoTo(FrameManager.MainPageFrame);

                }
                if (cleanedPhrase.Contains("verouille"))
                {
                    FrameManager.GoTo(FrameManager.LockPageFrame);
                }
            }

            return Task.CompletedTask;
        }

        async public Task<String> GetStringFromSpeechRecognition()
        {
            String str = "";
            var recognizer = new SpeechRecognizer();
            await recognizer.CompileConstraintsAsync();
            var result = await recognizer.RecognizeAsync();
            if (result != null)
            {
                str = result.Text.ToString();
            }
            return str;
        }
    }
}
