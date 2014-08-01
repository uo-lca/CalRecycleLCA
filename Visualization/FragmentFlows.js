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
    /*global d3, colorbrewer, LCA, console */

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
    var selectedFlowPropertyID = 23;
    var apiResourceNames = [],
        nodeTypes = [],
        flowTables = [],    // d3 selection of flow tables
        flowColumns = ["Name", "Magnitude", "Unit"],    // Flow table column names
        panelSelection,     // d3 selection of panel for node information
        nodeTip,            // tooltip for node, not currently used
        nodeTypeSelection,  // d3 selection of element to display node type
        nodeNameSelection,  // d3 selection of element to display fragment/process name
        baseValue = 1E-14,  // sankey link base value (replaces 0).
        curFragment = null; // reference to selected fragment in LCA.indexedData

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

    function prepareToolTip() {
        // Initialize tooltip plugin
        nodeTip = d3.tip()
          .attr('class', 'd3-tip')
          .direction('e')
          .html(function (d) {
              return "<strong>" + nodeTypes[d.nodeTypeID] + "</strong>" +
                 "<p>" + d.nodeName + "</p>";
          });
        svg.call(nodeTip);
    }

    /**
      * Initial preparation of svg element.
      */
    function prepareNodeView() {
        var parSelection;
        panelSelection = d3.select("#chartcontainer")
              .append("div")
              .classed("vis-panel", true)
              .style("display", "none");
        panelSelection.append("h2")
            .text("Node Details");

        parSelection = panelSelection.append("p");
        parSelection.append("label")
            .text("Node Type: ");
        nodeTypeSelection = parSelection.append("span");
    
        parSelection = panelSelection.append("p");
        parSelection.append("label")
            .text("Name: ");
        nodeNameSelection = parSelection.append("span");

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
     * Update flow table display
     * For each Sankey link provided, display
     * flow name, magnitude and unit associated with selected flow property.
     * If the link does have the selected flow property, display
     * magnitude and unit for the flow's reference flow property.
     *
     * @param {Array}  nodeLinks     Sankey links
     * @param {Object}  flowTable    D3 table selection
     */
    function updateFlowTable(nodeLinks, flowTable) {
        var flowData = [], flow;
        nodeLinks.forEach( function (l) {
            if ("flowID" in l) {
                var flowPropertyID = selectedFlowPropertyID,
                    magnitude = l.magnitude,
                    unit = "";
                flow = LCA.indexedData.flows[l.flowID];
                if (l.magnitude === null && "referenceFlowPropertyID" in flow) {
                    flowPropertyID = flow.referenceFlowPropertyID;
                    magnitude = getMagnitude(curFragment.links[l.fragmentFlowID], flowPropertyID);
                }
                if ("referenceUnitName" in LCA.indexedData.flowProperties[flowPropertyID]) {
                    unit = LCA.indexedData.flowProperties[flowPropertyID].referenceUnitName;
                }
                flowData.push({ Name: flow.name, Magnitude: magnitude, Unit: unit });
            }
        });
        LCA.updateTable(flowTable, flowData, flowColumns);
    }

    /**
     * Update flowTables with flows related to a graph node
     * @param {Object}  node    Reference to graph node
     */
    function displayFlows(node) {
        updateFlowTable(node.targetLinks, flowTables[0]);
        updateFlowTable(node.sourceLinks, flowTables[1]);
    }

    /**
     * Respond to mouse over Sankey node
     * Update panel with information related to node
     * Fade other nodes and unconnected links
     *
     * @param {Object}  node    Reference to graph node data
     * @param {Number}  index   D3 data index
     */
    function onMouseOverNode(node, index) {
        var nodeTypeName = "", nodeName = "";
        svg.selectAll(".node")
           .transition()
           .style("opacity", function (d, i) {
               return i === index ? 1 : 0.1;
           });
        if ( "nodeTypeID" in node) {
            nodeTypeName = nodeTypes[node.nodeTypeID];
        }
        svg.selectAll(".link")
            .transition()
           .style("stroke-opacity", function (l) {
               return (
               (l.source.fragmentFlowID === node.fragmentFlowID || l.target.fragmentFlowID === node.fragmentFlowID)
                ? 0.5 : 0.2);
           });
        if ("nodeTypeID" in node) {
            nodeTypeName = nodeTypes[node.nodeTypeID];
        }
        nodeTypeSelection.text(nodeTypeName);
        if ("nodeName" in node) {
            nodeName = node.nodeName;
        }
        nodeNameSelection.text(nodeName);

        displayFlows(node);
        panelSelection.style("display", "inline-block");
    }

    /**
     * Get magnitude of link with a flow property
     * @param {Object}  link              Fragment link
     * @param {Number}  flowPropertyID    flow property key
     * @return {Number} The magnitude, if link has the flow property. Otherwise, null.
     */
    function getMagnitude(link, flowPropertyID) {
        var magnitude = null;
        if ("linkMagnitudes" in link) {
            linkMagnitudes = link.linkMagnitudes.filter(function (lm) {
                return +lm.flowPropertyID === flowPropertyID;
            });
        }
        if (linkMagnitudes && linkMagnitudes.length > 0) {
            magnitude = linkMagnitudes[0].magnitude;
        }
        return magnitude;
    }

    /**
     * Draw the sankey graph
     */
    function drawSankey() {

        var link, node,
            path = sankey.link(),
            linkedNodes = [];

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
                return getFragmentFlowName(d);
            });
        linkedNodes = graph.nodes.filter(function (n) {
            return n.sourceLinks.length > 0 || n.targetLinks.length > 0;
        });
        node = svg.append("g").selectAll(".node")
            .data(linkedNodes)
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
        //var ntElement = d3.select(".d3-tip");
        node.on('mouseover', onMouseOverNode);
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
                return LCA.shortName(getFragmentFlowName(d), 30);
            })
            .filter(function (d) {
                return d.x < width / 2;
            })
            .attr("x", 6 + sankey.nodeWidth())
            .attr("text-anchor", "start");
        //
        //  Change links with 0 of less magnitude
        //  - display as thin dashed line.
        //
        link.filter(function (d) {
            return (d.value === baseValue );
        })
        //.style("stroke-width", 1)
        .style("stroke-dasharray", "5,5");

        //onMouseOverNode(linkedNodes[0], 0);
    }

    /**
     * Build sankey graph from fragment link data
     * @param {Array}  data          fragment link data
     */
    function buildGraph(data) {
        var nodeIndex = 0,
            reverseIndex = []; // map fragmentFlowID to graph.nodes and graph.links

        graph.nodes = [];
        graph.links = [];

        // Create root node
        //graph.nodes.push({
        //    fragmentFlowName: "",
        //    nodeTypeID: 0,
        //    nodeName: ""
        //});
        //reverseIndex[0] = 0;
        // Add a node for every flow
        data.forEach(function (element) {
            var node = {
                nodeTypeID: element.nodeTypeID,
                fragmentFlowID: element.fragmentFlowID,
                nodeName: "" // Name of referenced object, if any
            };
            if ("processID" in element) {
                node.processID = element.processID;
                if (node.processID in LCA.indexedData.processes) {
                    node.nodeName = LCA.indexedData.processes[node.processID].name;
                }
            }
            if ("subFragmentID" in element) {
                node.subFragmentID = element.subFragmentID;
                if (node.subFragmentID in LCA.indexedData.fragments) {
                    node.nodeName = LCA.indexedData.fragments[node.subFragmentID].name;
                }
            }
            reverseIndex[element.fragmentFlowID] = graph.nodes.push(node) - 1;
        });

        // Add a link for every flow. source and target are indexes into nodes array.
        data.forEach(function (element) {
            var link, parentIndex = 0;
            nodeIndex = reverseIndex[element.fragmentFlowID];
            if ("parentFragmentFlowID" in element) {
                var magnitude = getMagnitude(element, selectedFlowPropertyID),
                    value = (magnitude === null || magnitude <= 0) ? baseValue : baseValue + magnitude;
                link = {
                    flowID: element.flowID,
                    fragmentFlowID: element.fragmentFlowID,
                    magnitude: magnitude,
                    value: value
                };
                if (element.directionID === 1) {
                    link.source = nodeIndex;
                    link.target = parentIndex;
                } else {
                    link.source = parentIndex;
                    link.target = nodeIndex;
                }
                graph.links.push(link);
            }
        });
        drawSankey();
    }

    function onFragmentsLoaded() {
        LCA.indexedData.fragments = LCA.indexData("fragments", "fragmentID");
        curFragment = LCA.indexedData.fragments[selectedFragmentID];
        if (curFragment && "name" in curFragment) {
            d3.select("#fragmentName").text(curFragment.name);
        } else {
            window.alert("Fragment with ID = " + selectedFragmentID + " was not loaded.");
        }
        onFragmentLinksLoaded();
    }

    function onProcessesLoaded() {
        LCA.indexedData.processes = LCA.indexData("processes", "processID");
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
                if (curFragment) {
                    buildGraph( curFragment.links);
                }
            }
        }
    }

    function updateUnit() {
        var refUnitName = "";
        if (selectedFlowPropertyID in LCA.indexedData.flowProperties) {
            refUnitName = LCA.indexedData.flowProperties[selectedFlowPropertyID].referenceUnitName;
        }
        d3.select("#refUnitName").text(refUnitName);
    }

    /**
     * Change event handler for property type selection list.
     * Triggers sankey link update
     */
    function onPropertyTypeChange() {

        selectedFlowPropertyID = parseInt(this.options[this.selectedIndex].value);
        updateUnit();
        
        panelSelection.style("display", "none");

        // IE does not display link style changes - need to recreate svg.
        buildGraph(LCA.indexedData.fragments[selectedFragmentID].links);
    }

    /**
     * Populate flow property selection list with flow properties 
     * related to flows in fragment.
     */
    function onFlowPropertiesLoaded() {
        if ("flowproperties" in LCA.loadedData) {
            LCA.indexedData.flowProperties = LCA.indexData("flowproperties", "flowPropertyID");
            updateUnit();
            LCA.loadSelectionList(LCA.loadedData.flowproperties,
                "#ptSelect", "flowPropertyID", onPropertyTypeChange, selectedFlowPropertyID);
        }
        onDataLoaded();
    }

    /**
     * Invoked after links have been loaded and fragments have loaded and indexed.
     */
    function onFragmentLinksLoaded() {
        if ("fragments" in LCA.indexedData && apiResourceNames[3] in LCA.loadedData) {
            if (curFragment) {
                curFragment.links = LCA.indexData(apiResourceNames[3], "fragmentFlowID");
            }
            onDataLoaded();
        }
    }

    function onFlowsLoaded() {
        LCA.indexedData.flows = LCA.indexData("flows", "flowID");
        onDataLoaded();
    }

    /**
     * Get name of fragment flow associated with current Sankey node or link
     * @param {Object} data     d3 data at current node or link
     * @return the name
     */
    function getFragmentFlowName(data) {
        var name = "";
        if ("fragmentFlowID" in data) {
            if ("name" in curFragment.links[data.fragmentFlowID]) {
                name = curFragment.links[data.fragmentFlowID].name;
            }
        }
        return name;
    }

    /**
     * Starting point for FragmentFlows
     */
    function init() {
        var webApiFilter;
        color.range(colorbrewer.Set3[5]);
        nodeTypes = LCA.enumData.nodeTypes;
        nodeTypes[0] = "Root Node";
        //color.domain(d3.keys(nodeTypes)); // NodeTypeIDs
        // Assign vibrant colors to processes and fragments
        color.domain([2, 3, 4, 1, 0]);
        if ("fragmentid" in LCA.urlVars) {
            selectedFragmentID = +LCA.urlVars.fragmentid;
        }
        webApiFilter = "fragments/" + selectedFragmentID;
        prepareSvg();
        prepareToolTip();
        prepareNodeView();
        LCA.startSpinner("chartcontainer");
        //toolTip = LCA.createToolTip(".container");
        apiResourceNames = ["fragments", "processes", "flowproperties", "links", "flows"];
        LCA.loadData(apiResourceNames[0], false, onFragmentsLoaded);
        LCA.loadData(apiResourceNames[1], false, onProcessesLoaded);
        LCA.loadData(apiResourceNames[2], false, onFlowPropertiesLoaded, webApiFilter);
        LCA.loadData(apiResourceNames[3], false, onFragmentLinksLoaded, webApiFilter);
        LCA.loadData(apiResourceNames[4], false, onFlowsLoaded, webApiFilter);
    }

    LCA.init(init);
}