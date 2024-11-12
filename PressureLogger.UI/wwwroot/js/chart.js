let myChart;

window.drawWeightChart = function (labels, data, chartTitle) {
    var ctx = document.getElementById('weightChart').getContext('2d');

    ctx.canvas.width = ctx.canvas.offsetWidth;
    ctx.canvas.height = 200;

    if (myChart) {
        myChart.destroy();
    }

    myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Waga (g)',
                data: data,
                borderColor: 'rgba(75, 192, 192, 1)',
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                fill: true,
                tension: 0.1
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: chartTitle,
                    font: {
                        size: 18,
                    },
                    padding: {
                        top: 10,
                        bottom: 30
                    }
                }
            },
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Czas'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Waga (g)'
                    }
                }
            }
        }
    });
}