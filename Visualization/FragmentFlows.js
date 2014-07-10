/**
 * Fragment Flow visualization using sankey diagram
 */

function FragmentFlows() {

    // library globals
    /*global d3, window, colorbrewer, LCA, console */

    // Current selections
    var selectedFragmentID = 2,
        fragmentName = "Natural Gas Supply Mixer";
    
    // SVG margins
    var margin = {
        top: 10,
        right: 20,
        bottom: 30,
        left: 20
    },
        width = 800 - margin.left - margin.right,   // diagram width
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
    var apiResourceNames = ["fragments", "process", "flowproperties", "fragmentflows", "flowflowproperties"];

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
        var nodeName = LCA.enumData.nodeTypes[ffData.NodeTypeID];
        switch(+ffData.NodeTypeID) {
            case 1:
                if (ffData.NodeID in LCA.indexedData.process) {
                    nodeName = LCA.indexedData.process[ffData.NodeID].Name;
                }
                else {
                    console.error("FragmentNode ProcessID: " + ffData.NodeID + " not found.");
                    nodeName += ffData.NodeID;
                }
                break;
            case 2:
                if (ffData.NodeID in LCA.indexedData.fragments) {
                    nodeName = LCA.indexedData.fragments[ffData.NodeID].Name;
                }
                else {
                    console.error("FragmentNode FragmentID: " + ffData.NodeID + " not found.");
                    nodeName +=  ffData.NodeID;
                }
                break;
        }
        return nodeName;
    }

    /**
     * Draw the sankey graph
     */
    function drawSankey() {

        var link, node, bars,
            path = sankey.link(),
            // Set of FlowIDs related to current fragment and flow property 
            flowSet = d3.set(fragFlowFlowProperties.filter(function (ffp) {
                return (ffp.FlowPropertyID === selectedFlowPropertyID);
            }).map(function (rf) {
                return rf.FlowID;
            }));

        svg.selectAll("g").remove();
        sankey.nodes(graph.nodes)
            .links(graph.links)
            .layout(32);

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
        // Tooltip for links <fragment flow name> : <node weight>
        link.append("title")
            .text(function (d) {
                return d.fragmentFlowName + " : " + formatNumber(d.value);
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
                return d.dy;
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
        //
        // Node tooltip
        //
        node.append("title")
            .text(function (d) {
                return d.nodeName;
            });
        //
        // Position fragment flow name to the right or left of node.
        //
        node.append("text")
            .attr("x", -6)
            .attr("y", function (d) {
                return d.dy / 2;
            })
            .attr("dy", ".35em")
            .attr("text-anchor", "end")
            .attr("transform", null)
            .text(function (d) {
                return d.fragmentFlowName;
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
        link.filter(function(d) {
            return (!flowSet.has(d.flowID));
        })
        .style("stroke-width", 1)
        .style("stroke-dasharray", "5,5");

   
    }

    /**
     * Build sankey graph from fragment flow data
     * @param {Array}  data          fragment flow data
     */
    function buildGraph(data) {
        var nodeIndex = 0,
            reverseIndex = []; // map FragmentFlowID to graph.nodes and graph.links

        graph.nodes = [];
        graph.links = [];

        // Create root node
        graph.nodes.push({
            nodeID: 0,
            fragmentFlowName: "",
            nodeTypeID: 2,
            nodeName: "root"
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
        });
        
        // Add a link for every flow. source and target are indexes into nodes array.
        data.forEach(function (element) {
            var link, parentIndex;
            ++nodeIndex;
            parentIndex = reverseIndex[element.ParentFragmentFlowID];
            link = {
                flowID: element.FlowID,
                fragmentFlowName: element.Name,
                value: Math.abs(element.Quantity)   // TODO : replace with NodeWeight when available
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
        LCA.indexData("fragments", "FragmentID");
        onDataLoaded();
    }

    function onProcessesLoaded() {
        LCA.indexData("process", "ProcessID");
        onDataLoaded();
    }

    /**
     * Callback function for data load.
     */
    function onDataLoaded() {
        if (apiResourceNames.every( function (n1){
            return n1 in LCA.loadedData;
        })) {
            // All requests executed
            if (LCA.spinner) {
                LCA.spinner.stop();
            }
            if (apiResourceNames.every( function (n2) {
                return LCA.loadedData[n2] !== null;
            })) {
                // All requests executed successfully
                prepareFragmentFlowPropertyList();
                buildGraph(LCA.loadedData.fragmentflows);
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
        fragFlowFlowProperties = LCA.loadedData.flowflowproperties.filter(function (ffp) {
            return LCA.loadedData.fragmentflows.some(function (ff) {
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

    /**
     * Starting point for FragmentFlows
     */
    function init() {
        color.range(colorbrewer.Set3[4]);
        color.domain(d3.keys(LCA.enumData.nodeTypes)); // NodeTypeIDs
        prepareSvg();
        d3.select("#fragmentName").text(fragmentName);
        LCA.startSpinner("chartcontainer");
        LCA.loadData(apiResourceNames[0], true, onFragmentsLoaded);
        LCA.loadData(apiResourceNames[1], false, onProcessesLoaded);
        LCA.loadData(apiResourceNames[2], true, onDataLoaded);
        LCA.loadData(apiResourceNames[3], true, onDataLoaded);
        LCA.loadData(apiResourceNames[4], true, onDataLoaded);

    }

    LCA.init(init);
}