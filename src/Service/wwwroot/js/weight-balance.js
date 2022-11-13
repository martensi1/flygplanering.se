/************************************************
 * Flight Planner
 ***********************************************/
var aircraftsData = {};

var selectElement = null;
var tableElement = null;



function showWeightBalanceSection() {
    var sectionElement = document.getElementById('section-weight-balance');
    var buttonElement = document.getElementById('button-weight-balance');

    sectionElement.style.display = 'block';
    buttonElement.style.display = 'none';
}

function initializeWeightBalance() {
    selectElement = document.getElementById('select-weight-balance');
    tableElement = document.getElementById('table-weight-balance');

    fetchWeightBalanceData();
}

window.onload = initializeWeightBalance;



function fetchWeightBalanceData() {
    var dataUrl = '/aircrafts.json';
    var fetchPromise = fetch(dataUrl);

    fetchPromise
        .then(response => response.json())
        .then(json => {
            aircraftsData = json;
            createWeightBalanceTable();
        })
        .catch(function (error) {
            console.log(error);
            alert('Error!');
        })
}

function createWeightBalanceTable() {
    var selectValue = selectElement.value;

    var aircraftData = aircraftsData[selectValue];
    var weightPoints = aircraftData.weightPoints;

    clearWeightBalanceTable();

    for (var i = 0; i < weightPoints.length; i++) {
        var weightPoint = weightPoints[i];

        insertWeightBalanceRow('weight-point', i, weightPoint.name, weightPoint.modifiable,
            weightPoint.value, weightPoint.arm.toFixed(1), weightPoint.note || "");
    }

    insertWeightBalanceRow('summarize', weightPoints.length - 1, 'Torrvikt', false, 0, 0, ``);
    insertWeightBalanceRow('summarize', weightPoints.length + 1, 'Totalvikt', false, 0, 0, `Max ${aircraftData.maxWeight} kg`);

    updateWeightBalanceTable();
}

function updateWeightBalanceTable() {
    var totalWeight = 0;
    var totalMoment = 0;

    for (var row of tableElement.childNodes) {
        var type = row.dataset.rowtype;

        var cells = row.childNodes;
        var weightCell = cells[1], armCell = cells[2], momentCell = cells[3];

        if (type == 'weight-point') {
            var isInputCell = (weightCell.firstChild != null);

            var weight = Number(isInputCell ? weightCell.firstChild.value : weightCell.innerHTML);
            var arm = Number(armCell.innerHTML);

            if (isNaN(weight)) {
                weight = 0;
            }

            if (isInputCell) {
                var weightStr = weight.toString();
                weight = Number(weightStr.substring(0, 3));
                weightCell.firstChild.value = weight.toString();
            }

            var moment = Math.round(weight * arm);
            momentCell.innerHTML = moment;

            totalWeight += weight;
            totalMoment += moment;
        }
        else if (type == 'summarize') {
            weightCell.innerHTML = parseFloat(totalWeight.toFixed(1));
            armCell.innerHTML = (totalMoment / totalWeight).toFixed(1);
            momentCell.innerHTML = totalMoment;
        }
    }
}

function insertWeightBalanceRow(type, position, name, modifiable, value, arm) {
    var row = tableElement.insertRow(position);
    row.dataset.rowtype = type;

    var valueHtml = modifiable ?
        `<input type="number" min="0" maxlength="3" inputmode="numeric" pattern="[0-9]*" placeholder="Skriv ett värde..."
            value="${value}" onkeypress="return event.charCode >= 48 && event.charCode <= 57" oninput="updateWeightBalanceTable()" />` : value;

    row.insertCell(0).innerHTML = name;
    row.insertCell(1).innerHTML = valueHtml;
    row.insertCell(2).innerHTML = arm;
    row.insertCell(3).innerHTML = 0;

    if (type == 'summarize') {
        row.style.border = '2px solid';
    }
}

function clearWeightBalanceTable() {
    tableElement.innerHTML = '';
}