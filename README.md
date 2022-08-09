# flygplanering.se
ASP.NET Core web application for easy preflight preperations. Intended to be used by JFK pilots in for self-briefing before flight

Visit the live website: http://flygplanering.se

## Data sources

The information presented on the page is taken from three different data sources:

* **SWC**  
  [LFV AROWeb](https://aro.lfv.se/)  
  *Swedish LFV (Luftfartsverket) is a state owned enterprise that provides air traffic management and air navigation services. The data is taken directly from their self-briefing system by web scraping their AIS MET pages.*  
* **METAR/TAF**  
  [MET](https://api.met.no/weatherapi/tafmetar/1.0/documentation)  
  *Norway's national meteorological institute provides a simple REST API for retrieving METAR and TAF for swedish airports.*  
* **NOTAM**  
  [NOTAM Search (FAA)](https://notams.aim.faa.gov/notamSearch)  
  *The Federal Aviation Administration (FAA) provides a web interface for accessing digital airport NOTAMs. This system also includes an simple REST API that can be used to retrieve NOTAMs with a simple HTTP POST request; no authentication or API key is needed.*

## Limitations

Future plans for this project is to add a feature where each client can configure which airports should be included in their report.

## License

This repository is licensed with the [MIT](LICENSE) license

## Author

martensi (Simon Mårtensson)
