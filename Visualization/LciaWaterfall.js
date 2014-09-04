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
        right: 20,
        bottom: 30,
        left: 20
    },
        width = 600 - margin.left - margin.right,   // diagram width
        height = 1000 - margin.top - margin.bottom;  // diagram height

    var waterfall = d3.waterfall()
        .width(width)
        .colors(colorbrewer.Spectral[5]);

    var formatNumber = d3.format("^.2g"),   // Round numbers to 2 significant digits
                                            // TODO: make this user configurable
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
            chartGroup, bars,
            chartID = "scenario" + index,
            scenarioSegments = waterfall.segments[index],
            transitionTime = 0;

        if (updateOnly) {
            transitionTime = 250;
            chartGroup = svg.select("#" + chartID);
        } else {
            chartGroup = svg.append("g").attr("id", chartID)
                .attr("transform", "translate(" + margin.left + "," + top + ")");

            chartGroup.append("text").attr("x", 0)
                .attr("y", 0)
                .style("text-anchor", "left")
                .text(scenario);
        }
        bars = chartGroup.selectAll("rect")
            .data(scenarioSegments);
        
        bars.enter().append("rect")
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

        bars.exit().remove();

        if (scenarioSegments.length > 0) {
            return top + scenarioSegments[scenarioSegments.length - 1].y + waterfall.segmentHeight() + margin.bottom;
        } else {
            return top;
        }
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

        waterfallData.scenarios.forEach(function (scenario, index) {
            bottom = displayScenario(scenario, index, bottom);
        });
    }

    /**
     * Callback function for data load.
     */
    function onDataLoaded() {
        if ("waterfall" in LCA.loadedData) {
            if (LCA.loadedData.waterfall != null) {
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