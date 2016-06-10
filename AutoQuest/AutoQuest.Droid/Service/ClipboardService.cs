// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using Android.Content;
using AutoQuest.API;
using AutoQuest.Droid.Service;

[assembly: Xamarin.Forms.Dependency(typeof(ClipboardService))]

namespace AutoQuest.Droid.Service
{
    internal class ClipboardService : IClipboardService
    {
        public void SaveToClipboard(string text)
        {
            var systemService = MainActivity.Instance.GetSystemService(Context.ClipboardService);
            var service = systemService as ClipboardManager;
            if (service != null)
                service.Text = text;
        }
    }
}