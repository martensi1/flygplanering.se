/**************************************************
 * Initialization
 *************************************************/

/**
 * Initialization function run at page load
 */
window.addEventListener("load", function () {
    fetchAircraftsData("JFK", function () {
        createTable();
    });
});



/**************************************************
 * Event callbacks
 *************************************************/

/**
 * Called from: add weight/balance button
 *
 * Sets the display CSS attribute of the weight balance section to 'block'
 * Hides the add weight balance button
 */
function showWeightBalance() {
    var sectionElement = document.getElementById('section-weight-balance');
    var buttonElement = document.getElementById('button-weight-balance');

    sectionElement.style.display = 'block';
    buttonElement.style.display = 'none';
}

/**
 * Called from: table cells with modifiable input
 *
 * Verifies that the new input is valid, then refreshes the table
 */
function onWeightCellChanged(element) {
    // Verify new value is valid
    var value = Number(element.value);

    if (isNaN(value)) {
        value = 0;
    }

    var valueStr = value.toString();
    element.value = valueStr.substring(0, 3);

    // Refresh table
    updateTableCalculations();
}



/**************************************************
 * Data Fetch
 *************************************************/
var aircraftsData = {};

/**
 * Fetches aircrafts data for a specific organization from the server
 * @param {string} organization Organization to fetch aircrafts data for
 * @param {function} callback Callback function to call when the data is fetched
 */
function fetchAircraftsData(organization, callback) {
    fetchAircraftsJson(json => {
        if (organization in json) {
            aircraftsData = json[organization];
            callback();
        }
        else {
            console.log(`Organization ${organization} not found in aircrafts.json`);
        }
    });
}

/**
 * Fetches aircrafts data file from the server
 * @param {function} callback Callback function to call when the data is fetched
 */
function fetchAircraftsJson(callback) {
    var dataUrl = '/aircrafts.json';
    var fetchPromise = fetch(dataUrl);

    fetchPromise
        .then(response => response.json())
        .catch(function (error) {
            console.log('Error fetching ' + dataUrl + ': ' + error);
        })
        .then(json => {
            callback(json);
        });
}

/**
 * Get aircraft data for the currently selected aircraft
 * @returns {object} Aircraft data
 * @throws If the aircraft is not found in the aircrafts data
 */
function getSelectedAircraftData() {
    var selectElement = document.getElementById('select-weight-balance');
    var selectValue = selectElement.value;

    if (selectValue != null && selectValue in aircraftsData) {
        return aircraftsData[selectValue];
    }
    else {
        throw `Aircraft ${selectValue} not found in aircraft data`;
    }
}



/**************************************************
 * Table functions
 *************************************************/
var tableElement = null;

const NAME_CELL_INDEX = 0;
const WEIGHT_CELL_INDEX = 1;
const ARM_CELL_INDEX = 2;
const MOMENT_CELL_INDEX = 3;

/**
 * Creates and fills weight balance table with data
 */
function createTable() {
    tableElement = document.getElementById('table-weight-balance');
    deleteTableRows();

    var aircraftData = getSelectedAircraftData();
    var weightPoints = aircraftData.weightPoints;

    for (var i = 0; i < weightPoints.length; i++) {
        var weightPoint = weightPoints[i];

        var rowId = 'weight-point-' + i;
        insertInputRow(rowId, i, weightPoint.name, weightPoint.value, weightPoint.arm.toFixed(1), weightPoint.modifiable)
    }

    insertSummarizeRow('dry-weight', weightPoints.length - 1, 'Torrvikt');
    insertSummarizeRow('gross-weight', weightPoints.length + 1, 'Totalvikt');

    updateTableCalculations();
}

/**
 * Inserts a input row in the weight balance table
 * @param {string} rowId Row identifier
 * @param {number} insertPosition Position to insert the row at
 * @param {string} nameValue Name cell value
 * @param {number} weightValue Weight cell value (cm)
 * @param {number} armValue Arm cell value (kg)
 * @param {boolean} isModifiable If the weight value is modifiable
 */
function insertInputRow(rowId, insertPosition, nameValue, weightValue, armValue, isModifiable) {
    var weightHtml = isModifiable ?
        `<input type="number" min="0" maxlength="3" inputmode="numeric" pattern="[0-9]*" placeholder="Skriv ett värde..."
            value="${weightValue}" onkeypress="return event.charCode >= 48 && event.charCode <= 57" oninput="onWeightCellChanged(this)" />` : weightValue;

    var rowType = isModifiable ? 'input-row' : 'static-row';
    insertTableRow(rowId, rowType, insertPosition, nameValue, weightHtml, armValue);
}

/**
 * Inserts a summarize row in the weight balance table
 * @param {string} rowId Row identifier
 * @param {number} insertPosition Position to insert the row at
 * @param {string} nameValue Name cell value
 */
function insertSummarizeRow(rowId, insertPosition, nameValue) {
    insertTableRow(rowId, 'summarize-row', insertPosition, nameValue, 0, 0, true);
}

/**
 * Inserts a row in the weight balance table
 * @param {string} rowId Row identifier
 * @param {string} rowType Row type, can be either 'static-row', 'input-row' or 'summarize-row'
 * @param {number} insertPosition Position to insert the row at
 * @param {string} nameHtml Name cell HTML
 * @param {string} weightHtml Weight cell HTML
 * @param {string} armHtml Arm cell HTML
 * @param {boolean} thickBorder If a thick border CSS should be applied to the row, defaults to false
 */
function insertTableRow(rowId, rowType, insertPosition, nameHtml, weightHtml, armHtml, thickBorder = false) {
    var row = tableElement.insertRow(insertPosition);

    if (!['static-row', 'input-row', 'summarize-row'].includes(rowType)) {
        throw `Invalid row type ${rowType}`;
    }

    row.dataset.rowid = rowId;
    row.dataset.rowtype = rowType;

    row.insertCell(NAME_CELL_INDEX).innerHTML = nameHtml;
    row.insertCell(WEIGHT_CELL_INDEX).innerHTML = weightHtml;
    row.insertCell(ARM_CELL_INDEX).innerHTML = armHtml;
    row.insertCell(MOMENT_CELL_INDEX).innerHTML = 0;

    if (thickBorder) {
        row.style.border = '2px solid';
    }

    return row;
}

/**
 * Returns name, weight, arm and moment cell for the specified table row element
 * @param {HTMLTableRowElement} rowElement Row element from the weight balance table
 * @returns {object} Object with cells
 */
function getTableRowCells(rowElement) {
    var cells = rowElement.childNodes;

    return {
        name: cells[NAME_CELL_INDEX],
        weight: cells[WEIGHT_CELL_INDEX],
        arm: cells[ARM_CELL_INDEX],
        moment: cells[MOMENT_CELL_INDEX]
    }
}

/**
 * Find and return table row element based on the specified row id
 * @param {string} rowId
 * @returns {HTMLTableRowElement} Table row element
 * @throws If the row id can not be found
 */
function findTableRow(rowId) {
    var rowElement = document.querySelector(`[data-rowid="${rowId}"]`);

    if (rowElement == null) {
        throw `Could not find table row with id ${rowId}`;
    }

    return rowElement;
}

/**
 * Get data for a specific table row in the weight balance table
 * @param {string} rowId Row id
 * @returns {object} Row data
 * @throws If the row id can not be found
 */
function getTableRowData(rowId) {
    var rowElement = findTableRow(rowId);
    var cells = getTableRowCells(rowElement);

    const rowType = rowElement.dataset.rowtype;

    return {
        id: rowId,
        type: rowType,
        name: cells.name.innerHTML,
        weight: Number((rowType == 'input-row') ? cells.weight.firstChild.value : cells.weight.innerHTML),
        arm: Number(cells.arm.innerHTML),
        moment: Number(cells.moment.innerHTML)
    };
}

/**
 * Get data for all table rows in the weight balance table
 * @returns {object} List of objects with row data
 */
function getTableRowsData() {
    var rowsData = [];

    for (var rowElement of tableElement.childNodes) {
        const rowId = rowElement.dataset.rowid;
        const rowData = getTableRowData(rowId);

        rowsData.push(rowData);
    }

    return rowsData;
}

/**
 * Updates a row in the weight balance table with new data.
 * The row to update is decided by the id attribute in the row data
 * @param {object} rowData Row data, should be the same format as the row data returned by 'getTableRowsData'
 * @throws If the row id can not be found
 */
function updateTableRow(rowData) {
    var rowElement = findTableRow(rowData.id);
    var cells = getTableRowCells(rowElement);

    cells.name.innerHTML = rowData.name;
    cells.arm.innerHTML = rowData.arm;
    cells.moment.innerHTML = rowData.moment;

    if (rowData.type == 'input-row') {
        cells.weight.firstChild.value = rowData.weight;
    }
    else {
        cells.weight.innerHTML = rowData.weight;
    }
}

/**
 * Deletes all rows from the weight balance table
 */
function deleteTableRows() {
    tableElement.innerHTML = '';
}



/**************************************************
 * Weight and balance calculation
 *************************************************/

/**
 * Calculates new moment and arm values for the rows in the weight balance table
 */
function updateTableCalculations() {
    var accumulatedWeight = 0;
    var accumulatedMoment = 0;

    var rows = getTableRowsData();

    for (var row of rows) {
        if (row.type == 'summarize-row') {
            const weightHtml = accumulatedWeight;
            const momentHtml = accumulatedMoment;
            const armHtml = calculateArm(accumulatedWeight, accumulatedMoment);

            row.weight = weightHtml;
            row.moment = momentHtml;
            row.arm = armHtml;
        }
        else {
            const weight = row.weight;
            const arm = row.arm;

            const moment = calculateMoment(weight, arm);
            row.moment = moment;

            accumulatedWeight += weight;
            accumulatedMoment += moment;
        }

        updateTableRow(row);
    }

    createChart();
}

/**
 * Calculates moment from weight and arm
 * @param {number} weight Weight in kg
 * @param {number} arm Arm in cm
 * @returns {number} Moment in kgcm rounded to the nearest integer
 */
function calculateMoment(weight, arm) {
    var moment = weight * arm;
    return Math.round(moment);
}

/**
 * Calculates the arm from weight and moment
 * @param {number} weight Weight in kg
 * @param {number} moment Moment in kgcm
 * @returns {number} Arm in cm formatted to one decimal
 */
function calculateArm(weight, moment) {
    var arm = moment / weight;
    return arm.toFixed(1);
}



/**************************************************
 * Chart functions
 *************************************************/
var chartElement = null;
var chartReference = null;


/**
 * Creates a new chart (Chart.js) based on the weight balance data
 **/
function createChart() {
    chartElement = document.getElementById('weight-balance-chart');

    grossWeightRow = getTableRowData('gross-weight');
    dryWeightRow = getTableRowData('dry-weight');

    x1 = grossWeightRow.arm;
    y1 = grossWeightRow.weight;
    x2 = dryWeightRow.arm;
    y2 = dryWeightRow.weight;

    const chartDefinition = getChartDefinition(x1, y1, x2, y2);

    chartReference?.destroy();
    chartReference = new Chart(chartElement, chartDefinition);
}

/**
 * Creates the chart definition object
 * @param {number} x1 From x
 * @param {number} y1 From y
 * @param {number} x2 To x
 * @param {number} y2 To y
 * @returns {object} Chart definition
 */
function getChartDefinition(x1, y1, x2, y2) {
    const chartData = getChartData(x1, y1, x2, y2);

    const aircraftData = getSelectedAircraftData();
    const aircraftChartData = aircraftData.chart;

    return {
        type: 'scatter',
        data: chartData,
        options: {
            animation: {
                duration: 0
            },
            scales: {
                x: {
                    type: 'linear',
                    position: 'bottom',
                    min: aircraftChartData.xMin,
                    max: aircraftChartData.xMax
                },
                y: {
                    min: aircraftChartData.yMin,
                    max: aircraftChartData.yMax
                }
            },
            lineCoords: aircraftChartData.coords,
            pointArrow: {
                x1: x1,
                y1: y1,
                x2: x2,
                y2: y2,
                do: -5
            }
        }
    };
}

/**
 * Creates the chart data object
 * @param {number} x1 From x
 * @param {number} y1 From y
 * @param {number} x2 To x
 * @param {number} y2 To y
 * @returns {object} Chart data
 */
function getChartData(x1, y1, x2, y2) {
    return {
        datasets: [
            {
                label: 'Torrvikt',
                data: [{
                    x: x2,
                    y: y2
                }],
                pointRadius: 5,
                backgroundColor: 'rgb(210, 100, 100)'
            },
            {
                label: 'Totalvikt',
                data: [{
                    x: x1,
                    y: y1
                }],
                pointRadius: 5,
                backgroundColor: 'rgb(50, 76, 168)'
            }
        ],
    };
}

/**
 * Register plug-in for drawing the weight balance limit lines
 */
Chart.register({
    id: 'draw-weight-balance-limits',
    beforeDraw: function (chartInstance, _) {
        var coords = chartInstance.options.lineCoords;
        if (coords == null) {
            throw 'Line coord options missing in chart options';
        }

        var yAxis = chartInstance.scales["y"];
        var xAxis = chartInstance.scales["x"];
        var ctx = chartInstance.ctx;

        ctx.beginPath();
        ctx.lineWidth = 1;
        ctx.setLineDash([]);
        ctx.strokeStyle = "rgba(0, 0, 0, 1)";

        for (var y = 0; y < (coords.length - 1); y++) {
            var x1 = xAxis.getPixelForValue(coords[y][0], 0, 0, true);
            var y1 = yAxis.getPixelForValue(coords[y][1], 0, 0, true);
            var x2 = xAxis.getPixelForValue(coords[y + 1][0], 0, 0, true);
            var y2 = yAxis.getPixelForValue(coords[y + 1][1], 0, 0, true);

            ctx.moveTo(x1, y1);
            ctx.lineTo(x2, y2);
        }

        ctx.closePath();
        ctx.stroke();
    }
});

/**
 * Register plug-in for drawing the weight balance arrow between the two points
 */
Chart.register({
    id: 'draw-weight-balance-arrow',
    beforeDraw: function (chartInstance, _) {
        var arrowData = chartInstance.options.pointArrow;
        if (arrowData == null) {
            throw 'Point arrow options missing in chart options';
        }

        var yAxis = chartInstance.scales["y"];
        var xAxis = chartInstance.scales["x"];

        var x1 = xAxis.getPixelForValue(arrowData.x1, 0, 0, true);
        var y1 = yAxis.getPixelForValue(arrowData.y1, 0, 0, true);
        var x2 = xAxis.getPixelForValue(arrowData.x2, 0, 0, true);
        var y2 = yAxis.getPixelForValue(arrowData.y2, 0, 0, true);

        var distanceOffset = arrowData?.do || 0;

        var ctx = chartInstance.ctx;
        ctx.beginPath();

        ctx.lineWidth = 1;
        ctx.setLineDash([]);
        ctx.strokeStyle = "rgba(100, 100, 100, 0.5)";

        drawCanvasArrow(ctx, x1, y1, x2, y2, distanceOffset);

        ctx.closePath();
        ctx.stroke();
    }
})

/**
 * Draws an arrow between two points on a 2D canvas
 * @param {CanvasRenderingContext2D} ctx Canvas context
 * @param {number} x1 From x (in pixels)
 * @param {number} y1 From y (in pixels)
 * @param {number} x2 To x (in pixels)
 * @param {number} y2 To y (in pixels)
 * @param {number} distanceOffset The amount of pixels shorter/longer the arrow head should be relative to x2 and y2
 */
function drawCanvasArrow(ctx, x1, y1, x2, y2, distanceOffset) {
    const deltaX = x2 - x1;
    const deltaY = y2 - y1;

    const arrowAngle = Math.atan2(deltaY, deltaX);
    x2 = x2 + Math.cos(arrowAngle) * distanceOffset;
    y2 = y2 + Math.sin(arrowAngle) * distanceOffset;

    const headLength = 10;
    const headAngle = Math.PI / 6;

    ctx.moveTo(x1, y1);
    ctx.lineTo(x2, y2);

    ctx.lineTo(
        x2 - headLength * Math.cos(arrowAngle - headAngle),
        y2 - headLength * Math.sin(arrowAngle - headAngle)
    );

    ctx.moveTo(x2, y2);

    ctx.lineTo(
        x2 - headLength * Math.cos(arrowAngle + headAngle),
        y2 - headLength * Math.sin(arrowAngle + headAngle)
    );
}

/**
 * Draws a line between two points on a 2D canvas
 * @param {CanvasRenderingContext2D} ctx Canvas context
 * @param {number} x1 From x (in pixels)
 * @param {number} y1 From y (in pixels)
 * @param {number} x2 To x (in pixels)
 * @param {number} y2 To y (in pixels)
 */
function drawCanvasLine(ctx, x1, y1, x2, y2) {
    ctx.moveTo(x1, y1);
    ctx.lineTo(x2, y2);
}