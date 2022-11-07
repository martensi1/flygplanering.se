/************************************************
 * Flight Planner
 ***********************************************/
var aircraftsData = {}

var selectElement = null
var tableElement = null



function initialize_weight_balance() {
    selectElement = document.getElementById('select-weight-balance')
    tableElement = document.getElementById('table-weight-balance')

    fetch_weight_balance_data()
}

window.onload = initialize_weight_balance



function fetch_weight_balance_data() {
    var dataUrl = '/aircrafts.json'
    var fetchPromise = fetch(dataUrl)

    fetchPromise
        .then(response => response.json())
        .then(json => {
            aircraftsData = json
            create_weight_balance_table()
        })
        .catch(function (error) {
            console.log(error)
            alert('Error!')
        })
}

function create_weight_balance_table() {
    var selectValue = selectElement.value

    var aircraftData = aircraftsData[selectValue]
    var weightPoints = aircraftData.weightPoints

    clear_weight_balance_table()

    for (var i = 0; i < weightPoints.length; i++) {
        var weightPoint = weightPoints[i]

        insert_weight_balance_row('weight-point', i, weightPoint.name, weightPoint.modifiable,
            weightPoint.value, weightPoint.arm, weightPoint.note || "")
    }

    insert_weight_balance_row('summarize', weightPoints.length - 1, 'Torrvikt', false, 0, 0, ``)
    insert_weight_balance_row('summarize', weightPoints.length + 1, 'Totalvikt', false, 0, 0, `Max ${aircraftData.maxWeight} kg`)

    update_weight_balance_table()
}

function update_weight_balance_table() {
    var totalWeight = 0
    var totalMoment = 0

    for (var row of tableElement.childNodes) {
        var type = row.dataset.rowtype

        var cells = row.childNodes
        var weightCell = cells[1], armCell = cells[2], momentCell = cells[3]

        if (type == 'weight-point') {
            var weight = Number(weightCell.firstChild?.value || weightCell.innerHTML)
            var arm = Number(armCell.innerHTML)

            if (isNaN(weight)) {
                weight = 0;
            }

            var moment = Math.round(weight * arm)
            momentCell.innerHTML = moment

            totalWeight += weight
            totalMoment += moment
        }
        else if (type == 'summarize') {
            weightCell.innerHTML = parseFloat(totalWeight.toFixed(1))
            armCell.innerHTML = (totalMoment / totalWeight).toFixed(1)
            momentCell.innerHTML = totalMoment
        }
    }
}

function insert_weight_balance_row(type, position, name, modifiable, value, arm, note = null) {
    var row = tableElement.insertRow(position)
    row.dataset.rowtype = type

    var valueHtml = modifiable ?
            `<input type="number" min="0" placeholder="Enter value..." value="${value}" oninput="update_weight_balance_table()" />` : value

    row.insertCell(0).innerHTML = name
    row.insertCell(1).innerHTML = valueHtml
    row.insertCell(2).innerHTML = arm
    row.insertCell(3).innerHTML = 0
    //row.insertCell(4).innerHTML = note || ''

    if (type == 'summarize') {
        row.style.border = '2px solid'
    }
}

function clear_weight_balance_table() {
    tableElement.innerHTML = ''
}



function show_weight_balance_section() {
    var sectionElement = document.getElementById('section-weight-balance')
    var buttonElement = document.getElementById('button-weight-balance')

    sectionElement.style.display = 'block'
    buttonElement.style.display = 'none'
}