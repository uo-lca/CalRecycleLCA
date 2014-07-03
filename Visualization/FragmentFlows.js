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
        .nodeWidth(15)
        .nodePadding(10)
        .size([width, height]),
        graph = {};

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
     * Create or refresh sankey diagram with graph data
     */

    function updateSankey() {

        var link, node, bars,
            path = sankey.link();

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
            })
            .call(d3.behavior.drag()
                .origin(function (d) {
                    return d;
                })
                .on("dragstart", function () {
                    this.parentNode.appendChild(this);
                })
                .on("drag", function dragmove(d) {
                    d3.select(this).attr("transform", "translate(" + d.x + "," + (d.y = Math.max(0, Math.min(height - d.dy, d3.event.y))) + ")");
                    sankey.relayout();
                    link.attr("d", path);
                }));

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

        //node.append("text")
        //    .attr("x", -6)
        //    .attr("y", function (d) {
        //        return d.dy / 2;
        //    })
        //    .attr("dy", ".35em")
        //    .attr("text-anchor", "end")
        //    .attr("transform", null)
        //    .text(function (d) {
        //        return d.label;
        //    })
        //    .filter(function (d) {
        //        return d.x < width / 2;
        //    })
        //    .attr("x", 6 + sankey.nodeWidth())
        //    .attr("text-anchor", "start");

    }

    /**
     * Build graph from fragment flow data
     */
    function buildGraph(data) {
        var positiveFlows = data.filter(function (f) {
            return (f.Quantity > 0);
        });
        var nodeIndex = 0;
        graph.nodes = [];
        graph.links = [];
        // Create root node
        graph.nodes.push({
            id: 0,
            type: 2
        });
        // Add a node for every flow
        positiveFlows.forEach(function (element) {
            var node = {
                id: element.FragmentFlowID,
                type: element.NodeTypeID
            };
            graph.nodes.push(node);
        });
        // Add a link for every flow. source and target are indexes into nodes array.
        positiveFlows.forEach(function (element) {
            var link, parentIndex;
            ++nodeIndex;
            for (parentIndex = 0;
                parentIndex < graph.nodes.length && (element.ParentFragmentFlowID !== graph.nodes[parentIndex].id) ;
                parentIndex++) {
            }
            link = {
                name: element.Name,
                value: element.Quantity,
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

    ///**
    // * Change event handler for process selection list.
    // * Triggers LCIA computation update.
    // */
    //function onProcessChange() {
    //    selectedProcessID = this.options[this.selectedIndex].value;
    //    processName = this.options[this.selectedIndex].text;
    //    displayResults();
    //}
    /**
     * Callback function for data load.
     */
    function onDataLoaded() {
        if (("flowproperties" in LCA.loadedData) && ("fragmentflows" in LCA.loadedData)) {
            // All requests have been executed
            if (LCA.spinner) {
                LCA.spinner.stop();
            }
            if (LCA.loadedData.flowproperties !== null && LCA.loadedData.fragmentflows !== null) {
                // All requests executed successfully
                buildGraph(LCA.loadedData.fragmentflows);
            } 
        }
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
        //LCA.prepareSelect(processesURL, "#ptSelect", "ProcessID",
        //    onProcessChange, selectedProcessID);
    }

    LCA.init(init);
}