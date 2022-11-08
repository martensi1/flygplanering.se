/************************************************
 * Flight Planner
 ***********************************************/
var cookie_names = {
    "METAR": "metar-airports",
    "TAF": "taf-airports",
    "NOTAM": "notam-airports"
}


function add_airport(target) {
    icao = prompt("Enter airport icao: ");

    if (!is_valid_icao(icao)) {
        alert("Invalid airport format!");
    }

    if (is_airport_in_cookie(icao)) {
        alert("Airport already added!");
    }


    var cookie_name = cookie_names[target];

    cookie_value = get_cookie(cookie_name);
    new_value = cookie_value + "%2C" + icao;

    set_cookie(cookie_name, new_value);
    location.reload();
}

function get_cookie(cookie_name) {
    let name = cookie_name + "=";
    let cookies = document.cookie.split(';');

    for (let i = 0; i < cookies.length; i++) {
        let c = cookies[i];

        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }

        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }

    return "";
}

function set_cookie(cookie_name, cookie_value, expiration_days) {
    let date = new Date();
    date.setTime(date.getTime() + (expiration_days * 24 * 60 * 60 * 1000));

    const expires = "expires=" + date.toUTCString();
    document.cookie = cookie_name + "=" + cookie_value + "; " + expires + "; path=/";
}

function is_airport_in_cookie(target, icao) {
    var cookie_name = cookie_names[target];
    if (cookie_name == null) {
        return false;
    }

    var cookie_value = get_cookie(cookie_name);
    if (cookie_value == null) {
        return false;
    }

    return cookie_value.includes(icao);
}

function is_valid_icao(icao) {
    return icao.match(/^[A-Z]{4}$/g);
}



var airportsData = {}
var tableElement = null

function initialize_airports() {
    tableElement = document.getElementById('table-airport-selection')
    fetch_airports_data()
}

window.onload = initialize_airports

function create_settings_table() {
    for (var i = 0; i < airportsData.airports.length; i++) {
        var airportData = airportsData.airports[i]
        console.log(airportData.icao)

        var row = tableElement.insertRow(i)
        row.insertCell(0).innerHTML = airportData.name + "<br/ >(" + airportData.icao + ")"
        row.insertCell(1).innerHTML = ""
        row.insertCell(2).innerHTML = ""
        row.insertCell(3).innerHTML = ""
    }
}


function fetch_airports_data() {
    var dataUrl = '/airports.json'
    var fetchPromise = fetch(dataUrl)

    fetchPromise
        .then(response => response.json())
        .then(json => {
            airportsData = json
            console.log(airportsData)
            create_settings_table()
        })
        .catch(function (error) {
            console.log(error)
            alert('Error!')
        })
}