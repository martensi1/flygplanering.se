/************************************************
 * Flight Planner
 ***********************************************/
var settings = {}


window.onload = function () {
    loadSettings();
    fetchAirportsData();
}


function fetchAirportsData() {
    fetch('/airports.json')
        .then(response => response.json())
        .then(json => {
            createSettingsTable(json);
        })
        .catch(function (error) {
            console.log(error);
            alert('Error!');
        })
}

function createSettingsTable(airportsData) {
    var airportList = airportsData.airports;
    airportList.sort(function (a, b) {
        return a.name.localeCompare(b.name);
    })


    for (var i = 0; i < airportList.length; i++) {
        var name = airportList[i].name;
        var icao = airportList[i].icao;

        var tableElement = document.getElementById('table-airport-selection');
        var row = tableElement.insertRow(i);

        row.insertCell(0).innerHTML = name + "<br/ >(" + icao + ")";
        row.insertCell(1).innerHTML = createCheckboxHtml(icao, "METAR");
        row.insertCell(2).innerHTML = createCheckboxHtml(icao, "TAF");
        row.insertCell(3).innerHTML = createCheckboxHtml(icao, "NOTAM");
    }
}

function createCheckboxHtml(icao, type) {
    return '<div><input type="checkbox" class="form-check-input" aria-label="..." checked="checked" value="" onclick="onChange(this, \'' + type + '\', \'' + icao + '\')" /></div>'
}

function onChange(element, type, icao) {
    if (element.checked) {
        settings[type].push(icao);
    }
    else {
        settings[type] = settings[type].filter(i => i != icao);
    }

    console.log(settings)
}

function removeAirport(type, icao) {
    settings[type] = settings[type].filter(i => i != icao);
    console.log(settings)
}



function loadSettings() {
    var cookieValue = getCookie("fpl-settings");
    
    if (cookieValue == null) {
        alert('Något gick fel! Kontakta sidans administratör')
    }

    decodedCookieValue = decodeURIComponent(cookieValue)
    settings = JSON.parse(decodedCookieValue);

    console.log(settings)
}

function saveSettings() {
    if (settings == null) {
        return;
    }

    var jsonString = JSON.stringify(settings);
    console.log(jsonString);

    cookieValue = encodeURIComponent(jsonString);
    setCookie("fpl-settings", cookieValue, 10)

    location.href = "/"
}


function setCookie(cookieName, cookieValue, expirationYears) {
    let date = new Date();
    date.setTime(date.getTime() + (expirationYears * 24 * 60 * 60 * 1000 * 365));

    const expires = "expires=" + date.toUTCString();
    document.cookie = cookieName + "=" + cookieValue + "; " + expires + "; path=/";
}

function getCookie(cookieName) {
    let name = cookieName + "=";
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