/**
 * Process Flow visualization using sankey diagram
 */
function processFlow() {


    // library globals
    /*global d3, console, window, $, colorbrewer, LCA */

    /**
     * lciaComputation variables
     */
    var // Data loaded from web API
    processList = [],
        intFlowList = [],
        // Web API methods
        baseURI = "http://rachelscanlon.com/api/",
        processesURL = baseURI + "process",
        intFlowURL = "IntermediateFlows.json",
        // Current selections
        selectedProcessID = 3;
    /**
     * d3 variables
     */
    var margin = {
        top: 10,
        right: 20,
        bottom: 30,
        left: 20
    },
        width = 600 - margin.left - margin.right,
        svgHeight = 1000,
        chartHeight = 100;

    var color = d3.scale.ordinal();

    var x = d3.scale.linear()
        .rangeRound([0, width]);

    var labelFormat = d3.format("^.2g"); // Format numbers with precision 2;

    var svg;

    /**
     * Initial preparation of svg element.
     */
    function prepareSvg() {
        svg = d3.select("#chartcontainer").append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", svgHeight);
        svg.append("g")
            .attr("class", "chartgroup")
            .attr("transform", "translate(" + margin.left + "," + margin.top + ")");
        svg.append("g")
            .attr("class", "legendgroup")
            .attr("transform", "translate(" + margin.left + "," + (chartHeight + margin.top) + ")");
    }


    /**
     * Display intermediate product flows for selected process.
     */
    function displayResults() {

        d3.json(intFlowURL, visualizeResults);
    }

    /**
     * Change event handler for process selection list.
     * Triggers LCIA computation update.
     */
    function onProcessChange() {
        var paramURL;

        selectedProcessID = this.options[this.selectedIndex].value;
        displayResults();
    }

    /**
     * Callback function for IntermediateFlows
     * @param {string} error          Web API GET error
     * @param {Array} results  JSON data from web API
     */
    function visualizeResults(error, results) {

    }

    /**
     * Starting point for IntermediateFlows
     */
    function init() {

        LCA.prepareSelect(processesURL, "#processSelect", "ProcessID",
            onProcessChange, selectedProcessID);

        prepareSvg();
        displayResults();
    }

    init();
}