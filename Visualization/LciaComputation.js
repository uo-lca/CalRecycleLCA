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
        impactScore = 0,
        startupProcessID = 116,
        elementaryFlows = {},
        inputFlows = [],
        outputFlows = [];

    /**
     * d3 variables
     */
    var margin = {
        top: 10,
        right: 200,
        bottom: 30,
        left: 20
    },
        width = 760 - margin.left - margin.right,
        svgHeight = 1000,
        chartHeight = 100,
        barHeight = 30,
        textPadding = 6;

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

    var svg = null, msg = null, flowTables = [],
        flowColumns = ["Name", "Magnitude", "Unit"];    // Flow table column names

    /**
     * Initial preparation of svg element.
     */
    function prepareSvg() {
        svg = d3.select("#chartcontainer").append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", svgHeight);
        svg.append("g")
            .attr("class", "chartgroup")
            .attr("transform", "translate(" + margin.left + "," + margin.top + ")")
            .append("text").attr("id", "chartheader").style("font-weight", "bold").text("Lifecycle Impact Assessment");
        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(" + margin.left + "," + (chartHeight - margin.bottom) + ")");
        svg.append("g")
            .attr("class", "legendgroup")
            .attr("transform", "translate(" + margin.left + "," + (chartHeight + margin.top) + ")");
        
    }

    function prepareMsg() {
        msg = d3.select("#chartcontainer").append("p").append("i");
    }

    /**
      * Initial preparation of intermediate flow elements
      */
    function prepareFlowTables() {
        var parentSelection = d3.select("#chartcontainer"),
            panelSelection = parentSelection
              .append("div")
              .classed("vis-panel", true)
              .style("margin-left", "20px")
              .style("margin-bottom", "20px")
              .style("display", "none");

        panelSelection.append("h3")
            .text("Input Flows");
        flowTables[0] = LCA.createTable(panelSelection, flowColumns);

        panelSelection = parentSelection
              .append("div")
              .classed("vis-panel", true)
              .style("margin-bottom", "20px")
              .style("display", "none");
        panelSelection.append("h3")
            .text("Output Flows");
        flowTables[1] = LCA.createTable(panelSelection, flowColumns);
    }

    /**
     * Change event handler for process selection list.
     * Triggers LCIA computation update.
     */
    function onProcessChange() {
        selectedProcessID = this.options[this.selectedIndex].value;
        loadFlowProperties();
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
     * Display LCIA impact score with flow property name and reference unit
     * @param {number} impactScore  Total impact score 
     * @param {boolean} flowsExist  Indicates if there were any results 
     */
    function displayImpactScore(impactScore, flowsExist) {
        var chartGroup = svg.select(".chartgroup"),
            axisGroup = svg.select(".x.axis"),
            impactSelection = chartGroup.select("#impactscore"),
            fpSelection = axisGroup.select("#flowproperty"),
            flowProperty = "",
            referenceUnit = "",
            impactText = "";
                
        if (impactSelection.empty()) {
            impactSelection = chartGroup.append("text")
                .style("font-weight", "bold")
                .style("font-size", "12px")
                .attr({
                    id: "impactscore",
                    x: width + textPadding,
                    y: (barHeight / 2) + margin.top + textPadding
                });
        }
        if (fpSelection.empty()) {
            fpSelection = axisGroup.append("text")
                .style("font-weight", "bold")
                .attr({
                    id: "flowproperty",
                    x: width + textPadding,
                    y: textPadding
                });
        }
        if (flowsExist && ("lciaMethods" in LCA.indexedData)) {
            var method = LCA.indexedData.lciaMethods[selectedMethodID];
            if (method && ("referenceFlowProperty" in method)) {
                flowProperty = method.referenceFlowProperty.name;
                referenceUnit = method.referenceFlowProperty.referenceUnit;
                impactText = impactScore.toPrecision(4).toString();
            }
        }
        impactSelection.text(impactText + " " + referenceUnit);
        fpSelection.text(flowProperty);
    }

    /**
     * Create header for legend, if it does not already exist.
     * Make it invisible if there are no flows.
     *
     * @param {number} catX          X coordinate for flow category
     * @param {number} flowX          X coordinate for flow name
     * @param {number} resultX        X coordinate for result header
     * @param {number} headerY        Y coordinate for headers
     * @param {boolean} flowsExist    Flag if flows exist.
     */
    function makeLegendHeader(catX, flowX, resultX, headerY, flowsExist) {
        var legendGroup = svg.select(".legendgroup"),
            header = legendGroup.selectAll(".legendheader");
        if (header.empty() && flowsExist) {
            legendGroup.append("text").attr({
                class: "legendheader",
                x: catX,
                y: headerY
            })
                .text("Flow Category");
            legendGroup.append("text").attr({
                class: "legendheader",
                x: flowX,
                y: headerY
            })
                .text("Name");
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
            colPadding = textPadding,
            textY = 9,            
            colXs = [0, boxSize + colPadding, width - 275, width - 75],
            legend,
            newRows,
            squares,
            flows;

        makeLegendHeader(colXs[1], colXs[2], colXs[3], textY, flowData && flowData.length > 0);
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
            .attr("class", "category");
        newRows.append("text")
            .attr("x", colXs[2])
            .attr("y", textY)
            .attr("dy", ".35em")
            .attr("class", "flowname");
        newRows.append("text")
            .attr("x", colXs[3])
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
        legend.selectAll(".category")
            .text(function (d) {
                return (d.flowID in elementaryFlows) ?
                    elementaryFlows[d.flowID].category :
                    "";
            });
        flows = legend.selectAll(".flowname")
            .text(function (d) {
                return (d.flowID in elementaryFlows) ?
                    elementaryFlows[d.flowID].name :
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

 
        impactScore = 0;
        lciaResultData = results;
        svg.select("#chartheader").style("visibility", lciaResultData.length > 0 ? "visible" : "hidden");
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
            .attr("height", barHeight)
            .style("fill", function (d) {
                return color(d.flowID);
            });
        makeLegend(lciaResultData);
        displayImpactScore(impactScore, lciaResultData.length > 0);
        if (lciaResultData.length > 0) {
            resume("");
        } else {
            resume("No impact to display.");
        }
    }

    /**
     * Prepare process resources for selection list by appending property value to non-distinct
     * names.
     * @param {Array} processArray   Process resources from web API
     */
    function distinguishProcessNames(processArray) {
        if (processArray.length > 1) {
            var curName = processArray[0].name;
            for (var i = 1; i < processArray.length; ++i) {
                var curP = processArray[i],
                    prevP = processArray[i - 1];
                if (curP.name === curName) {
                    if (curP.geography !== prevP.geography ) {
                        curP.name = curP.name.concat(" [", curP.geography, "]");
                        if (prevP.name === curName) {
                            prevP.name = prevP.name.concat("  [", prevP.geography, "]");
                        }
                    } else if ( curP.version !== prevP.version) {
                        curP.name = curP.name.concat(" [", curP.version, "]");
                        if (prevP.name === curName) {
                            prevP.name = prevP.name.concat("  [", prevP.version, "]");
                        }
                    } else {
                        curP.name = curP.name.concat(" [id:", curP.processID, "]");
                        if (prevP.name === curName) {
                            prevP.name = prevP.name.concat("  [id:", prevP.processID, "]");
                        }
                    }
                } else {
                    curName = curP.name;
                }
            }
        }
    }

    /**
     * After processes have been loaded, prepare process selection list,
     * select first process (if none pre-selected),
     * request resources depending on process filter, 
     * and request LCIA results
     */
    function onProcessesLoaded() {
        if ("processes" in LCA.loadedData && LCA.loadedData.processes && 
            LCA.loadedData.processes.length > 0) {
            var processArray = LCA.loadedData.processes;
            processArray.sort(LCA.compareNames);            
            // Set selectedProcessID to startupProcessID,
            // if it exists. 
            if (processArray.some(function (p) {
                return p.processID === startupProcessID;
            })) {
                selectedProcessID = startupProcessID;
            } else {
                selectedProcessID = processArray[0].processID;
            }
            loadFlowProperties();
            distinguishProcessNames(processArray);
            LCA.loadSelectionList(processArray,
                    "#processSelect", "processID", onProcessChange, selectedProcessID);
            getLciaResults();
        } else {
            resume("Unable to load processes.");
        }
    }

    /**
     * After impact categories have been loaded, 
     * prepare selection list,
     * select first one,
     * request LCIA methods with selected impact category
     */
    function onImpactCategoriesLoaded() {
        if ("impactcategories" in LCA.loadedData && LCA.loadedData.impactcategories &&
            LCA.loadedData.impactcategories.length > 0) {
            selectedImpactCategoryID = LCA.loadedData.impactcategories[0].impactCategoryID;
            loadMethods();
            LCA.loadSelectionList(LCA.loadedData.impactcategories,
                    "#impactCategorySelect", "impactCategoryID", onImpactCategoryChange, selectedImpactCategoryID);
        } else {
            resume("Unable to load impact categories.");
        }
    }

    /**
     * After LCIA methods have been loaded, 
     * prepare selection list,
     * select first one (if none pre-selected),
     * request LCIA results
     */
    function onMethodsLoaded() {
        if ("lciamethods" in LCA.loadedData && LCA.loadedData.lciamethods) {
            if (LCA.loadedData.lciamethods.length > 0) {
                // If no default setting, select first LCIA method
                selectedMethodID = LCA.loadedData.lciamethods[0].lciaMethodID;
                LCA.loadSelectionList(LCA.loadedData.lciamethods,
                   "#lciaMethodSelect", "lciaMethodID", onMethodChange, selectedMethodID);
                LCA.indexedData.lciaMethods = LCA.indexData("lciamethods", "lciaMethodID");
                getLciaResults();
            } else {
                selectedMethodID = 0;
                LCA.emptySelectionList("#lciaMethodSelect");
                visualizeResults([]);
            }
        } else {
            resume("Unable to load LCIA methods.");
        }
    }

    function onFlowsLoaded() {      
        if ("processflows" in LCA.loadedData && LCA.loadedData.processflows) {
            LCA.loadedData.processflows.forEach(function (pf) {
                var rowObj;
                switch (pf.flow.flowTypeID) {
                    case 1:
                        rowObj = {
                            Name: pf.flow.name,
                            Magnitude: LCA.formatNumber(pf.result),
                            Unit: LCA.indexedData.flowProperties[pf.flow.referenceFlowPropertyID].referenceUnit
                        };
                        if (pf.directionID === 1) {
                            inputFlows.push(rowObj);
                        } else if (pf.directionID === 2 ) {
                            outputFlows.push(rowObj);
                        }
                        break;
                    case 2:
                        elementaryFlows[pf.flow.flowID] = pf.flow;
                        break;
                }
            });
            d3.selectAll(".vis-panel").style("display", "inline-block");
            LCA.updateTable(flowTables[0], inputFlows, flowColumns);
            LCA.updateTable(flowTables[1], outputFlows, flowColumns);
            getLciaResults();
        } else {
            d3.selectAll(".vis-panel").style("display", "none");
            resume("Unable to load process flows.");
        }
    }

    function onResultsLoaded() {
        if ("lciaresults" in LCA.loadedData) {
            visualizeResults(LCA.loadedData.lciaresults.lciaScore[0].lciaDetail);
        } 
    }

    /**
      * Get all processes with elementary flows from web API
      */
    function loadProcesses() {
        LCA.loadData("processes", false, onProcessesLoaded, "flowtypes/2");
    }

    function onFlowPropertiesLoaded() {
        if ("flowproperties" in LCA.loadedData) {
            LCA.indexedData.flowProperties = LCA.indexData("flowproperties", "flowPropertyID");
            loadFlows();
        } else {
            resume("Unable to load flow properties.");
        }
    }

    /**
      * Get all flows related to selected process
      */
    function loadFlows() {
        elementaryFlows = {};
        inputFlows = [];
        outputFlows = [];
        LCA.loadData("processflows", false, onFlowsLoaded, "processes/" + selectedProcessID);
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
        if ( selectedProcessID > 0 && selectedMethodID > 0 && "processflows" in LCA.loadedData) {
            wait("Compute LCIA...");
            LCA.loadData("lciaresults", false, onResultsLoaded,
            "processes/" + selectedProcessID + "/lciamethods/" + selectedMethodID);
        }
    }

    function wait(message) {
        d3.select("#impactScore").text("");
        svg.style("opacity", 0.1);
        msg.style("display", "block").text(message);
        d3.select("#processSelect").property("disabled", true);
        d3.select("#impactCategorySelect").property("disabled", true);
        d3.select("#lciaMethodSelect").property("disabled", true);
    }

    function resume(message) {       
        msg.text(message);
        svg.style("opacity", 1);
        d3.select("#processSelect").property("disabled", false);
        d3.select("#impactCategorySelect").property("disabled", false);
        d3.select("#lciaMethodSelect").property("disabled", false);
    }

    /**
     * Starting point for lciaComputation
     */
    function init() {

        prepareMsg();
        prepareFlowTables();
        prepareSvg();
        LCA.createSpinner("chartcontainer");
        wait("Load Data...");
        if ("processid" in LCA.urlVars) {
            startupProcessID = +LCA.urlVars.processid;
        }
        loadProcesses();
        loadImpactCategories();
    }

    LCA.init(init);
}
