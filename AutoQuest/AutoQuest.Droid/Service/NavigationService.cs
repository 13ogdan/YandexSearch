using System.Linq;
using Android.Content;
using AutoQuest.API;
using AutoQuest.Droid.Service;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(NavigationService))]
namespace AutoQuest.Droid.Service
{
    public class NavigationService : INavigationService
    {
        public class YandexIntent
        {
            public const string NavigateAction = "BUILD_ROUTE_ON_MAP";
            public const string ShowOnMap = "SHOW_POINT_ON_MAP";
            private readonly string _prefix;

            public YandexIntent(string prefix)
            {
                _prefix = prefix;
            }

            public Intent GetIntent(string action)
            {
                var intent = new Intent($"{_prefix}.action.{action}");
                intent.SetPackage(_prefix);
                // check if Yandex package available 
                var pm = Forms.Context.PackageManager;
                var infos = pm.QueryIntentActivities(intent, 0);
                if (infos == null || !infos.Any())
                    return null;

                return intent;
            }
        }

        private readonly bool _isAvailable;
        private readonly YandexIntent _mapIntent;
        private readonly YandexIntent _naviIntent;

        /// <summary>Initializes a new instance of the <see cref="NavigationService"/> class.</summary>
        public NavigationService()
        {
            _mapIntent = new YandexIntent("ru.yandex.yandexmaps");
            _naviIntent = new YandexIntent("ru.yandex.yandexnavi");
            _isAvailable = _naviIntent.GetIntent(YandexIntent.NavigateAction) != null || _mapIntent.GetIntent(YandexIntent.NavigateAction) != null;
        }

        public bool IsAvailable { get { return _isAvailable; } }

        public void NavigateTo(GeoPoint geoPoint)
        {
            var intent = _naviIntent.GetIntent(YandexIntent.NavigateAction) ?? _mapIntent.GetIntent(YandexIntent.NavigateAction);

            intent.PutExtra("lat_to", geoPoint.Lat);
            intent.PutExtra("lon_to", geoPoint.Long);

            MainActivity.Instance.StartActivity(intent);
        }

        public void ShowOnMap(GeoPoint geoPoint)
        {
            var intent = _mapIntent.GetIntent(YandexIntent.ShowOnMap) ?? _naviIntent.GetIntent(YandexIntent.ShowOnMap);

            intent.PutExtra("lat", geoPoint.Lat);
            intent.PutExtra("lon", geoPoint.Long);

            MainActivity.Instance.StartActivity(intent);
        }
    }
}