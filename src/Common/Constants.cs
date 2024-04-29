using System;

namespace FlightPlanner.Service.Common
{
    public class Constants
    {
        // <summary>
        // Website name
        // </summary>
        internal const string WebsiteName = "Flygplanering.se";

        // <summary>
        // Configured website URL
        // </summary>
        internal const string WebsiteUrl = "https://flygplanering.se";

        // <summary>
        // Website Author
        // </summary>
        internal const string WebsiteAuthor = "Simon Alm-Mårtensson";

        // <summary>
        // Configured website URL
        // </summary>
        internal const string WebsiteSitemap = "https://flygplanering.se/sitemap.xml";



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




        // <summary>
        // URL for learning more about cookies
        // </summary>
        internal const string AboutCookiesUrl = "https://internetstiftelsen.se/om-webbplatsen/om-kakor/";

        // <summary>
        // URL for the project
        // </summary>
        internal const string ProjectUrl = "https://gitlab.com/martensi1/flygplanering-se";

        // <summary>
        // URL for the NSWC image
        // </summary>
        internal const string NswcUrl = "https://aro.lfv.se/tor/nswc2aro.gif";

        // <summary>
        // URL for the NorthAviMet low level forecast
        // </summary>
        internal const string NorthAviMetUrl = "https://www.northavimet.com/low-level-forecast/llf-sweden-denmark-finland";

        // <summary>
        // URL for the CC BY 4.0 license
        // </summary>
        internal const string CC4LicenseUrl = "https://creativecommons.org/licenses/by/4.0/";

        // <summary>
        // URL for the NSWC info page
        // </summary>
        internal const string MetHomePageUrl = "https://www.met.no/";

        // <summary>
        // URL for the NSWC info page
        // </summary>
        internal const string NotamSearchHomePageUrl = "https://notams.aim.faa.gov/notamSearch/nsapp.html#/";

        // <summary>
        // URL for the NSWC info page
        // </summary>
        internal const string AboutNswcUrl = "https://www.smhi.se/professionella-tjanster/hallbara-och-sakra-transporter/flyg/nswc-1.2428";
    }
}
