using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Util;
using AutoQuest.API;
using AutoQuest.Droid.Service;

[assembly: Xamarin.Forms.Dependency(typeof(LocationService))]
namespace AutoQuest.Droid.Service
{
    public class LocationListener : Java.Lang.Object, ILocationListener
    {
        public event EventHandler LocationChanged;
        private GeoPoint _location;
        private const double TOLERANCE = 0.000001;

        public GeoPoint Location { get { return _location; } }

        public void OnLocationChanged(Location location)
        {
            Log.Debug("Android", "CenterStreetPoint listener OnLocationChanged {0}", location);

            if (location == null)
                return;

            var geoPoint = new GeoPoint(location.Latitude, location.Longitude);
            if (Math.Abs(_location.Lat - geoPoint.Lat) < TOLERANCE && Math.Abs(_location.Long - geoPoint.Long) < TOLERANCE)
                return;
            _location = geoPoint;
            LocationChanged?.Invoke(this, new EventArgs());
        }

        public void OnProviderDisabled(string provider)
        {
            Log.Debug("Android", "CenterStreetPoint listener OnProviderDisabled {0}", provider);
        }

        public void OnProviderEnabled(string provider)
        {
            Log.Debug("Android", "CenterStreetPoint listener OnProviderEnabled {0}", provider);
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Log.Debug("Android", "CenterStreetPoint listener OnStatusChanged {0} ({1})", provider, status);
        }
    }
    class LocationService : ILocationService
    {
        private readonly LocationManager _locationManager;
        private string _locationProvider;
        private readonly Criteria _criteria;
        private readonly LocationListener _locationListener;

        public GeoPoint Location => _locationListener.Location;

        public event EventHandler LocationChanged;

        public LocationService()
        {
            _locationManager = Application.Context.GetSystemService(Context.LocationService) as LocationManager;
            if (_locationManager == null)
                return;

            _criteria = new Criteria()
            {
                Accuracy = Accuracy.NoRequirement,
                AltitudeRequired = false,
                BearingRequired = false,
                SpeedRequired = false,
                PowerRequirement = Power.NoRequirement
            };

            _locationListener = new LocationListener();
            _locationListener.LocationChanged += OnLocationChanged;

            var acceptableLocationProviders = _locationManager.GetProviders(_criteria, true);
            _locationProvider = acceptableLocationProviders.Any() ? acceptableLocationProviders.First() : string.Empty;
            Log.Debug("Android LocationService", "Provider" + _locationProvider + ".");

            if (!ProviderEnabled())
            {
                // Try to turn on provider
                var builder = new AlertDialog.Builder(MainActivity.Instance);
                builder.SetTitle("GPS navigation");
                builder.SetMessage("Please turn on GPS navigation");
                builder.SetNegativeButton("No thanks", (sender, args) =>
                {

                });
                builder.SetPositiveButton("Turn on", (sender, args) =>
                {
                    var intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    Application.Context.StartActivity(intent);
                });
                var ad = builder.Create();
                ad.Show();
            }
        }

        private void OnLocationChanged(object sender, EventArgs e)
        {
            LocationChanged?.Invoke(this, e);
        }

        private bool ProviderEnabled()
        {
            return !string.IsNullOrEmpty(_locationProvider) &&
                   _locationProvider != "passive" &&
                   _locationManager.IsProviderEnabled(_locationProvider);
        }

        public void StopListen()
        {
            _locationManager.RemoveUpdates(_locationListener);
        }

        public void StartListen()
        {
            ListenToBestProvider();
        }

        private void ListenToBestProvider()
        {
            _locationProvider = _locationManager.GetBestProvider(_criteria, false);
            if (ProviderEnabled())
                _locationManager.RequestLocationUpdates(_locationProvider, 1000 , 100, _locationListener);
        }
    }
}