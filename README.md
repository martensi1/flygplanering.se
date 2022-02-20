# flygplanering.se
ASP.NET Core web application for easy preflight preperations. Intended to be used by Swedish private pilots for self-briefing before flight.

Visit the live website: http://flygplanering.se

## Data sources

The information presented on the page is taken from two different data sources:

* **METAR/TAF/SWC**  
  [LFV AROWeb](https://aro.lfv.se/)  
  *Swedish LFV (Luftfartsverket) is a state owned enterprise that provides air traffic management and air navigation services. The data is taken directly from their self-briefing system by web scraping their AIS MET pages.*  
* **NOTAM**  
  [NOTAM Search (FAA)](https://notams.aim.faa.gov/notamSearch)  
  *The Federal Aviation Administration (FAA) provides a web interface for accessing digital airport NOTAMs. This system also includes an simple REST API that can be used to retrieve NOTAMs with a simple HTTP POST request; no authentication or API key is needed.*

## Limitations

This web app is currently only intended for pilots operating in and out of Jönköping Airport (ESGJ). Future plans for this project is to add a feature where each client can configure which airports should be included in their report.

## Author

martensi (Simon Mårtensson)
