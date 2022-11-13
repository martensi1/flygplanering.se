# JFK Flight Planner
ASP.NET Core web application for easy preflight preperations. Intended to be used by JFK pilots in for self-briefing before flight

Visit the live website: http://flygplanering.se

## Data sources

The information presented on the page is taken from three different data sources:

* **NSWC**  
  [LFV AROWeb](https://aro.lfv.se/)  
  *Swedish LFV (Luftfartsverket) is a state owned enterprise that provides air traffic management and air navigation services. The data is taken directly from their self-briefing system by web scraping their AIS MET pages.*  
* **METAR/TAF**  
  [MET Norway](https://api.met.no/weatherapi/tafmetar/1.0/documentation)  
  *Norway's national meteorological institute provides a simple REST API for retrieving METAR and TAF for swedish airports.*  
* **NOTAM**  
  [NOTAM Search (FAA)](https://notams.aim.faa.gov/notamSearch)  
  *The Federal Aviation Administration (FAA) provides a web interface for accessing digital airport NOTAMs. This system also includes an simple REST API that can be used to retrieve NOTAMs with a simple HTTP POST request; no authentication or API key is needed.*

## Diagram

```plantuml
skinparam sequenceMessageAlign center
participant "FAA Notam Search API" as NotamSearch
participant "MET.no API" as MetNorway
participant "Flygplanering.se" as Backend #green
participant Client

NotamSearch <- Backend : POST /search
NotamSearch -> Backend : NOTAM data

.......

MetNorway <- Backend : GET /tafmetar/1.0/metar
MetNorway -> Backend : METAR data

MetNorway <- Backend : GET /tafmetar/1.0/taf
MetNorway -> Backend : TAF data

......

rnote over Backend 
  Process and store
  data in memory
endrnote

Backend <- Client : GET /index.html
Backend -> Client : Page data

```

## License

This repository is licensed with the [MIT](LICENSE) license

## Author

martensi (Simon Mårtensson)
