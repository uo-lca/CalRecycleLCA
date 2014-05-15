/**
 * Process Flow visualization using sankey diagram
 */
function processFlow() {

    // library globals
    /*global d3, window, colorbrewer, LCA, console */

    /**
     * lciaComputation variables
     */
    // Current selections
    var selectedProcessID = 3,
        processName = "CA Waste Code 222_2010";
    /**
     * d3 variables
     */
    var margin = {
        top: 10,
        right: 20,
        bottom: 30,
        left: 20
    },
        width = 700 - margin.left - margin.right,
        height = 600 - margin.top - margin.bottom;

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
        color.domain(graph.nodes.map(function (n) {
            return n.property;
        }));

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
                return d.source.name + " → " + d.target.name + "\n" + formatNumber(d.displayValue) + " " + d.property;
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
                d.color = color(d.property);
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
                return d.label;
            })
            .filter(function (d) {
                return d.x < width / 2;
            })
            .attr("x", 6 + sankey.nodeWidth())
            .attr("text-anchor", "start");

    }

    /**
     * Callback function for IntermediateFlows.
     * @param {String} error    Web API GET error
     * @param {Array} data      JSON data from web API
     */
    function buildGraph(error, data) {
        var nodeCount = 0;

        if (error) {
            window.alert("Error getting intermediate flows.");
            console.error();
            return false;
        }

        graph.nodes = [];
        graph.links = [];

        graph.nodes.push({
            name: processName,
            property: processName,
            label: ""
        });
        data.forEach(function (element) {
            var node, link;

            if (element.SankeyWidth > 0) {
                node = {
                    name: element.FlowName,
                    property: element.ReferenceProperty,
                    label: element.FlowName + " : " + formatNumber(element.Quantity) + " " + element.ReferenceUnit
                };
                nodeCount = graph.nodes.push(node);
                link = {
                    value: element.SankeyWidth,
                    displayValue: element.Quantity,
                    property: element.ReferenceProperty
                };
                if (element.FlowDirection === "Input") {
                    link.source = nodeCount - 1;
                    link.target = 0;
                } else {
                    link.source = 0;
                    link.target = nodeCount - 1;
                }
                graph.links.push(link);
            }
        });
        updateSankey();
    }

    /**
     * Display intermediate product flows for selected process.
     */
    function displayResults() {
        var intFlowURL = LCA.baseURI + "intermediateflow?balance=0",
            paramURL = intFlowURL + "&processId=" + selectedProcessID;
        d3.json(paramURL, buildGraph);
    }

    /**
     * Change event handler for process selection list.
     * Triggers LCIA computation update.
     */
    function onProcessChange() {
        selectedProcessID = this.options[this.selectedIndex].value;
        processName = this.options[this.selectedIndex].text;
        displayResults();
    }

    /**
     * Starting point for IntermediateFlows
     */
    function init() {
        var processesURL;

        processesURL = LCA.baseURI + "process";

        color.range(colorbrewer.Set3[12]);

        LCA.prepareSelect(processesURL, "#processSelect", "ProcessID",
            onProcessChange, selectedProcessID);

        prepareSvg();
        displayResults();
    }

    LCA.init(init);
}