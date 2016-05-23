using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace AutoQuest.Droid
{
    [Activity(Label = "AutoQuest", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        private static MainActivity _instance;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _instance = this;
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
            _instance = this;
        }

        public static MainActivity Instance
        {
            get{ return _instance;}
        }

        private void ShowTestDialog()
        {
            // Try to turn on provider
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("GPS navigation");
            builder.SetMessage("Please turn on GPS navigation");
            builder.SetPositiveButton("Turn on", (sender, args) =>
            {
                var intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                Application.Context.StartActivity(intent);
            });
            var ad = builder.Create();
            ad.Show();
        }
    }
}

