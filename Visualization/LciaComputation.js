/**
 * LCIA computation - gets data from web API and visualizes it with the help of d3.
 */
function lciaComputation() {

    // library globals
    /*global d3, colorbrewer, LCA */

    /**
     * lciaComputation variables
     */
    var // Data loaded from web API
        lciaResultData = [],
        // Current resource ID selections - 0 means none selected
        selectedProcessID = 0,
        selectedImpactCategoryID = 0,
        selectedMethodID = 0,
        // Score for current selection
        impactScore = 0;

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

    var xAxis = d3.svg.axis()
        .scale(x)
        .orient("bottom")
        .ticks(4)
        .tickFormat(labelFormat);

    //
    // Color scales to be used in chart. Index is (ImpactCategoryID - 1)
    //
    var colorScales = [colorbrewer.Purples, colorbrewer.YlGn, colorbrewer.GnBu,
                       colorbrewer.YlOrRd, colorbrewer.Reds, colorbrewer.Blues,
                       colorbrewer.Greens, colorbrewer.YlOrBr, colorbrewer.BuGn,
                       colorbrewer.PuBuGn, colorbrewer.Greys, colorbrewer.YlGnBu];

    var svg, msg;

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
            .attr("class", "x axis")
            .attr("transform", "translate(" + margin.left + "," + (chartHeight - margin.bottom) + ")");
        svg.append("g")
            .attr("class", "legendgroup")
            .attr("transform", "translate(" + margin.left + "," + (chartHeight + margin.top) + ")");
    }

    function prepareMsg() {
        msg = d3.select("#chartcontainer").append("p").append("i").text("Get data...");
    }

    /**
     * Change event handler for process selection list.
     * Triggers LCIA computation update.
     */
    function onProcessChange() {
        selectedProcessID = this.options[this.selectedIndex].value;
        loadFlows();
        getLciaResults();
    }

    /**
     * Change event handler for method selection list.
     */
    function onMethodChange() {

        selectedMethodID = this.options[this.selectedIndex].value;
        getLciaResults();
    }

    /**
     * Change event handler for impact category selection list.
     * Triggers update to LCIA methods.
     */
    function onImpactCategoryChange() {
        selectedImpactCategoryID = this.options[this.selectedIndex].value;
        selectedMethodID = 0;
        loadMethods();
    }

    /**
     * Compare function used to sort LCIA results in descending order
     */
    function compareLciaResults(a, b) {
        return d3.descending(a.result, b.result);
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
            header = legendGroup.selectAll(".legendheader");
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
            flows;

        makeLegendHeader(colXs[1], colXs[2], textY, flowData && flowData.length > 0);
        //TO DO: Make legend data update work, and only remove unused rows.
        svg.select(".legendgroup").selectAll(".legend").remove();
        // Update legend data
        legend = svg.select(".legendgroup").selectAll(".legend").data(flowData);
        // Add rows, if necessary
        newRows = legend.enter().append("g").attr("class", "legend");
        newRows.filter(function (d) {
            return d.result > 0 && (x(d.x1) - x(d.x0) > 1);
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
                return color(d.flowID);
            });
        flows = legend.selectAll(".flowname")
            .text(function (d) {
                return (d.flowID in LCA.indexedData.flows) ?
                    LCA.indexedData.flows[d.flowID].name :
                    "FlowID: " + d.flowID.toString;
            });
        legend.selectAll(".lciaresult")
            .text(function (d) {
                return d.result.toPrecision(4);
            });
    }

    /**
     * Callback function for LCIA computation.
     * @param {Array} results  JSON data from web API
     */
    function visualizeResults(results) {
        var flowList = [],
            runningTotal = 0,
            rects,
            colorClassSize = 9, // Number of classes in colorbrewer scale, ranges from 3 to 9
            colorScale = colorbrewer.PuRd,
            colorIndex = 1, // Index to color scale (ImpactCategoryID - 1)
            reverseScale; // Clone of color scale in reverse order (dark to light)

        if (results.length > 0) {
            msg.style("display", "none");
        } else {
            msg.text("No impact to display.");
        }
        impactScore = 0;
        lciaResultData = results;
        lciaResultData.sort(compareLciaResults);
        flowList = lciaResultData.map(function (d) {
            return d.flowID;
        });
        color.domain(flowList);
        if (flowList.length < 3) {
            colorClassSize = 3;
        } else if (flowList.length < 9) {
            colorClassSize = flowList.length;
        }
        colorIndex = selectedImpactCategoryID - 1;
        if (colorIndex in colorScales) {
            colorScale = colorScales[colorIndex];
        }
        reverseScale = colorScale[colorClassSize].slice();
        reverseScale.reverse();
        color.range(reverseScale);
        /**
         * Compute impact score.
         * Add rect start and end points for each flow
         */
        lciaResultData.forEach(function (d) {
            impactScore += +d.result;
            d.x0 = runningTotal;
            // ignore negative values in chart
            if (+d.result > 0) {
                runningTotal += +d.result;
            }
            d.x1 = runningTotal;
        });

        x.domain([0, runningTotal]);
        d3.select("#impactScore").text(impactScore.toPrecision(4));
        svg.select(".x.axis")
            .call(xAxis)
            .style("visibility", lciaResultData.length > 0 ? "visible" : "hidden");
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
                return color(d.flowID);
            });
        makeLegend(lciaResultData);
    }

    function addVersionToDupName(processArray) {
        if (processArray.length > 1) {
            var curName = processArray[0].name;
            for (var i = 1; i < processArray.length; ++i) {
                var curP = processArray[i],
                    prevP = processArray[i - 1];
                if (curP.name === curName) {
                    curP.name = curP.name.concat(" (Version ", curP.version, ")");
                    if (prevP.name === curName) {
                        prevP.name = prevP.name.concat(" (Version ", prevP.version, ")");
                    }                   
                } else {
                    curName = curP.name;
                }
            }
        }
    }

    function onProcessesLoaded() {
        if ("processes" in LCA.loadedData) {
            var processArray = LCA.loadedData.processes;

            processArray.sort(LCA.compareNames);            
            if (processArray.length > 0) {
                // If no default setting, select first process
                if (selectedProcessID === 0) {
                    selectedProcessID = processArray[0].processID;
                }
                loadFlowProperties();
                loadFlows();
                addVersionToDupName(processArray);
                LCA.loadSelectionList(processArray,
                        "#processSelect", "processID", onProcessChange, selectedProcessID);
                getLciaResults();
            }
        }
    }

    function onImpactCategoriesLoaded() {
        if ("impactcategories" in LCA.loadedData) {
            if (LCA.loadedData.impactcategories.length > 0) {
                // If no default setting, select first category
                selectedImpactCategoryID = LCA.loadedData.impactcategories[0].impactCategoryID;
                loadMethods();
                LCA.loadSelectionList(LCA.loadedData.impactcategories,
                        "#impactCategorySelect", "impactCategoryID", onImpactCategoryChange, selectedImpactCategoryID);
            }
        }
    }

    function onMethodsLoaded() {
        if ("lciamethods" in LCA.loadedData) {
            if (selectedMethodID === 0 && LCA.loadedData.lciamethods.length > 0) {
                // If no default setting, select first LCIA method
                selectedMethodID = LCA.loadedData.lciamethods[0].lciaMethodID;

                LCA.loadSelectionList(LCA.loadedData.lciamethods,
                   "#lciaMethodSelect", "lciaMethodID", onMethodChange, selectedMethodID);
                getLciaResults();
            }
        }
    }

    function onFlowPropertiesLoaded() {
        if ("flowproperties" in LCA.loadedData) {
            LCA.indexedData.flowProperties = LCA.indexData("flowproperties", "flowPropertyID");
        }
    }

    function onFlowsLoaded() {
        if ("flows" in LCA.loadedData) {
            LCA.indexedData.flows = LCA.indexData("flows", "flowID");
            onResultsLoaded();
        }
    }

    function onResultsLoaded() {
        if ("lciaresults" in LCA.loadedData && "flows" in LCA.indexedData) {
            visualizeResults(LCA.loadedData.lciaresults);
        }
    }

    /**
      * Get all processes with elementary flows from web API
      */
    function loadProcesses() {
        LCA.loadData("processes", false, onProcessesLoaded, "flowtypes/2");
    }

    /**
      * Get all flows related to selected process
      */
    function loadFlows() {
        LCA.loadData("flows", false, onFlowsLoaded, "processes/" + selectedProcessID);
    }

    /**
      * Get all flow properties related to selected process
      */
    function loadFlowProperties() {
        LCA.loadData("flowproperties", false, onFlowPropertiesLoaded, "processes/" + selectedProcessID);
    }

    /**
      * Get all impact categories from web API
      */
    function loadImpactCategories() {
        LCA.loadData("impactcategories", false, onImpactCategoriesLoaded);
    }

    /**
     * Load LCIA methods having selected Impact Category
     */
    function loadMethods() {
        LCA.loadData("lciamethods", false, onMethodsLoaded, "impactcategories/" + selectedImpactCategoryID);
    }

    /**
      * If a process and a method have been selected,
      * get LCIA computation results from web API
      */
    function getLciaResults() {
        if (selectedProcessID > 0 && selectedMethodID > 0) {
            msg.style("display", "block").text("Compute LCIA...");
            LCA.loadData("lciaresults", false, onResultsLoaded,
            "processes/" + selectedProcessID + "/lciamethods/" + selectedMethodID);
        }
    }

    /**
     * Starting point for lciaComputation
     */
    function init() {

        prepareMsg();
        prepareSvg();
        LCA.startSpinner("chartcontainer");
        if ("processid" in LCA.urlVars) {
            selectedProcessID = +LCA.urlVars.processid;
        }
        loadProcesses();
        loadImpactCategories();
    }

    LCA.init(init);
}