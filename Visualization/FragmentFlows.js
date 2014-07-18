/**
 * Fragment Flow visualization using sankey diagram
 */

/* Reference paths for Visual Studio Intellisense */
/// <reference path="LCA.js" />
/// <reference path="d3.min.js" />
/// <reference path="sankey.js" />
/// <reference path="d3-tip.js" />

function FragmentFlows() {

    // library globals - used to avoid jslint errors
    /*global d3, window, colorbrewer, LCA, console */

    // Current selections
    var selectedFragmentID = 2,
        fragmentName = "";

    // SVG margins
    var margin = {
        top: 10,
        right: 20,
        bottom: 30,
        left: 20
    },
        width = 700 - margin.left - margin.right,   // diagram width
        height = 600 - margin.top - margin.bottom;  // diagram height

    var formatNumber = d3.format("^.2g"),
        svg,
        color = d3.scale.ordinal();
    /**
     * sankey variables
     */
    var sankey = d3.sankey()
        .nodeWidth(20)
        .nodePadding(20)
        .size([width, height]),
        graph = {};
    var selectedFlowPropertyID = 23,
        fragFlowFlowProperties = [];
    var apiResourceNames = ["fragments", "processes", "flowproperties", "fragmentflows", "flowflowproperties"],
        nodeTypes = [],
        flowTables = [],
        flowColumns = ["Name", "CASNumber"],
        panelSelection;   

    

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
      * Initial preparation of svg element.
      */
      function prepareNodeView() {
          panelSelection = d3.select("#chartcontainer")
              .append("div")
              .classed("vis-panel", true);
        panelSelection.append("h2")
            .text("Node Details");
        panelSelection.append("p")
            .append("h3")
            .text("Input Flows");
        flowTables[0] = LCA.createTable(panelSelection, flowColumns);
        panelSelection.append("p")
            .append("h3")
            .text("Output Flows");
        flowTables[1] = LCA.createTable(panelSelection, flowColumns);
    }

    /**
     * Update link appearance in sankey diagram.
     * When the fragment flow does not have the selected flow property,
     * draw a thin dashed line.
     */
    function updateLinks() {
        // Set of FlowIDs related to current fragment and flow property 
        var flowSet = d3.set(fragFlowFlowProperties.filter(function (ffp) {
            return (ffp.FlowPropertyID === selectedFlowPropertyID);
        }).map(function (rf) {
            return rf.FlowID;
        }));

        var links = svg.selectAll(".link")
          .style("stroke-width", function (d) {
              return flowSet.has(d.flowID) ? Math.max(1, d.dy) : 1;
          })
          .style("stroke-dasharray", function (d) {
              return flowSet.has(d.flowID) ? "0,0" : "5,5";
          });
        console.debug("Updated links...");
        console.debug(links);
    }

    /**
     * Look up name of entity referenced by NodeID in fragment flow data
     * @param {Array}  ffData          fragment flow data
     * @return {String} the name
     */
    function getNodeName(ffData) {
        var nodeName = "";
        switch (ffData.NodeTypeID) {
            case 1:
                if (ffData.NodeID in LCA.indexedData.processes) {
                    nodeName = LCA.indexedData.processes[ffData.NodeID].Name;
                } else {
                    console.error("FragmentNode ProcessID: " + ffData.NodeID + " not found.");
                    nodeName = nodeTypes[ffData.NodeTypeID] + ffData.NodeID.toString();
                }
                break;
            case 2:
                if (ffData.NodeID in LCA.indexedData.fragments) {
                    nodeName = LCA.indexedData.fragments[ffData.NodeID].Name;
                } else {
                    console.error("FragmentNode FragmentID: " + ffData.NodeID + " not found.");
                    nodeName = nodeTypes[ffData.NodeTypeID] + ffData.NodeID.toString();
                }
                break;
        }
        return nodeName;
    }

    function displayFlows(node) {
        var inputFlows = [],
            outputFlows = [];

        node.targetLinks.forEach(function (l) {
            inputFlows.push(LCA.indexedData.flows[l.flowID]);
        });
        LCA.updateTable(flowTables[0], inputFlows, flowColumns);
        node.sourceLinks.forEach(function(l) {
            outputFlows.push(LCA.indexedData.flows[l.flowID]);
        });
        LCA.updateTable(flowTables[1], outputFlows, flowColumns);
    }

    /**
     * Draw the sankey graph
     */
    function drawSankey() {

        var link, node,
            path = sankey.link(),
            // Set of FlowIDs related to current fragment and flow property 
            flowSet = d3.set(fragFlowFlowProperties.filter(function (ffp) {
                return (ffp.FlowPropertyID === selectedFlowPropertyID);
            }).map(function (rf) {
                return rf.FlowID;
            }));
        // Initialize tooltip plugin
        var nodeTip = d3.tip()
          .attr('class', 'd3-tip')
          .direction('e')
          .html(function (d) {
              return "<strong>" + nodeTypes[d.nodeTypeID] + "</strong>" +
                 "<p>" + d.nodeName + "</p>";
          });
        svg.call(nodeTip);

        svg.selectAll("g").remove();
        sankey.nodes(graph.nodes)
            .links(graph.links)
            .layout(10);

        link = svg.append("g").selectAll(".link")
            .data(graph.links)
            .enter().append("path")
            .attr("class", "link")
            .attr("d", path)
            .style("stroke-width", function (d) {
                return Math.max(1, d.dy);
            })
            .sort(function (a, b) {
                return b.dy - a.dy;
            });
        // Tooltip for links <fragment flow name>
        link.append("title")
            .text(function (d) {
                return d.fragmentFlowName;
            });

        // Workaround for NaN problem
        graph.nodes.forEach(function (d) {
            if (isNaN(d.x)) {
                console.error("x attribute of node is NaN. Fragment Flow Name:  " + d.fragmentFlowName);
                d.x = 1;
            }
            if (isNaN(d.y)) {
                console.error("y attribute of node is NaN. Fragment Flow Name:  " + d.fragmentFlowName);
                d.y = 1;
            }
            if (isNaN(d.dy)) {
                console.error("dy attribute of node is NaN. Fragment Flow Name:  " + d.fragmentFlowName);
                d.dy = 1;
            }
        });

        node = svg.append("g").selectAll(".node")
            .data(graph.nodes)
            .enter().append("g")
            .attr("class", "node")
            .attr("transform", function (d) {
                return "translate(" + d.x + "," + d.y + ")";
            });

        node.append("rect")
            .attr("height", function (d) {
                return Math.max(1, d.dy);
            })
            .attr({
                width: sankey.nodeWidth()
            })
            .style("fill", function (d) {
                d.color = color(d.nodeTypeID);
                return d.color;
            })
            .style("stroke", function (d) {
                return d3.rgb(d.color).darker(2);
            });
        var ntElement = d3.select(".d3-tip");
        node.on('mouseover', function (d) {
            ntElement.transition()
                .duration(200);
            nodeTip.show(d);
            displayFlows(d);
        });
            //.on('mouseout', function (d) {
            //    ntElement.transition()
            //    .duration(500);
            //    nodeTip.hide(d);
            //});
        //
        // Position fragment flow name to the right or left of node.
        //
        node.append("text")
            .attr("x", -6)
            .attr("y", function (d) {
                return Math.max(1, d.dy) / 2;
            })
            .attr("dy", ".35em")
            .attr("text-anchor", "end")
            .attr("transform", null)
            .text(function (d) {
                return LCA.shortName(d.fragmentFlowName, 30);
            })
            .filter(function (d) {
                return d.x < width / 2;
            })
            .attr("x", 6 + sankey.nodeWidth())
            .attr("text-anchor", "start");

        //
        //  Change links for fragment flows not in flowSet
        //  - display as thin dashed line.
        //
        link.filter(function (d) {
            return ((d.nodeWeight <= 0) || (!flowSet.has(d.flowID)));
        })
        .style("stroke-width", 1)
        .style("stroke-dasharray", "5,5");

        displayFlows(graph.nodes[0]);
    }

    /**
     * Build sankey graph from fragment flow data
     * @param {Array}  data          fragment flow data
     */
    function buildGraph(data) {
        var nodeIndex = 0,
            reverseIndex = [], // map FragmentFlowID to graph.nodes and graph.links
            minVal = 1;

        graph.nodes = [];
        graph.links = [];

        // Create root node
        graph.nodes.push({
            nodeID: 0,
            fragmentFlowName: "",
            nodeTypeID: 0,
            nodeName: ""
        });
        reverseIndex[0] = 0;
        // Add a node for every flow
        data.forEach(function (element) {
            var node = {
                nodeID: element.NodeID,
                nodeTypeID: element.NodeTypeID,
                fragmentFlowName: element.Name,    // Fragment flow name
                nodeName: getNodeName(element) // Name of referenced object
            };
            reverseIndex[element.FragmentFlowID] = graph.nodes.push(node) - 1;
            if (element.NodeWeight > 0 && element.NodeWeight < minVal) {
                minVal = element.NodeWeight;
            }
        });

        // Add a link for every flow. source and target are indexes into nodes array.
        data.forEach(function (element) {
            var link, parentIndex;
            ++nodeIndex;
            parentIndex = reverseIndex[element.ParentFragmentFlowID];
            link = {
                flowID: element.FlowID,
                fragmentFlowName: element.Name,
                nodeWeight: element.NodeWeight,
                value: element.NodeWeight > 0 ? element.NodeWeight : minVal / 2   // sankey cannot handle 0 values
            };
            if (element.DirectionID === 1) {
                link.source = nodeIndex;
                link.target = parentIndex;
            } else {
                link.source = parentIndex;
                link.target = nodeIndex;
            }
            graph.links.push(link);

        });
        drawSankey();
    }

    function onFragmentsLoaded() {
        LCA.indexedData.fragments = LCA.indexData("fragments", "FragmentID");
        fragmentName = LCA.indexedData.fragments[selectedFragmentID].Name;
        d3.select("#fragmentName").text(fragmentName);
        onFragmentFlowsLoaded();
    }

    function onProcessesLoaded() {
        LCA.indexedData.processes = LCA.indexData("processes", "ProcessID");
        onDataLoaded();
    }

    /**
     * Callback function for data load.
     */
    function onDataLoaded() {
        if (apiResourceNames.every(function (n1) {
            return n1 in LCA.loadedData;
        })) {
            // All requests executed
            if (LCA.spinner) {
                LCA.spinner.stop();
            }
            if (apiResourceNames.every(function (n2) {
                return LCA.loadedData[n2] !== null;
            })) {
                // All requests executed successfully
                prepareFragmentFlowPropertyList();
                buildGraph( LCA.indexedData.fragments[selectedFragmentID].fragmentFlows);
            }
        }
    }

    /**
     * Change event handler for property type selection list.
     * Triggers sankey link update
     */
    function onPropertyTypeChange() {
        selectedFlowPropertyID = parseInt(this.options[this.selectedIndex].value);
        // IE does not display link style changes - need to recreate svg.
        // updateLinks();
        drawSankey();
    }

    /**
     * Populate flow property selection list with flow properties 
     * related to flows in fragment.
     */
    function prepareFragmentFlowPropertyList() {
        var fragmentFlows = LCA.indexedData.fragments[selectedFragmentID].fragmentFlows;
        fragFlowFlowProperties = LCA.loadedData.flowflowproperties.filter(function (ffp) {
            return fragmentFlows.some(function (ff) {
                return (ffp.FlowID === ff.FlowID);
            });
        });

        var fragFlowProperties = LCA.loadedData.flowproperties.filter(function (fp) {
            return fragFlowFlowProperties.some(function (ffp) {
                return (fp.FlowPropertyID === ffp.FlowPropertyID);
            });
        });
        LCA.loadSelectionList(fragFlowProperties, "#ptSelect", "FlowPropertyID", onPropertyTypeChange, selectedFlowPropertyID);
    }

    function onFragmentFlowsLoaded() {
        if ("fragments" in LCA.indexedData && apiResourceNames[3] in LCA.loadedData) {
            LCA.indexedData.fragments[selectedFragmentID].fragmentFlows = LCA.indexData(apiResourceNames[3], "FragmentFlowID");
            loadNodeWeights();
        }
    }

    /**
     * STOPGAP : load node weights from csv and update fragmentflows
     */
    function loadNodeWeights() {
        var fragmentFlows = LCA.indexedData.fragments[selectedFragmentID].fragmentFlows;
        d3.csv("TestData/NodeCache_FragmentID=1..2.csv")
            .row(function (d) { return { FragmentFlowID: +d.FragmentFlowID, NodeWeight: +d.NodeWeight }; })
            .get(function (error, rows) {
                if (error) {
                    console.error(error);
                } else {
                    rows.forEach(function(r) {
                        if (r.FragmentFlowID in fragmentFlows) {
                            var fragFlow = fragmentFlows[r.FragmentFlowID];
                            fragFlow.NodeWeight = r.NodeWeight < 0 ? 0 : r.NodeWeight;
                        }
                    });
                }
                onDataLoaded();
            });
    }

    /**
     * STOPGAP : load flows from csv
     */
    function loadFlows() {
        d3.csv("TestData/flows.csv")
        .row(function (d) { return { FlowID: +d.FlowID, Name: d.Name, CASNumber: d.CASNumber, FlowTypeID: +d.FlowTypeID, ReferenceFlowPropertyID: +d.ReferenceFlowPropertyID }; })
        .get(function (error, data) {
            if (error) {
                window.alert("Error loading flows.");
                console.error(error);
                LCA.loadedData.flows = null;
            } else {
                LCA.loadedData.flows = data;
            }
            LCA.indexedData.flows = LCA.indexData("flows", "FlowID");
            onDataLoaded();
        });
        
    }

    /**
     * Starting point for FragmentFlows
     */
    function init() {
        color.range(colorbrewer.Set3[5]);
        nodeTypes = LCA.enumData.nodeTypes;
        nodeTypes[0] = "Root Node";
        //color.domain(d3.keys(nodeTypes)); // NodeTypeIDs
        // Assign vibrant colors to processes and fragments
        color.domain([2,3,4,1,0]);
        prepareSvg();
        prepareNodeView();
        LCA.startSpinner("chartcontainer");
        //toolTip = LCA.createToolTip(".container");
        apiResourceNames = ["fragments", "processes", "flowproperties", "fragmentflows", "flowflowproperties", "flows"];
        LCA.loadData(apiResourceNames[0], true, onFragmentsLoaded);
        LCA.loadData(apiResourceNames[1], true, onProcessesLoaded);
        loadFlows();
        LCA.loadData(apiResourceNames[2], true, onDataLoaded);
        LCA.loadData(apiResourceNames[3], true, onFragmentFlowsLoaded, "." + selectedFragmentID);
        LCA.loadData(apiResourceNames[4], true, onDataLoaded);
    }

    LCA.init(init);
}