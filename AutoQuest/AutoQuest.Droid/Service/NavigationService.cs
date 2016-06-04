using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AutoQuest.API;
using AutoQuest.Droid.Service;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(NavigationService))]
namespace AutoQuest.Droid.Service
{
    
    public class NavigationService: INavigationService
    {
        private readonly bool _isAvailable;

        /// <summary>Initializes a new instance of the <see cref="NavigationService"/> class.</summary>
        public NavigationService()
        {
            // https://github.com/yandexmobile/yandexmapkit-android/wiki/%D0%98%D0%BD%D1%82%D0%B5%D0%B3%D1%80%D0%B0%D1%86%D0%B8%D1%8F-%D1%81-%D0%AF%D0%BD%D0%B4%D0%B5%D0%BA%D1%81.%D0%9D%D0%B0%D0%B2%D0%B8%D0%B3%D0%B0%D1%82%D0%BE%D1%80%D0%BE%D0%BC
            // Создаем интент для построения маршрута
            var intent = new Intent("ru.yandex.yandexnavi.action.BUILD_ROUTE_ON_MAP");
            intent.SetPackage("ru.yandex.yandexnavi");

            // check if Yandex navigator available 
            var pm = Forms.Context.PackageManager;
            var infos = pm.QueryIntentActivities(intent, 0);
            if (infos == null || !infos.Any())
                return;

            _isAvailable = true;
        }

        public bool IsAvailable { get { return _isAvailable; } }

        public void NavigateTo(GeoPoint geoPoint)
        {
            var intent = new Intent("ru.yandex.yandexnavi.action.BUILD_ROUTE_ON_MAP");
            intent.SetPackage("ru.yandex.yandexnavi");
            intent.PutExtra("lat_to", geoPoint.Lat);
            intent.PutExtra("lon_to", geoPoint.Long);
        }
    }
}