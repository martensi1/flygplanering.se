using System.Collections.Generic;


namespace FPSE.Core.Types
{
    public class AirportReports : Dictionary<IcaoCode, string> 
    {
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
