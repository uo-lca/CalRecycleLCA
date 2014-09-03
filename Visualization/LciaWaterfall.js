/**
 * LCIA visualization using waterfall diagram
 */

/* Reference paths for Visual Studio Intellisense */
/// <reference path="LCA.js" />
/// <reference path="d3.min.js" />
/// <reference path="waterfall.js" />
/// <reference path="d3-tip.js" />
/// <reference path="LciaWaterfall.html" />

function FragmentFlows() {

    // library globals - used to avoid jslint errors
    /*global d3, colorbrewer, LCA, console */

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
        height = 500 - margin.top - margin.bottom;  // diagram height

    var waterfall = d3.waterfall()
        .size([width, height]);
    var formatNumber = d3.format("^.2g"),   // Round numbers to 2 significant digits
                                            // TODO: make this user configurable
        svg,
        color = d3.scale.ordinal(),
        transitionTime = 250,   // d3 transition time in ms
        updateOnly = false;     // Flag when diagram should be updated, as opposed to drawn from scratch
     
    /**
     * Display waterfall chart for one scenario
     */
    function displayScenario(scenario, index) {
        var topPos = margin.top + (height * index),
            chartGroup, bars
        chartID = "scenario" + index,
        xScale = waterfall.xScale,
        yScale = waterfall.yScale,
        transitionTime = 0;

        if (updateOnly) {
            transitionTime = 250;
            chartGroup = svg.select("#" + chartID);
        } else {
            chartGroup = svg.append("g").attr("id", chartID)
                .attr("transform", "translate(" + margin.left + "," + topPos + ")");

            chartGroup.append("text").attr("x", 0)
                .attr("y", 0)
                .style("text-anchor", "left")
                .text(scenario);
        }
        bars = chartGroup.selectAll("rect")
            .data(waterfall.segments[index]);
        
        bars.enter().append("rect")
            .attr("class", "bar rect")
            .attr("height", yScale.rangeBand())
            .attr("x", function (d) {
                return xScale(Math.min(d.startVal, d.endVal));
            })
            .attr("y", function (d) {
                return yScale(d.stage);
            })
            .attr("width", function (d) {
                return Math.abs(xScale(d.value) - xScale(0));
            })
    }

    /**
     * Display LCIA results
     * @param {Object}  data    Results prepared as waterfall input data
     * @param {Boolean}  rebuild    Flag to indicate if this is a full redraw or an update
     */
    function displayResults(data, rebuild) {

        var link, node;

        if (rebuild) {
            svg.selectAll("g").remove();
        }
        waterfall.scenarios(data.scenarios)
            .stages(data.stages)
            .values(data.values)
            .layout();

        updateOnly = !rebuild;
        waterfall.scenarios.forEach(displayScenario);
    }

    /**
     * Callback function for data load.
     */
    function onDataLoaded() {
        if ("waterfall" in LCA.loadedData) {
            displayResults( LCA.loadedData.waterfall, true);
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
        LCA.loadData("waterfall.json", true, onDataLoaded);

    }

    LCA.init(init);
}