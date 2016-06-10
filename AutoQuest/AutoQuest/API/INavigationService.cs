using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoQuest.API
{
    public interface INavigationService
    {
        bool IsAvailable { get; }
        void NavigateTo(GeoPoint geoPoint);
        void ShowOnMap(GeoPoint geoPoint);
    }
}
