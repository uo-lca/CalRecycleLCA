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
        selectedImpactCategoryID = 0,
        selectedMethodID = 0,
        unit = "kg CO2-Equiv.",
        curFragment = null;

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
    var legendSvg;

    /**
     * Initial preparation of svg element.
     */
    function prepareSvg() {
        legendSvg = d3.select("#legend")
                    .append("svg")
                    .attr("width", 150 )
                    .attr("height", 150)
                    .append("g")
                    .attr("transform", "translate(" + 5 + "," + 5 + ")");
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

    function visualizeResults() {
        if ( "lciaMethods" in LCA.indexedData) {
            var method = LCA.indexedData.lciaMethods[selectedMethodID];
            if (method && ("referenceFlowProperty" in method)) {
                flowProperty = method.referenceFlowProperty.name;
                referenceUnit = method.referenceFlowProperty.referenceUnit;
                d3.select("#unitName").text(referenceUnit);
                //d3.select("#fpName").text(flowProperty);
            }
        }
    }

    /**
     * Display LCIA results
     */
    function displayResults() {
        var bottom = 0;

        
        waterfall.layout();
        LCA.makeLegend(legendSvg, waterfall.stages(), waterfall.colorScale);
        xAxis.scale(waterfall.xScale);
        waterfallData.scenarios.forEach(function (scenario, index) {
            bottom = displayScenario(scenario, index, bottom);
        });
    }

    function wait(message) {
        console.log(message);
    }

    function resume(message) {
        console.log(message);
    }

    //TODO: share common code with LciaComputation

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

    function onFragmentLoaded() {
        var resourceName = "fragments/" + selectedFragmentID;
        if (resourceName in LCA.loadedData && LCA.loadedData) {
            curFragment = LCA.loadedData[resourceName];
            d3.select("#fragmentName").text(curFragment.name);
        }
    }

    function onResultsLoaded() {
        if ("lciaresults" in LCA.loadedData) {
            visualizeResults(LCA.loadedData.lciaresults.fragmentFlowLCIAResults);
        } 
    }

    /**
      * If a method has been selected,
      * get LCIA computation results from web API
      */
    function getLciaResults() {
        if ( selectedMethodID > 0 ) {
            wait("Get LCIA results...");
            LCA.loadData("lciaresults", false, onResultsLoaded,
            "fragments/" + selectedFragmentID + "/lciamethods/" + selectedMethodID);
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
    * Load fragment
    */
    function loadFragment() {
        LCA.loadData("fragments/" + selectedFragmentID, false, onFragmentLoaded);
    }

    /**
     * Starting point for lciaComputation
     */
    function init() {
        
        LCA.createSpinner("chartcontainer");
        prepareSvg();
        if ("fragmentid" in LCA.urlVars) {
            selectedFragmentID = +LCA.urlVars.fragmentid;
        }
        loadImpactCategories();
        loadFragment();
    }

    LCA.init(init);
}