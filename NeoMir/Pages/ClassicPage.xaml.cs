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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace NeoMir
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
public sealed partial class ClassicPage : Page
    {

        List<String> pages = new List<string>();
        public ClassicPage()
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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
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

        private void LaunchAppButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            LaunchAppButton.Height += 10;
            LaunchAppButtonName.FontSize += 3;
        }

        private void LaunchAppButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            LaunchAppButton.Height -= 10;
            LaunchAppButtonName.FontSize -= 3;
        }

        private void LaunchAppButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("goToApps", LaunchAppButton);
            this.Frame.Navigate(typeof(AppsPage));
        }



        private void RightButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            RightButton.Height += 10;
        }

        private void RightButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            RightButton.Height -= 10;
        }

        private void RightButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("goToApps", LaunchAppButton);
            goToRight("ClassicPage");
        }





        private void LeftButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            LeftButton.Height += 10;
        }

        private void LeftButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            LeftButton.Height -= 10;
        }

        private void LeftButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("goToApps", LaunchAppButton);
            goToLeft("ClassicPage");
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
            ///if (string.Equals("Page1", pages[p_nbr]) == true)
            ///{
            ///    this.Frame.Navigate(typeof(Page1));
            ///}
            if (string.Equals("ClassicPage", pages[p_nbr]) == true)
            {
                this.Frame.Navigate(typeof(ClassicPage), parameters);
            }
            if (string.Equals("SecondPage", pages[p_nbr]) == true)
            {
                this.Frame.Navigate(typeof(SecondPage), parameters);
            }

        }
    }
}
