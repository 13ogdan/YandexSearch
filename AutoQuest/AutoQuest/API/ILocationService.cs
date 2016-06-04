// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using System;

namespace AutoQuest.API
{
    public interface ILocationService
    {
        event EventHandler LocationChanged;

        GeoPoint Location { get; }

        void StopListen();
        void StartListen();
    }
}