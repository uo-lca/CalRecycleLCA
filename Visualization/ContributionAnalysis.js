function contributionAnalysis() {

    'use strict';

    // library globals
    /*global d3, console, window, $*/
    /*jslint plusplus: true*/

    /**
     * contributionAnalysis variables
     */
    /**  
     * Viz component sizes
     */
    var margin = {
        top: 40,
        right: 30,
        bottom: 30,
        left: 100
    },
        svgWidth = 500, // TO DO: Derive from bootstrap panel width
        groupHeight = 200,
        chartWidth = svgWidth - margin.left - margin.right,
        chartHeight = groupHeight - margin.top - margin.bottom,
        svgHeight = groupHeight * 4 + margin.top,
        /**  
         * d3 scales - map LCIA values to chart locations
         */
        x = d3.scale.linear().range([0, chartWidth]).nice(),
        y = d3.scale.ordinal().rangeRoundBands([0, chartHeight], 0.1),
        labelFormat = d3.format("^.2g"), // Format numbers with precision 2
        /**  
         * Configure X and Y axis for charts
         */
        xAxis = d3.svg.axis().scale(x).orient("bottom").tickFormat(labelFormat),
        yAxis = d3.svg.axis().scale(y).orient("left"),
        /**
         * Data sources
         */
        selectionData = [],
        jsonURL = //"http://www.rachelscanlon.com/api/gwpt",
        "LCIAresults.json",
        lciaData,
        /**
         * Indicator style configuration
         */
        indicatorStyle = {
            "Global Warming Air": {
                color: "rgb(175,228,235)",
                exponent: 8
            },
            "Acidification Air": {
                color: "rgb(201,245,143)",
                exponent: 8
            },
            "Eutrophication": {
                color: "rgb(232,194,230)",
                exponent: 5
            },
            "Ecotoxicity": {
                color: "rgb(242,221,134)",
                exponent: 8
            },
            "Human Health Cancer Potential": {
                color: "rgb(197,217,205)",
                exponent: 0
            },
            "Human Health Non Cancer Potential": {
                color: "rgb(219,175,175)",
                exponent: 0
            },
            "Human Health Criteria Air": {
                color: "rgb(178,232,188)",
                exponent: 5
            },
            "Smog Creation": {
                color: "rgb(183,185,247)",
                exponent: 7
            }
        },
        gridCols;

    /**
     * Generate chart for a scenario
     * @param {Object} svg              D3 selected svg element.
     * @param {Object} scenarioData     LCIA results for a scenario.
     * @param {Number} transY           Vertical starting point for the chart.
     * @param {String} labelText        X axis label prefix.
     * @param {String} barColor         Color for chart bars and bar connectors.
     * @param {Number} labelExponent    Power of 10 to use in X axis label.
     */
    function chartScenario(svg, scenarioData, transY, labelText, barColor, labelExponent) {
        var chartGroup = svg.append("g").attr("class", "chart")
            .attr("transform", "translate(0," + transY + ")"),
            barGroup,
            xLabel,
            tickDiff;

        chartGroup.append("text").attr("x", chartWidth / 2)
            .attr("y", 0)
            .style("text-anchor", "middle")
            .text(scenarioData.scenario);
//        chartGroup.append("rect")
//            .attr("class", "grid-background")
//            .attr("width", chartWidth)
//            .attr("height", chartHeight);
        chartGroup.append("line")
            .attr("class", "reference line")
            .attr("x1", x(0))
            .attr("y1", 0)
            .attr("x2", x(0))
            .attr("y2", chartHeight);
        barGroup = chartGroup.selectAll("g")
            .data(scenarioData.stages)
            .enter().append("g");
        barGroup.append("rect")
            .attr("class", "bar rect")
            .attr("height", y.rangeBand())
            .attr("x", function (d) {
                return x(d.xVal);
            })
            .attr("y", function (d) {
                return y(d.stage);
            })
            .attr("width", function (d) {
                return Math.abs(x(d.contribution) - x(0));
            })
            .attr("fill", barColor);
        barGroup.append("line")
            .attr("x1", function (d) {
                return x(d.endVal);
            })
            .attr("y1", function (d) {
                return y(d.stage);
            })
            .attr("x2", function (d) {
                return x(d.endVal);
            })
            .attr("y2", function (d, i) {
                if ((i + 1) < scenarioData.stages.length) {
                    return y(scenarioData.stages[i + 1].stage);
                }
                return y(d.stage) + y.rangeBand();
            })
            .style("stroke", function (d, i) {
                if ((i + 1) < scenarioData.stages.length) {
                    return barColor;
                }
                return "red";
            })
            .attr("class", function (d, i) {
                if ((i + 1) < scenarioData.stages.length) {
                    return "connect line";
                }
                return "end line";
            });
        barGroup.append("text")
            .attr("class", "bar text")
            .attr("x", function (d) {
                var barWidth = Math.abs(x(d.contribution) - x(0));
                if (barWidth < 50) {
                    return x(d.xVal) + barWidth + 5;
                } else {
                    return x(d.xVal) + barWidth / 2 - 5;
                }
            })
            .attr("y", function (d) {
                return y(d.stage) + (y.rangeBand() / 2);
            })
            .attr("dy", ".71em")
            .text(function (d) {
                return labelFormat(d.contribution);
            });
        chartGroup.append("g")
            .attr("class", "y axis")
            .call(yAxis);

        tickDiff = x(scenarioData.maxVal) - x(scenarioData.endVal);
        if (tickDiff < 25) {
            xAxis.tickValues([scenarioData.endVal]);
        } else {
            xAxis.tickValues([scenarioData.endVal, scenarioData.maxVal]);
        }
        xLabel = chartGroup.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + chartHeight + ")")
            .call(xAxis)
            .append("text")
            .attr("x", chartWidth)
            .attr("y", margin.bottom)
            .attr("dy", ".71em")
            .style("text-anchor", "end")
            .text(labelText);
        if (labelExponent !== 0) {
            xLabel.append("tspan")
                .attr("dy", "-.7em")
                .text(labelExponent);
        }
    }

    /**
     * Generate charts for data at index.
     * @param {Object} svg D3 selected svg node.
     * @param {Number} index Index into LCIA data array.
     */
    function drawCharts(svg, indicatorData, index) {
        /**
         * Derive min and max values across all scenarios
         */
        var minVal = d3.min(indicatorData.chartData, function (d) {
            return d.minVal;
        }),
            maxVal = d3.max(indicatorData.chartData, function (d) {
                return d.maxVal;
            }),
            lastHeight = 0, // current height of appended charts
            labelText, // text for x axis label
            labelExp = indicatorData.indicatorStyle.exponent, // Power of 10 in label
            xAxisBuffer; // Start X axis a little before minVal

        xAxisBuffer = (maxVal - minVal) / 50;
        svg.selectAll("g").remove();
        svg.append("g")
            .attr("transform", "translate(" + margin.left + "," + margin.top + ")");
        x.domain([minVal - xAxisBuffer, maxVal]);
        y.domain(indicatorData.stages);
        lastHeight = 0;
        labelText = indicatorData.ReferenceQuantity;
        if (labelExp !== 0) {
            labelText += " X 10";
        }
        indicatorData.chartData.forEach(function (d) {
            chartScenario(svg, d, lastHeight, labelText, indicatorData.indicatorStyle.color, indicatorData.indicatorStyle.exponent);
            lastHeight += groupHeight;
        });
        /*
        svg.append("line")
            .attr("class", "reference line")
            .attr("x1", x(0))
            .attr("y1", margin.top + y.rangeBand() * 0.1)
            .attr("x2", x(0))
            .attr("y2", lastHeight - y.rangeBand() * 0.1);
        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + lastHeight + ")")
            .call(xAxis)
            .append("text")
            .attr("x", chartWidth)
            .attr("y", margin.bottom)
            .attr("dy", ".71em")
            .style("text-anchor", "end")
            .text("E+008 kg CO2-Equiv.");
            */
    }

    /**
     * Visualize data at index.
     * @param {Object} svg D3 selected svg node.
     * @param {Number} index Index into LCIA data array.
     */
    function visualizeIndicatorData(svg, index) {
        var indicatorData = lciaData[index],
            scenarios = indicatorData.Scenarios,
            displayFactor = 1;

        if (!indicatorData.hasOwnProperty("chartData")) {
            indicatorData.stages = d3.keys(scenarios[0]).filter(function (key) {
                return key !== "Scenario";
            });
            indicatorData.indicatorStyle = indicatorStyle[indicatorData.IndicatorType];
            displayFactor = Math.pow(10, indicatorData.indicatorStyle.exponent);
            indicatorData.chartData = [];
            scenarios.forEach(function (d) {
                //first calculate the left edge for each stage
                var endVal = 0.0,
                    i,
                    barData,
                    scenarioData = {};
                scenarioData.scenario = d.Scenario;
                scenarioData.stages = [];
                scenarioData.minVal = 0.0;
                scenarioData.maxVal = 0.0;
                for (i = 0; i < indicatorData.stages.length; i++) {
                    barData = {};
                    barData.stage = indicatorData.stages[i];
                    barData.startVal = endVal;
                    barData.contribution = parseFloat(d[indicatorData.stages[i]]) / displayFactor;
                    barData.endVal = barData.startVal + barData.contribution;
                    barData.xVal = barData.startVal < barData.endVal ? barData.startVal : barData.endVal;
                    barData.width = Math.abs(barData.contribution);
                    endVal = barData.endVal;
                    scenarioData.stages.push(barData);
                    if (endVal < scenarioData.minVal) {
                        scenarioData.minVal = endVal;
                    } else if (endVal > scenarioData.maxVal) {
                        scenarioData.maxVal = endVal;
                    }
                }
                scenarioData.endVal = endVal;
                indicatorData.chartData.push(scenarioData);
            });

        }
        drawCharts(svg, indicatorData, index);
    }

    /**
     * Convert indicator index to a grid column id.
     *
     * @param {Number} index            Index to indicator data.
     * @param {Boolean} makeSelector    Flags when to prepend selector symbol to id
     *
     * @return {String} The grid column id
     */
    function indexToId(index, makeSelector) {
        var idString = "";
        if (makeSelector) {
            idString = "#";
        }
        return idString + "indicator-" + index.toString();
    }

    /**
     * Add grid column, panel to grid row for indicator charts.
     *
     * @param {Number} index        Index to indicator data.
     */
    function addPanel(index) {
        var indicator,
            gridRow, // row containing panels
            gridCol,
            panel,
            panelHeading,
            panelTitle,
            panelBody,
            svg; // svg inside panel body

        indicator = lciaData[index].IndicatorType;
        gridRow = d3.select("#chartcontainer");
        gridCol = gridRow.append("div")
            .attr("class", "col-md-6")
            .attr("id", indexToId(index, false));
        panel = gridCol.append("div").attr("class", "panel panel-default");
        panelHeading = panel.append("div").attr("class", "panel-heading");
        panelBody = panel.append("div").attr("class", "panel-body");
        panelTitle = panelHeading.append("h3").attr("class", "panel-title");
        panelTitle.text(indicator);
        panelHeading.style("background-color", indicatorStyle[indicator].color);
        svg = panelBody
            .append("svg")
            .attr({
                width: svgWidth,
                height: svgHeight
            })
            .append("g")
            .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

        visualizeIndicatorData(svg, index);
    }

    /**
     * Change event handler for selection list.
     * Adds chart panels for selected indicators that do not already exist.
     * Removes chart panels for unselected indicators that are displayed.
     */
    function onSelectChange(e) {
        var selectOptions,
            indicatorCol; // Grid column with indicator id

        selectOptions = d3.select(".selectpicker").selectAll("option");
        selectOptions[0].forEach(function (opt, i) {
            indicatorCol = d3.select(indexToId(i, true));
            if (opt.selected) {
                if (indicatorCol.empty()) {
                    addPanel(i);
                }
            } else {
                if (!indicatorCol.empty()) {
                    indicatorCol.remove();
                }
            }
        });
    }

    /**
     * Prepare select element. Enter LCIA indicator types.
     */
    function prepareSelect() {
        var jqSelect = $(".selectpicker"),
            indicators;

        jqSelect.selectpicker(); // Initialize bootstrap-select plugin
        indicators = lciaData.map(function (d) {
            return d.IndicatorType;
        });
        d3.select(".selectpicker")
            .selectAll("option")
            .data(indicators)
            .enter()
            .append("option")
            .text(function (d) {
                return d;
            });
        jqSelect.change(onSelectChange);
        jqSelect.selectpicker("refresh");
        jqSelect.selectpicker("val", "Global Warming Air");
        jqSelect.selectpicker("refresh");
        onSelectChange();
    }

    function init() {
        d3.json(jsonURL, function (error, data) { // Load LCIA results

            if (error) {
                return console.warn(error);
            }
            lciaData = data;
            prepareSelect();

        });
    }

    /**
     * Begin contributionAnalysis execution
     */
    init();
}
