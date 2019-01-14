using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Globalization;
using Windows.UI.Xaml.Media.Animation;

namespace NeoMir
{
    /// <summary>
    /// Main Page
    /// </summary>

    public partial class MainPage : Page
    {
        //
        // PROPERTIES
        //

        Timer timerDateTime;
        Timer timerWeather;
        List<String> pages = new List<string>()
        {
            "MainPage",
        };
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

            bool flag = false;
            foreach (string element in pages)
            {
                if (string.Equals("MainPage", element) == true)
                {
                    flag = true;
                }
            }
            if (flag == false)
            {
                pages.Add("MainPage");
            }


            timerDateTime = new Timer(new TimerCallback((obj) => this.refreshDateTime()), null, 0, 1000);
            timerWeather = new Timer(new TimerCallback((obj) => this.refreshWeather()), null, 0, 900000);
        }

        //
        // METHODS
        //
        
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

        private async void refreshWeather()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                // TODO: Get current location for IoT devices

                // Use or Open Wheather API to get Wheather Information of a location
                var http = new HttpClient();
                var url = String.Format("http://api.openweathermap.org/data/2.5/weather?APPID={0}&units=metric&lang=fr&lat=16.265&lon=-61.551", Config.openWheatherAPIKey);
                var response = await http.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                var serializer = new DataContractJsonSerializer(typeof(RootObject));
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));

                // Now we have all the data of the weather
                var data = (RootObject)serializer.ReadObject(ms);

                // Get and display the weather icon and temperature
                var uri = "ms-appx:///Assets/weather-icons/" + weatherCodesIcons[data.weather.First().icon.ToString()];
                weather_icon.Source = new BitmapImage(new Uri(uri));
                weather_temperature.Text = data.main.temp + " °C";
                weather_description.Text = char.ToUpper(data.weather.First().description[0]) + data.weather.First().description.Substring(1);
            });
        }

        private void goToRight(String page_name)
        {
            int count = 0;
            foreach (String element in pages)
            {
                count++;
                if (string.Equals(element, page_name) == true)
                {
                    if (int.Equals(count, pages.Count) == true)
                    {
                        goToPage(0);
                    }
                    else
                    {
                        goToPage(count);
                    }
                }
            }
        }

        private void goToLeft(String page_name)
        {
            int count = 0;
            foreach (String element in pages)
            {

                if (string.Equals(element, page_name) == true)
                {

                    if (count == 0)
                    {
                        goToPage(pages.Count - 1);
                    }
                    else
                    {
                        goToPage(count - 1);
                    }
                }

                count++;
            }
        }

        private void goToPage(int p_nbr)
        {
            var parameters = new PageParams();
            parameters.pages = pages;
            if (string.Equals("MainPage", pages[p_nbr]) == true)
            {
                this.Frame.Navigate(typeof(MainPage), parameters);
            }
            if (string.Equals("ClassicPage", pages[p_nbr]) == true)
            {
                this.Frame.Navigate(typeof(ClassicPage), parameters);
            }
            if (string.Equals("SecondPage", pages[p_nbr]) == true)
            {
                this.Frame.Navigate(typeof(SecondPage), parameters);
            }
        }

        //
        // EVENTS
        //

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try {
                var parameters = (PageParams)e.Parameter;
                pages = parameters.pages;
            }
            catch
            {
                    
            }

            ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("goToMain");
            if (imageAnimation != null)
            {
                imageAnimation.TryStart(LaunchAppButton);
            }
        }

        private void LaunchAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("goToApps", LaunchAppButton);
            this.Frame.Navigate(typeof(AppsPage));
        }

        private void RightButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("goToApps", LaunchAppButton);
            goToRight("MainPage");
        }

        private void LeftButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("goToApps", LaunchAppButton);
            goToLeft("MainPage");
        }

        private void FirstPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("goToApps", LaunchAppButton);
            bool flag = false;
            foreach(string element in pages)
            {
                if (string.Equals("ClassicPage", element) == true)
                {
                    flag = true;
                }
            }
            if (flag == false)
            {
                pages.Add("ClassicPage");
            }
        }

        private void SecondPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("goToApps", LaunchAppButton);
            bool flag = false;
            foreach (string element in pages)
            {
                if (string.Equals("SecondPage", element) == true)
                {
                    flag = true;
                }
            }
            if (flag == false)
            {
                pages.Add("SecondPage");
            }
        }

    }
}
