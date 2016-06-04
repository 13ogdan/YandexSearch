// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using System;
using Xamarin.Forms;

namespace AutoQuest.Controls
{
    public class CircleProgressBar : Image
    {
        public CircleProgressBar()
        {
            Source = ImageSource.FromResource("AutoQuest.Resources.Icons.Raccoon.png");
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
            Device.StartTimer(TimeSpan.FromMilliseconds(10), Callback);
        }

        private bool Callback()
        {
            Rotation = (Rotation + 1)%360;
            return IsVisible;
        }
    }
}