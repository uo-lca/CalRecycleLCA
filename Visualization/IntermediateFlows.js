/**
 * Process Flow visualization using sankey diagram
 */
function processFlow() {


    // library globals
    /*global d3, console, window, $, colorbrewer, LCA */

    /**
     * lciaComputation variables
     */
    var // Data loaded from web API
    processList = [],
        intFlowList = [],
        // Web API methods
        baseURI = "http://rachelscanlon.com/api/",
        processesURL = baseURI + "process",
        intFlowURL = "IntermediateFlows.json",
        // Current selections
        selectedProcessID = 3,
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
        width = 600 - margin.left - margin.right,
        height = 500 - margin.top - margin.bottom;

    var color = d3.scale.ordinal();

    var x = d3.scale.linear()
        .rangeRound([0, width]);

    var labelFormat = d3.format("^.2g"); // Format numbers with precision 2;

    var svg;
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

        var link, node, bars, sliders,
            path = sankey.link();

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
                return d.source.name + " → " + d.target.name + "\n" + d3.format(d.value);
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
                //                d.color = color(d.name.replace(/ .*/, ""));
                d.color = colorbrewer.BuGn[3][1];
                return d.color;
            })
            .style("stroke", function (d) {
                return d3.rgb(d.color).darker(2);
            });
        bars.append("title")
            .text(function (d) {
                return d.name + "\n" + d3.format(d.value);
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

    }

    /**
     * Callback function for IntermediateFlows.
     * @param {String} error    Web API GET error
     * @param {Array} data      JSON data from web API
     */
    function buildGraph(error, data) {
        var process;	// Current process

        if (error) {
            window.alert("Error loading intermediate flows: " + error);
            return false;
        }

		graph.nodes = [];
        graph.links = [];

        graph.nodes.push({name: processName});
        data.forEach( function (element, index) {
            var node, link;

            node = {name: element.FlowName};
            graph.nodes.push(node);
//            link = {value: +element.ProcessFlowResult};
            link = {value: 1};
            if (element.FlowDirection == "Input") {
                link.source = index+1;
                link.target = 0;
            }
            else {
                link.source = 0;
                link.target = index+1;
            }
            graph.links.push(link);
        });
        updateSankey();
    }


    /**
     * Display intermediate product flows for selected process.
     */
    function displayResults() {
        d3.json(intFlowURL, buildGraph);
    }

    /**
     * Change event handler for process selection list.
     * Triggers LCIA computation update.
     */
    function onProcessChange() {
        var paramURL;

        selectedProcessID = this.options[this.selectedIndex].value;
        processName = this.options[this.selectedIndex].text;
        displayResults();
    }


    /**
     * Starting point for IntermediateFlows
     */
    function init() {

        LCA.prepareSelect(processesURL, "#processSelect", "ProcessID",
            onProcessChange, selectedProcessID);

        prepareSvg();
        displayResults();
    }

    init();
}
