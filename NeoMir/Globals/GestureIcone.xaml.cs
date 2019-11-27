using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace NeoMir.Globals
{
    public sealed partial class GestureIcone : UserControl
    {
        private string icon;

        public string Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                SetIcone(value);
            }
        }

        public GestureIcone()
        {
            this.InitializeComponent();
        }

        public async void SetIcone(string gesture)
        {
            ImgBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets//Gestures//" + gesture + ".png"));
            ImgBrush.Opacity = 0;
            for (int i = 0; i < 5; i++)
            {
                ImgBrush.Opacity +=0.2;
                await Task.Delay(50);
            }
            for (int i = 0; i < 5; i++)
            {
                ImgBrush.Opacity -= 0.2;
                await Task.Delay(50);
            }
        }
    }
}
