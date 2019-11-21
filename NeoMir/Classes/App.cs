using DataAccessLibrary.Entitites;
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
       
        // Database app representation
        public UserApp UserApp { get; set; }

        // CONSTRUCTOR
        public App(UserApp app)
        {
            Frame = new Frame();
            UserApp = app;
        }

        // METHODS
        public void Reset()
        {
            Frame = new Frame();
        }
    }
}




