using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace NeoMir.Classes
{
    public class App
    {
        // PROPERTIES

        // Frame of the app
        public Frame Frame { get; private set; }
        // Link of the app
        public string Link { get; private set; }
        // Preview of the app
        public BitmapSource Preview { get; set; }

        // CONSTRUCTOR
        public App(string _link)
        {
            Frame = new Frame();
            Link = _link;
        }

        // METHODS

    }
}




