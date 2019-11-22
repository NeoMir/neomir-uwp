using DataAccessLibrary.Entitites;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace NeoMir.Classes
{
    public class App
    {
        #region PROPERTIES

        // Frame l'application
        public Frame Frame { get; private set; }
       
        // Données de l'application de l'utilisateur depuis la base de données
        public UserApp UserApp { get; set; }

        #endregion

        #region CONSTRUCTOR

        public App(UserApp app)
        {
            Frame = new Frame();
            UserApp = app;
        }

        #endregion

        #region METHODS

        public void Reset()
        {
            Frame = new Frame();
        }

        #endregion
    }
}




