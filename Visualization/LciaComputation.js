/**
 * LCIA computation - gets data from web API and visualizes it with the help of d3.
 */
function lciaComputation() {


    // library globals
    /*global d3, console, window, colorbrewer */

    /**
     * lciaComputation variables
     */
    var // Data loaded from web API
    processList = [],
        impactCategoryList = [],
        methodList = [],
        lciaResultData = [],
        // Web API methods
        baseURI = "http://rachelscanlon.com/api/",
        processesURL = baseURI + "process",
        impactCategoriesURL = baseURI + "impactcategory",
        methodsURL = baseURI + "lciamethod",
        lciaResultsURL = baseURI + "LCIAComputation",
        // Current selections
        selectedProcessID = 2,
        selectedImpactCategoryID = 10,
        selectedMethodID = 1,
        // Score for current selection
        impactScore = 0;
    /**
     * d3 variables
     */
    var margin = {
        top: 20,
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

    var xAxis = d3.svg.axis()
        .scale(x)
        .orient("bottom");
    //
    // Color scales to be used in chart. Index is (ImpactCategoryID - 1)
    //
    var colorScales = [colorbrewer.Purples, colorbrewer.YlGn, colorbrewer.GnBu,
                       colorbrewer.YlOrRd, colorbrewer.Reds, colorbrewer.Blues,
                       colorbrewer.Greens, colorbrewer.YlOrBr, colorbrewer.BuGn,
                       colorbrewer.PuBuGn, colorbrewer.Greys, colorbrewer.YlGnBu];

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
            .attr("transform", "translate(" + margin.left + "," + chartHeight + ")");
    }

    /**
     * Update list of LCIA methods
     */
    function updateMethods() {
        var paramURL;

        paramURL = methodsURL + "?impactCategoryid=" + selectedImpactCategoryID;

        d3.json(paramURL, function (error, jsonData) {
            var selectOptions;

            if (error) {
                window.alert("Unable to refresh LCIA Methods. " + error);
            } else {
                jsonData.sort(compareNames);

                selectOptions = d3.select("#lciaMethodSelect")
                    .selectAll("option")
                    .data(jsonData);

                selectOptions.enter()
                    .append("option");

                selectOptions.exit().remove();

                selectOptions.attr("value", function (d) {
                    return d.LCIAMethodID;
                })
                    .text(function (d) {
                        return d.Name;
                    });
                //
                // Select first method
                //
                selectedMethodID = jsonData[0].LCIAMethodID;
                d3.select("#lciaMethodSelect")
                    .select("option")
                    .attr("selected", true);
                displayResults();
            }

        });
    }

    /**
     * Execute LCIA computation for selected process and method, then display results.
     */
    function displayResults() {
        var paramURL;

        paramURL = lciaResultsURL + "?processid=" + selectedProcessID + "&lciaMethodId=" + selectedMethodID;
        d3.json(paramURL, visualizeResults);
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
     * Change event handler for method selection list.
     */
    function onMethodChange() {

        selectedMethodID = this.options[this.selectedIndex].value;
        displayResults();
    }

    /**
     * Change event handler for impact category selection list.
     * Triggers update to LCIA methods.
     */
    function onImpactCategoryChange() {
        selectedImpactCategoryID = this.options[this.selectedIndex].value;
        updateMethods();
    }

    /**
     * Compare function used to sort array of objects by Name.
     */
    function compareNames(a, b) {
        return d3.ascending(a.Name, b.Name);
    }

    /**
     * Compare function used to sort LCIA results in descending order
     */
    function compareLciaResults(a, b) {
        return d3.descending(a.LCIAResult, b.LCIAResult);
    }

    /**
     * Prepare select element. Load with data and initialize selection
     * @param {string} jsonURL          URL for JSON data (web API endpoint or file)
     * @param {string} selectID         SELECT HTML element id
     * @param {string} oidName          Property name of object ID field.
     * @param {function} changeHandler  Function for handling selection update.
     * @param {number} initialValue     Default value (selected object ID).
     */
    function prepareSelect(jsonURL, selectID, oidName, changeHandler, initialValue) {

        d3.json(jsonURL, function (error, jsonData) {
            var selectOptions;

            if (error) {
                window.alert(error);
            }
            jsonData.sort(compareNames);

            selectOptions = d3.select(selectID)
                .on("change", changeHandler)
                .selectAll("option")
                .data(jsonData)
                .enter()
                .append("option")
                .attr("value", function (d) {
                    return d[oidName];
                })
                .text(function (d) {
                    return d.Name;
                });
            //
            // Initialize selection
            //
            selectOptions.filter(function (d, i) {
                return d[oidName] == initialValue;
            })
                .attr("selected", true);
        });

    }

    /**
     * Create header for legend, if it does not already exist.
     * Make it invisible if there are no flows.
     *
     * @param {number} flowX          X coordinate for flow header
     * @param {number} resultX        X coordinate for result header
     * @param {number} headerY        Y coordinate for headers
     * @param {boolean} flowsExist    Flag if flows exist.
     */
    function makeLegendHeader(flowX, resultX, headerY, flowsExist) {
        var legendGroup = svg.select(".legendgroup"),
            header = legendGroup.selectAll(".legendheader"),
            visibility = flowsExist ? "visible" : "hidden";
        if (header.empty() && flowsExist) {
            legendGroup.append("text").attr({
                class: "legendheader",
                x: flowX,
                y: headerY
            })
                .text("Flow");
            header = legendGroup.append("text").attr({
                class: "legendheader",
                x: resultX,
                y: headerY
            })
                .text("LCIA Result");
        } else {
            header.style("visibility", flowsExist ? "visible" : "hidden");
        }
    }

    /**
     * Make legend for chart, listing all flows with LCIA result.
     *
     * Flows that cannot be charted because the result is very small or negative are listed
     * below the legend colors.
     *
     * @param {Array} flowData          Flows sorted by LCIA result in descending order.
     */
    function makeLegend(flowData) {
        var rowHeight = 20,
            boxSize = 18,
            colPadding = 6,
            textY = 9,
            colXs = [0, boxSize + colPadding, width - 75],
            legend,
            newRows,
            squares,
            flows,
            flowHeader;

        makeLegendHeader(colXs[1], colXs[2], textY, flowData && flowData.length > 0);
        //TO DO: Make legend data update work, and only remove unused rows.
        svg.select(".legendgroup").selectAll(".legend").remove();
        // Update legend data
        legend = svg.select(".legendgroup").selectAll(".legend").data(flowData);
        // Add rows, if necessary
        newRows = legend.enter().append("g").attr("class", "legend");
        newRows.filter(function (d, i) {
            return d.LCIAResult > 0 && (x(d.x1) - x(d.x0) > 1);
        })
            .append("rect")
            .attr("x", colXs[0])
            .attr("width", boxSize)
            .attr("height", boxSize);
        newRows.append("text")
            .attr("x", colXs[1])
            .attr("y", textY)
            .attr("dy", ".35em")
            .attr("class", "flowname");
        newRows.append("text")
            .attr("x", colXs[2])
            .attr("y", textY)
            .attr("dy", ".35em")
            .attr("class", "lciaresult");
        // Remove unused rows
        //legend.exit().remove();

        legend.attr("transform", function (d, i) {
            var rowY = (i + 1) * rowHeight;
            return "translate(0," + rowY + ")";
        });
        squares = legend.selectAll("rect")
            .style("fill", function (d) {
                return color(d.Flow);
            });
        flows = legend.selectAll(".flowname")
            .text(function (d) {
                return d.Flow;
            });
        legend.selectAll(".lciaresult")
            .text(function (d) {
                return d.LCIAResult.toPrecision(4);
            });
    }

    /**
     * Callback function for LCIA computation.
     * @param {string} error          Web API GET error
     * @param {Array} lciaResultData  JSON data from web API
     */
    function visualizeResults(error, lciaResultData) {
        var flowList = [],
            runningTotal = 0,
            rects,
            colorClassSize = 9, // Number of classes in colorbrewer scale, ranges from 3 to 9
            colorScale = colorbrewer.PuRd,
            colorIndex = 1, // Index to color scale (ImpactCategoryID - 1)
            reverseScale; // Clone of color scale in reverse order (dark to light)

        if (error) {
            window.alert(error);
        }
        impactScore = 0;
        lciaResultData.sort(compareLciaResults);
        flowList = lciaResultData.map(function (d) {
            return d.Flow;
        });
        color.domain(flowList);
        if (flowList.length < 3)
            colorClassSize = 3;
        else if (flowList.length < 9)
            colorClassSize = flowList.length;
        colorIndex = selectedImpactCategoryID - 1;
        if (colorIndex in colorScales)
            colorScale = colorScales[colorIndex];
        reverseScale = colorScale[colorClassSize].slice();
        reverseScale.reverse();
        color.range(reverseScale);
        /**
         * Compute impact score.
         * Add rect start and end points for each flow
         */
        lciaResultData.forEach(function (d) {
            impactScore += +d.LCIAResult;
            d.x0 = runningTotal;
            // ignore negative values in chart
            if (+d.LCIAResult > 0) {
                runningTotal += +d.LCIAResult;
            }
            d.x1 = runningTotal;
        });

        x.domain([0, runningTotal]);
        d3.select("#impactScore").text(impactScore.toPrecision(4));
        /**
         * Update/Add/Delete rect data
         */
        rects = d3.select(".chartgroup").selectAll("rect").data(lciaResultData);
        rects.enter().append("rect");
        rects.exit().remove();
        rects.attr("width", function (d) {
            return x(d.x1) - x(d.x0);
        })
            .attr("x", function (d) {
                return x(d.x0);
            })
            .attr("y", 10)
            .attr("height", 30)
            .style("fill", function (d) {
                return color(d.Flow);
            });
        makeLegend(lciaResultData);
    }

    /**
     * Starting point for lciaComputation
     */
    function init() {
        var filteredMethodsURL = methodsURL + "?impactCategoryid=" + selectedImpactCategoryID;

        prepareSelect(processesURL, "#processSelect", "ProcessID",
            onProcessChange, selectedProcessID);
        prepareSelect(impactCategoriesURL, "#impactCategorySelect", "ImpactCategoryID",
            onImpactCategoryChange, selectedImpactCategoryID);
        prepareSelect(filteredMethodsURL, "#lciaMethodSelect", "LCIAMethodID",
            onMethodChange, selectedMethodID);
        prepareSvg();
        displayResults();
    }

    init();
}
