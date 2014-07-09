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
        width = 700 - margin.left - margin.right,   // diagram width
        height = 600 - margin.top - margin.bottom;  // diagram height

    var formatNumber = d3.format("^.2g"),
        svg,
        color = d3.scale.ordinal();
    /**
     * sankey variables
     */
    var sankey = d3.sankey()
        .nodeWidth(30)
        .nodePadding(10)
        .size([width, height]),
        graph = {};
    var selectedFlowPropertyID = 23,
        fragFlowFlowProperties = [];

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
              return flowSet.has(d.id) ? Math.max(1, d.dy) : 1;
          })
          .style("stroke-dasharray", function (d) {
              return flowSet.has(d.id) ? "0,0" : "5,5";
          });
        console.debug("Updated links...");
        console.debug(links);
    }

    /**
     * Create or refresh sankey diagram with graph data
     */
    function updateSankey() {

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

        link.append("title")
            .text(function (d) {
                return d.name + " : " + formatNumber(d.value);
            });

        node = svg.append("g").selectAll(".node")
            .data(graph.nodes)
            .enter().append("g")
            .attr("class", "node")
            .attr("transform", function (d) {
                return "translate(" + d.x + "," + d.y + ")";
            });

        bars = node.append("rect")
            .attr("height", function (d) {
                return d.dy;
            })
            .attr({
                width: sankey.nodeWidth()
            })
            .style("fill", function (d) {
                d.color = color(d.type);
                return d.color;
            })
            .style("stroke", function (d) {
                return d3.rgb(d.color).darker(2);
            });
        node.append("text")
            .attr("x", -6)
            .attr("y", function (d) {
                return d.dy / 2;
            })
            .attr("dy", ".35em")
            .attr("text-anchor", "end")
            .attr("transform", null)
            .text(function (d) {
                return d.name;
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
            return (!flowSet.has(d.id));
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
            id: 0,
            name: "",
            type: 2
        });
        reverseIndex[0] = 0;
        // Add a node for every flow
        data.forEach(function (element) {
            var node = {
                id: element.NodeID,
                type: element.NodeTypeID,
                name: element.Name
            };
            reverseIndex[element.FragmentFlowID] = graph.nodes.push(node) - 1;
        });
        
        // Add a link for every flow. source and target are indexes into nodes array.
        data.forEach(function (element) {
            var link, parentIndex;
            ++nodeIndex;
            parentIndex = reverseIndex[element.ParentFragmentFlowID];
            link = {
                id: element.FlowID,
                name: element.Name,
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
        updateSankey();
    }

    /**
     * Callback function for data load.
     */
    function onDataLoaded() {
        if (("flowproperties" in LCA.loadedData) 
            && ("fragmentflows" in LCA.loadedData) 
            && ("flowflowproperties" in LCA.loadedData)) {
                // All requests have been executed
                
                if (LCA.loadedData.flowproperties !== null 
                    && LCA.loadedData.fragmentflows !== null
                    && LCA.loadedData.flowflowproperties !== null) {
                    // All requests executed successfully
                    prepareFragmentFlowPropertyList();
                    buildGraph(LCA.loadedData.fragmentflows);
                } 
        }
        if (LCA.spinner) {
            LCA.spinner.stop();
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
        updateSankey();
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
        color.domain([1, 2, 3, 4]); // NodeTypeIDs  TODO : get from web service
        prepareSvg();
        d3.select("#fragmentName").text(fragmentName);
        LCA.startSpinner("chartcontainer");
        LCA.loadData("flowproperties", true, onDataLoaded);
        LCA.loadData("fragmentflows", true, onDataLoaded);
        LCA.loadData("flowflowproperties", true, onDataLoaded);
    }

    LCA.init(init);
}