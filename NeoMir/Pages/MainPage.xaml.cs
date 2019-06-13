using NeoMir.Classes.Com;
using NeoMir.Classes.Communication;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace NeoMir.Pages
{
    public sealed partial class MainPage : Page
    {
        //
        // PROPERTIES
        //

        Timer timerDateTime;
        Timer timerWeather;
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

        //
        // CONSTRUCTOR
        //

        public MainPage()
        {
            this.InitializeComponent();
            timerDateTime = new Timer(new TimerCallback((obj) => this.refreshDateTime()), null, 0, 1000);
            timerWeather = new Timer(new TimerCallback((obj) => this.refreshWeather()), null, 0, 900000);
            getProfile();
            GestureSetup();
        }

        //
        // METHODS
        //

        private async Task getProfile()
        {
        
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var id = "";
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///id/id.txt"));
                    using (var inputStream = await file.OpenReadAsync())
                    using (var classicStream = inputStream.AsStreamForRead())
                    using (var streamReader = new StreamReader(classicStream))
                    {
                        id = streamReader.ReadToEnd();
                    }

                var http = new HttpClient();
                var url = String.Format("http://www.martinbaud.com/V1/getUserFromId.php?id=" + id);
                var response = await http.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                msgWelcome.Text += "Welcome " + result;
            });
        }

        /// <summary>
        /// Actualize the datetime
        /// </summary>
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

        /// <summary>
        /// Actualize the wheather
        /// </summary>
        private async void refreshWeather()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                // TODO: Get current location for IoT devices

                // Use or Open Wheather API to get Wheather Information of a location
                var http = new HttpClient();
                var url = String.Format("http://api.openweathermap.org/data/2.5/weather?APPID={0}&units=metric&lang=fr&lat=16.265&lon=-61.551", Classes.Config.openWheatherAPIKey);
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

        private void GestureSetup()
        {
            gestureCollector = GestureCollector.Instance;
            gestureCollector.RegisterToGestures(this, ApplyGesture);
        }

        private async void ApplyGesture(Gesture gesture)
        {
            if (!gesture.IsConsumed)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (this == Classes.AppManager.GetCurrentPage())
                    {
                        if (gesture.Name == "Next Right" && !gesture.IsConsumed)
                        {
                            NextAppButton_Tapped(null, null);
                            gesture.IsConsumed = true;
                        }
                        else if (gesture.Name == "Validate" && !gesture.IsConsumed) 
                        {
                            LaunchAppButton_Tapped(null, null);
                            gesture.IsConsumed = true;
                        }
                        else if (gesture.Name == "Back" && !gesture.IsConsumed)
                        {
                            PrevAppButton_Tapped(null, null);
                            gesture.IsConsumed = true;
                        }
                        else if (gesture.Name == "Lock" && !gesture.IsConsumed)
                        {
                            LockButton_Tapped(null, null);
                            gesture.IsConsumed = true;
                        }
                    }
                });
            }
        }

        //
        // EVENTS
        //

        private void LaunchAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.AppManager.GoToApps();
        }

        private void NextAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

            Classes.AppManager.NextApp();
        }

        private void PrevAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.AppManager.PrevApp();
        }

        private void LockButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Classes.AppManager.GoToLock();
        }
        
        private void ApiPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ConnectToApi));
        }
    }
}
