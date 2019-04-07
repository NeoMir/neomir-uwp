using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NeoMir.Classes
{
    public class App
    {
        // PROPERTIES

        // Frame of the app
        public Frame Frame { get; private set; }
        // Link of the app
        public string Link { get; private set; }

        // CONSTRUCTOR
        public App(string _link)
        {
            Frame = new Frame();
            Link = _link;
        }

        // METHODS

    }
}




