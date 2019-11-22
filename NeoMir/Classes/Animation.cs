using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.UI.Xaml.Controls;

namespace NeoMir.Classes
{
    public class Animation
    {
        #region PROPERTIES

        Timer timer;

        #endregion

        #region CONSTRUCTOR

        public Animation(Button button, int delay)
        {
            this.timer = new Timer(new TimerCallback((obj) => this.AnimateButton(button)), null, 0, delay);
        }

        #endregion

        #region METHODS

        private async void AnimateButton(Button button)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await button.Fade(1).Blur(1).StartAsync();
                await button.Fade(0.5f).Blur(0).StartAsync();
            });
        }

        #endregion
    }
}
