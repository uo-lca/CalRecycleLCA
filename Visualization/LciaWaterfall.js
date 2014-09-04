/**
 * LCIA visualization using waterfall diagram
 */

/* Reference paths for Visual Studio Intellisense */
/// <reference path="LCA.js" />
/// <reference path="d3.min.js" />
/// <reference path="waterfall.js" />
/// <reference path="d3-tip.js" />
/// <reference path="colorbrewer.js" />
/// <reference path="LciaWaterfall.html" />

function LciaWaterfall() {

    // library globals - used to avoid jslint errors
    /*global d3, colorbrewer, LCA */

    // Current selections
    var selectedFragmentID = 8,
        fragmentName = "";

    // SVG margins
    var margin = {
        top: 10,
        right: 50,
        bottom: 30,
        left: 20
    },
        width = 600 - margin.left - margin.right,   // diagram width
        height = 1000 - margin.top - margin.bottom;  // diagram height

    var stageColors = colorbrewer.Spectral[7].slice(1, 6);       

    var waterfall = d3.waterfall()
        .width(width)
        .colors(stageColors);

    var formatNumber = d3.format("^.2g"),   // Round numbers to 2 significant digits
        xAxis = d3.svg.axis().orient("bottom").tickFormat(formatNumber),                                    
        svg,
        transitionTime = 250,   // d3 transition time in ms
        updateOnly = false,     // Flag when diagram should be updated, as opposed to drawn from scratch
        waterfallData = {};

    /**
     * Initial preparation of svg element.
     */
    function prepareSvg() {
        svg = d3.select("#chartcontainer")
            .append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .append("g")
            .attr("transform", "translate(" + margin.left + "," + margin.top + ")");
    }
     
    /**
     * Display waterfall chart for a scenario
      * @param {String}  scenario    Element at waterfall.scenarios[index]
     *  @param {Number}  index       
     *  @param {Number}  bottom      bottom of last chart (X coordinate)  
     * @returns {Number} bottom of this chart    
     */
    function displayScenario(scenario, index, bottom) {
        var top = margin.top + bottom,
            padding = 10,
            chartGroup, barGroup,
            chartID = "scenario" + index,
            scenarioSegments = waterfall.segments[index],
            transitionTime = 0,
            chartHeight = 0,
            minVal = 0.0, maxVal = 0.0;

        if (updateOnly) {
            transitionTime = 250;
            chartGroup = svg.select("#" + chartID).select(".chart");
        } else {
            chartGroup = svg.append("g").attr("id", chartID)
                .attr("transform", "translate(" + margin.left + "," + top + ")");
            chartGroup.append("text").attr("x", 0)
                .attr("y", 0)
                .style("text-anchor", "left")
                .text(scenario);
            chartGroup = chartGroup.append("g")
                .attr("class", "chart")
                .attr("transform", "translate(" + padding + "," + padding + ")");
        }
        if (scenarioSegments.length > 0) {
            chartHeight = scenarioSegments[scenarioSegments.length - 1].y
                        + waterfall.segmentHeight() + waterfall.segmentPadding();
            minVal = d3.min(scenarioSegments, function (d) {
                return d.endVal;
            });
            maxVal = d3.max(scenarioSegments, function (d) {
                return d.endVal;
            });
            xAxis.tickValues([minVal, maxVal]);
        }
        if (updateOnly) {
            chartGroup.select(".grid-background")
                .attr("height", chartHeight);
            
        } else {
            chartGroup.append("rect")
                .attr("class", "grid-background")
                .attr("width", width)
                .attr("height", chartHeight);            
        }
        chartGroup.select(".grid").remove();
        chartGroup.select(".axis").remove();
        if (scenarioSegments.length > 0) {
            chartGroup.append("g")
                .attr("class", "grid")
                .attr("transform", "translate(0," + chartHeight + ")")
                .call(xAxis.tickSize(-chartHeight));
            chartGroup.append("g")
                .attr("class", "axis")
                .attr("transform", "translate(0," + chartHeight + ")")
                .call(xAxis.tickSize(0));
        }            
        barGroup = chartGroup.selectAll(".bar.g")
            .data(scenarioSegments);

        barGroup.enter().append("g")
                .attr("class", "bar g");

        barGroup.exit().remove();

        barGroup.append("rect")
            .attr("class", "bar rect")
            .attr("height", waterfall.segmentHeight())
            .attr("x", function (d) {
                return d.x;
            })
            .attr("y", function (d) {
                return d.y;
            })
            .attr("width", function (d) {
                return d.width;
            })
            .style("fill", function (d) {
                return d.color;
            });

        barGroup.append("text")
            .attr("class", "bar text")
            .attr("x", function (d) {
                return (d.width < 50) ?
                    d.x + d.width + 5 :
                    d.x + d.width / 2 - 5;
            })
            .attr("y", function (d) {
                return d.y + (waterfall.segmentHeight() / 2);
            })
            .attr("dy", ".71em")
            .text(function (d) {
                return formatNumber(d.value);
            });

        barGroup.append("line")
            .attr("class", "bar line")
            .attr("x1", function (d) {
                return d.endX;
            })
            .attr("y1", function (d) {
                return d.y + waterfall.segmentHeight();
            })
            .attr("x2", function (d) {
                return d.endX;
            })
            .attr("y2", function (d) {
                return d.y + waterfall.segmentHeight() + waterfall.segmentPadding();
            })
            .style("stroke", function (d) {
                return d.color;
            });

            return top + chartHeight + margin.bottom + 2*padding; 
    }

    function setWaterfallData() {
        waterfallData = LCA.loadedData.waterfall;
        waterfall.scenarios(waterfallData.scenarios)
        .stages(waterfallData.stages)
        .values(waterfallData.values);
    }

    /**
     * Display LCIA results
     */
    function displayResults() {
        var bottom = 0;

        if (!updateOnly) {
            svg.selectAll("g").remove();
        }
        waterfall.layout();
        xAxis.scale(waterfall.xScale);
        waterfallData.scenarios.forEach(function (scenario, index) {
            bottom = displayScenario(scenario, index, bottom);
        });
    }

    /**
     * Callback function for data load.
     */
    function onDataLoaded() {
        if ("waterfall" in LCA.loadedData) {
            if (LCA.loadedData.waterfall !== null) {
                setWaterfallData();
                displayResults();
            }
        }
    }

    /**
     * Starting point for lciaComputation
     */
    function init() {
        // New API methods have not yet been developed.
        // Load test data

        //var filteredMethodsURL;

        //LCA.baseURI = "http://publictest.calrecycle.ca.gov/LCIATool/api/";

        //processesURL = LCA.baseURI + "process?flows=1";
        //impactCategoriesURL = LCA.baseURI + "impactcategory";
        //methodsURL = LCA.baseURI + "lciamethod";
        //lciaResultsURL = LCA.baseURI + "LCIAComputation";
        //filteredMethodsURL = methodsURL + "?impactCategoryid=" + selectedImpactCategoryID;

        //LCA.prepareSelect(processesURL, "#processSelect", "ProcessID", onProcessChange, selectedProcessID);
        //LCA.prepareSelect(impactCategoriesURL, "#impactCategorySelect", "ImpactCategoryID",
        //    onImpactCategoryChange, selectedImpactCategoryID);
        //LCA.prepareSelect(filteredMethodsURL, "#lciaMethodSelect", "LCIAMethodID",
        //    onMethodChange, selectedMethodID);

        prepareSvg();
        LCA.loadData("waterfall", true, onDataLoaded);

    }

    LCA.init(init);
}