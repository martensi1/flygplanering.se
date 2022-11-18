using System;

namespace FlightPlanner.Service.Config
{
    public class Constants
    {
        // <summary>
        // Interval in seconds between each METAR fetch
        // </summary>
        internal const int MetarFetchIntervalSeconds = 60;

        // <summary>
        // Interval in seconds between each TAF fetch
        // </summary>
        internal const int TafFetchIntervalSeconds = 60;

        // <summary>
        // Interval in seconds between each NOTAM fetch
        // </summary>
        internal const int NotamFetchIntervalSeconds = 300;
    }
}
