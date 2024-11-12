var options = {
    angle: 0,
    lineWidth: 0.2,
    radiusScale: 1,
    pointer: {
        length: 0.6,
        strokeWidth: 0.04,
        color: '#000000'
    },
    limitMax: true,
    colorStart: '#4caf50',
    colorStop: '#f44336',
    strokeColor: '#e0e0e0',
    generateGradient: true
};

var gauge;

function initializeGauge() {
    var target = document.getElementById('gaugeCanvas');
    gauge = new Gauge(target).setOptions(options);
    gauge.maxValue = 1300;
    gauge.setMinValue(0);
    gauge.animationSpeed = 32;
}

function setWeight(weight) {

    initializeGauge();

    if (weight >= 0 && weight <= 1300) {
        gauge.set(weight);
        changeColor(weight);
    } else if (weight > 1300) {
        gauge.set(1300);
        changeColor(1300);
    }
}

function changeColor(weight) {
    var percentage = (weight / gauge.maxValue) * 100;

    if (percentage <= 50) {
        gauge.setOptions({
            colorStart: '#4caf50',
            colorStop: '#4caf50'
        });
    } else if (percentage <= 75) {
        gauge.setOptions({
            colorStart: '#ff9800',
            colorStop: '#ff9800'
        });
    } else {
        gauge.setOptions({
            colorStart: '#f44336',
            colorStop: '#f44336'
        });
    }
}