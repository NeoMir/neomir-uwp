using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NeoMir
{
    /// <summary>
    /// Open App Page (Web View)
    /// </summary>
    public sealed partial class OpenPage : Page
    {
        //
        // PROPERTIES
        //

        public string Link { get; private set; }

        //
        // CONSTRUCTOR
        //

        public OpenPage()
        {
            this.InitializeComponent();
        }

        //
        // METHODS
        //

        // Nothing for the moment //

        //
        // EVENTS
        //

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Link = (string)e.Parameter;

            Uri uri = new Uri(this.Link);
            AppView.Navigate(uri);
        }
    }
}
