using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NeoMir.Classes
{
    public class Animation
    {
        // PROPERTIES
        Timer timer;
                
        // CONSTRUCTOR
        public Animation(Button button, int delay)
        {
            this.timer = new Timer(new TimerCallback((obj) => this.AnimateButton(button)), null, 0, delay);
        }

        // METHODS
        private async void AnimateButton(Button button)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await button.Fade(1).Blur(1).StartAsync();
                await button.Fade(0.5f).Blur(0).StartAsync();
            });
        }
    }
}
