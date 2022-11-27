/**************************************************
 * Initialization
 *************************************************/

/**
 * Initialization function run at page load
 */
window.addEventListener("load", function () {
    loadSettings();

    fetchAirportsData(function () {
        createSettingsTable();
    });
});



/**************************************************
 * Data Fetch
 *************************************************/
var airportsData = {};

/**
 * Fetches airports data from the server
 * @param {function} callback Callback function to call when the data is fetched
 */
function fetchAirportsData(callback) {
    var dataUrl = '/airports.json';
    var fetchPromise = fetch(dataUrl);

    fetchPromise
        .then(response => response.json())
        .catch(function (error) {
            console.log('Error fetching ' + dataUrl + ': ' + error);
        })
        .then(json => {
            airportsData = json;
            callback();
        })
}



/**************************************************
 * Settings functions
 *************************************************/
var settingsObject = {};
const SETTINGS_COOKIE_NAME = "fpl-airports";


function loadSettings() {
    var cookieValue = getCookie(SETTINGS_COOKIE_NAME);

    if (cookieValue == null) {
        alert('Något gick fel! Kontakta sidans administratör');
    }

    decodedCookieValue = decodeURIComponent(cookieValue);
    settingsObject = JSON.parse(decodedCookieValue);
}

function saveSettings() {
    if (settingsObject == null) {
        return;
    }

    var jsonString = JSON.stringify(settingsObject);
    console.log(jsonString);

    cookieValue = encodeURIComponent(jsonString);
    setCookie(SETTINGS_COOKIE_NAME, cookieValue, 10);

    document.getElementById("settings-form").submit();
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



/**************************************************
 * Table functions
 *************************************************/
function createSettingsTable() {
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
    var exists = settingsObject[type].includes(icao);
    return '<input type="checkbox" ' + (exists ? 'checked="checked"' : '') + ' onclick="onCheckboxClicked(this, \'' + type + '\', \'' + icao + '\')" />';
}

function onCheckboxClicked(element, type, icao) {
    if (element.checked) {
        settingsObject[type].push(icao);
    }
    else {
        settingsObject[type] = settingsObject[type].filter(i => i != icao);
    }
}