using System.Collections.Generic;


namespace FlightPlanner.Core.Types
{
    public class AirportReportMap : Dictionary<IcaoCode, string> 
    {
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
