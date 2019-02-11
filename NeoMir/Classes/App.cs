using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NeoMir.Classes
{
    public class App
    {
        // PROPERTIES
        public Frame Frame { get; set; }
        private string Link { get; set; }

        // CONSTRUCTOR
        public App(string _link)
        {
            this.Frame = new Frame();
            this.Link = _link;
        }

        // METHODS
        public void Open()
        {
            this.Frame.Navigate(typeof(Pages.AppPage), this.Link);
            Window.Current.Content = this.Frame;
            Window.Current.Activate();
        }
    }
}




