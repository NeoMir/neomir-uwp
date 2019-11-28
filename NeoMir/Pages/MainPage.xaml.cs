using NeoMir.Classes.Com;
using NeoMir.Classes.Communication;
using NeoMir.UserManagment;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI.Xaml;
using NeoMir.Classes;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using NeoMir.Globals;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;

namespace NeoMir.Pages
{
    public sealed partial class MainPage : Page
    {
        #region PROPERTIES

        Timer timerDateTime;
        Timer timerWeather;
        Dictionary<EGestures, Action> gestActions;

        GestureCollector gestureCollector;

        Dictionary<string, string> weatherCodesIcons = new Dictionary<string, string>()
        {
            {"11d", "thunderstorm.png"},
            {"11n", "thunderstorm.png"},
            {"09d", "rainy-cloud-outline.png"},
            {"09n", "rainy-cloud-outline.png"},
            {"10d", "rainy-cloud-and-sun.png"},
            {"10n", "rainy-cloud-and-moon.png"},
            {"13d", "snow-cloud.png"},
            {"13n", "snow-cloud.png"},
            {"50d", "foggy-day.png"},
            {"50n", "foggy-day.png"},
            {"01d", "sunny-day.png"},
            {"01n", "crescent-moon.png"},
            {"02d", "cloud-with-sun.png"},
            {"02n", "cloud-and-half-moon.png"},
            {"03d", "a-single-cloud.png"},
            {"03n", "a-single-cloud.png"},
            {"04d", "cloudy-sky.png"},
            {"04n", "cloudy-sky.png"},

        };
        private SpeechRecognizer recognizer;
        private CoreDispatcher _dispatcher;


        #endregion

        #region CONSTRUCTOR

        public MainPage()
        {
            this.InitializeComponent();
            SetTimers();
            StartAnimations();
            GestureSetup();
            UserManager.Instance.ProfileChanged += GetProfile;
            GestureIcone.Content = new GestureIcone() { Icon = "Validate" };
        }

        #endregion

        #region METHODS

        // Débute les animations
        private void StartAnimations()
        {
            new Animation(LaunchAppButton, 5000);
            new Animation(NextAppButton, 5000);
            new Animation(PrevAppButton, 5000);
            new Animation(LockButton, 5000);
        }

        // Débute les timers
        private void SetTimers()
        {
            timerDateTime = new Timer(new TimerCallback((obj) => this.refreshDateTime()), null, 0, 1000);
            timerWeather = new Timer(new TimerCallback((obj) => this.refreshWeather()), null, 0, 900000);
        }

        // Recupère le profil de l'utilisateur actuellement connecté
        private async void GetProfile()
        {
            if (UserManager.Instance.CurrentProfile.IsFaceLinked)
            {
                msgWelcome.Text = "Bienvenue " + UserManager.Instance.CurrentProfile.Name;

                // Défini un offset pour que le message descende de manière proportionnelle à la taille de l'ecran.
                float offset = (float)((Frame)Window.Current.Content).ActualHeight * 0.35f;
                // Animation du message
                msgWelcome.Offset(offsetX: 0, offsetY: offset, duration: 2500, delay: 500, easingType: EasingType.Default).Start();
            }
            else
            {
                msgWelcome.Text = Globals.GlobalNames.PhotoRequired;
                await Task.Delay(3000);
                FrameManager.GoTo(FrameManager.CapturePage);
            }

        }

        // Actualise la date et l'heure
        private async void refreshDateTime()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // Your UI update code goes here!
                DateTime dateTime = DateTime.Now;
                string hour = dateTime.Hour.ToString();
                string minute = dateTime.Minute.ToString();
                string day = dateTime.Day.ToString();
                string dayName = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(dateTime.DayOfWeek);
                string month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
                string year = dateTime.Year.ToString();

                if (hour.Length == 1)
                    hour = "0" + hour;
                if (minute.Length == 1)
                    minute = "0" + minute;
                if (day.Length == 1)
                    day = "0" + day;

                time_field.Text = hour + ":" + minute;
                date_field.Text = char.ToUpper(dayName[0]) + dayName.Substring(1) + " " + day + " " + month + " " + year;
            });
        }

        // Actualise la météo
        private async void refreshWeather()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                // TODO: Get current location for IoT devices

                // Use or Open Wheather API to get Wheather Information of a location
                var http = new HttpClient();
                var url = String.Format("http://api.openweathermap.org/data/2.5/weather?APPID={0}&units=metric&lang=fr&lat=48.856613&lon=2.352222", Classes.Config.openWheatherAPIKey);
                var response = await http.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                var serializer = new DataContractJsonSerializer(typeof(Classes.RootObject));
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));

                // Now we have all the data of the weather
                var data = (Classes.RootObject)serializer.ReadObject(ms);

                // Get and display the weather icon and temperature
                var uri = "ms-appx:///Assets/weather-icons/" + weatherCodesIcons[data.weather.First().icon.ToString()];
                weather_icon.Source = new BitmapImage(new Uri(uri));
                weather_temperature.Text = data.main.temp + " °C";
                weather_description.Text = char.ToUpper(data.weather.First().description[0]) + data.weather.First().description.Substring(1);
            });
        }

        // Setup des gestes
        private void GestureSetup()
        {
            gestureCollector = GestureCollector.Instance;
            InitGestureBehavior();
            gestureCollector.RegisterToGestures(this, ApplyGesture);

        }

        // Initialise un dictionnaire d'action qui serontt invoqué selon le geste détécté 
        private void InitGestureBehavior()
        {
            gestActions = new Dictionary<EGestures, Action>();
            gestActions.Add(EGestures.NextLeft, () => PrevAppButton_Tapped(null, null));
            gestActions.Add(EGestures.NextRight, () => NextAppButton_Tapped(null, null));
            gestActions.Add(EGestures.Lock, () => LockButton_Tapped(null, null));
            gestActions.Add(EGestures.Validate, () => LaunchAppButton_Tapped(null, null));
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

        #endregion

        #region EVENTS

        private void Capture_Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.GoTo(FrameManager.CapturePage, false);
        }

        private void LaunchAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.FrameManager.GoTo(Classes.FrameManager.AppsPageFrame);
        }

        private void NextAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.FrameManager.NextApp();
        }

        private void PrevAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.FrameManager.PrevApp();
        }

        private void LockButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.FrameManager.GoTo(Classes.FrameManager.LockPageFrame);
        }

        private void ApiPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.FrameManager.GoTo(Classes.FrameManager.PairPageFrame);
        }

        // Permet de compléter une note grâce à la reconnaissance vocale
        private async void btnNoteTapped(object sender, TappedRoutedEventArgs e)
        {
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            /*
            var SR = new mySpeechRecognition();
            txtNote.Text = await SR.GetStringFromSpeechRecognition();*/
            await MyContinuousRecognition();
            await recognizer.ContinuousRecognitionSession.StartAsync();

        }


        async public Task MyContinuousRecognition()
        {
            this.recognizer = new SpeechRecognizer();
            var expectedResonses = new String[] { "bonjour", "test" };
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

        private Task DoAction(string phrase)
        {
            var cleanedPhrase = phrase.ToLower().Replace(".", string.Empty);
            var list = new String[] { "bonjour", "test" };


            if (list.Contains(cleanedPhrase))
            {
                txtNote.Text = cleanedPhrase;
            }

            return Task.CompletedTask;
        }



        #endregion
    }
}
