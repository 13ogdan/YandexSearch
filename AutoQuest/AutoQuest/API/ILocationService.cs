using System;

namespace AutoQuest.API
{
    public interface ILocationService
    {
        GeoPoint Location { get; }

        event EventHandler LocationChanged;

        void StopListen();
        void StartListen();
    }
}