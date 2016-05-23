using System;
using Xamarin.Forms;

namespace AutoQuest
{
    public class CircleProgressBar : Image
    {
        public CircleProgressBar()
        {
            this.Source = ImageSource.FromResource("AutoQuest.Resources.Icons.Raccoon.png");
            this.HorizontalOptions = LayoutOptions.Center;
            this.VerticalOptions = LayoutOptions.Center;
            Device.StartTimer(TimeSpan.FromMilliseconds(10), Callback);
            
        }

        private bool Callback()
        {
            Rotation = (Rotation + 1) % 360;
            return IsVisible;
        }
    }
}