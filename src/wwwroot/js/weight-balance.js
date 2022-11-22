/************************************************
 * Variables
 ***********************************************/
var aircraftsData = {};

var selectElement = null;
var tableElement = null;
var chartElement = null;

var chartReference = null;



window.addEventListener("load", function () {
    selectElement = document.getElementById('select-weight-balance');
    tableElement = document.getElementById('table-weight-balance');
    chartElement = document.getElementById('weight-balance-chart');

    fetchAircraftsData(function () {
        createWeightBalanceTable();
    });
});

function fetchAircraftsData(callback) {
    var dataUrl = '/aircrafts.json';
    var fetchPromise = fetch(dataUrl);

    fetchPromise
        .then(response => response.json())
        .then(json => {
            aircraftsData = json;
            callback();
        })
        .catch(function (error) {
            console.log(error);
            alert('Error!');
        })
}

function getSelectedAircraftData() {
    var selectValue = selectElement.value;
    return aircraftsData[selectValue]
}

function showWeightBalanceSection() {
    var sectionElement = document.getElementById('section-weight-balance');
    var buttonElement = document.getElementById('button-weight-balance');

    sectionElement.style.display = 'block';
    buttonElement.style.display = 'none';
}



/************************************************
 * Weight balance table
 ***********************************************/
function createWeightBalanceTable() {
    var aircraftData = getSelectedAircraftData();
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
            var isInputCell = (weightCell.firstChild?.value != null);

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

    createWeightBalanceChart();
}

function insertWeightBalanceRow(type, position, name, modifiable, value, arm) {
    var row = tableElement.insertRow(position);

    row.dataset.name = name;
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


/************************************************
 * Weight balance chart
 ***********************************************/
function createWeightBalanceChart() {
    var x1 = 0;
    var x2 = 0;
    var y1 = 0;
    var y2 = 0;

    for (var row of tableElement.childNodes) {
        var name = row.dataset.name;

        var cells = row.childNodes;
        var weightCell = cells[1], armCell = cells[2];

        if (name == 'Totalvikt') {
            x1 = Number(armCell.innerHTML),
            y1 = Number(weightCell.innerHTML)
        }
        if (name == 'Torrvikt') {
            x2 = Number(armCell.innerHTML),
            y2 = Number(weightCell.innerHTML)
        }
    }

    const chartData = {
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


    var aircraftData = getSelectedAircraftData();
    var chartConfig = aircraftData.chart;


    chartReference?.destroy();

    chartReference = new Chart(chartElement, {
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
                    min: chartConfig.xMin,
                    max: chartConfig.xMax
                },
                y: {
                    min: chartConfig.yMin,
                    max: chartConfig.yMax
                }
            },
            drawLines: [
                {
                    coords: chartConfig.coords
                }
            ],
            drawArrow: {
                x1: x1,
                y1: y1,
                x2: x2,
                y2: y2,
                do: -5
            }
        }
    });
}

Chart.register({
    id: 'draw-lines',
    beforeDraw: function (chartInstance, easing) {
        var linesOpts = chartInstance.options.drawLines;

        if (linesOpts) {
            var yAxis = chartInstance.scales["y"];
            var xAxis = chartInstance.scales["x"];

            for (var i = 0; i < linesOpts.length; i++) {
                var lineOpts = linesOpts[i];
                var lineCoords = lineOpts.coords;

                var ctx = chartInstance.ctx;

                ctx.beginPath();
                ctx.lineWidth = 1;
                ctx.setLineDash([]);
                ctx.strokeStyle = "rgba(0, 0, 0, 1)";

                for (var y = 0; y < (lineCoords.length - 1); y++) {
                    var x1 = xAxis.getPixelForValue(lineCoords[y][0], 0, 0, true);
                    var y1 = yAxis.getPixelForValue(lineCoords[y][1], 0, 0, true);
                    var x2 = xAxis.getPixelForValue(lineCoords[y + 1][0], 0, 0, true);
                    var y2 = yAxis.getPixelForValue(lineCoords[y + 1][1], 0, 0, true);

                    ctx.moveTo(x1, y1);
                    ctx.lineTo(x2, y2);
                }

                ctx.closePath();
                ctx.stroke();
            }
        }
    }
});

Chart.register({
    id: 'draw-arrow',
    beforeDraw: function (chartInstance, easing) {
        var arrowOpts = chartInstance.options.drawArrow;

        if (arrowOpts) {
            var yAxis = chartInstance.scales["y"];
            var xAxis = chartInstance.scales["x"];

            var x1 = xAxis.getPixelForValue(arrowOpts.x1, 0, 0, true);
            var y1 = yAxis.getPixelForValue(arrowOpts.y1, 0, 0, true);
            var x2 = xAxis.getPixelForValue(arrowOpts.x2, 0, 0, true);
            var y2 = yAxis.getPixelForValue(arrowOpts.y2, 0, 0, true);

            var distanceOffset = arrowOpts?.do || 0;

            var ctx = chartInstance.ctx;
            ctx.beginPath();

            ctx.lineWidth = 1;
            ctx.setLineDash([]);
            ctx.strokeStyle = "rgba(100, 100, 100, 0.5)";

            drawCanvasArrow(ctx, x1, y1, x2, y2, distanceOffset);

            ctx.closePath();
            ctx.stroke();
        }
    }
})

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

function drawCanvasLine(ctx, x1, y1, x2, y2) {
    ctx.moveTo(x1, y1);
    ctx.lineTo(x2, y2);
}