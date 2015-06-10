/**
 * @ngdoc directive
 * @name lcaApp.sankey.directive:sankeyDiagram
 * @restrict E
 * @function
 * @scope
 *
 * @description
 * Sankey diagram directive.
 *
 * @param {object}  graph   Has nodes array and links array for SankeyService.
 * @param {object}  color   Has domain (array of node types) and range (array of fill colors).
 * @param {object}  selectedNode   Out - set to currently selected graph node.
 * @param {object}  mouseOverNode   Out - set to graph node where mouse is hovering.
 *
 */
angular.module('lcaApp.sankey.directive', ['d3.sankey.service', 'd3.tip'])
.directive('sankeyDiagram', ['SankeyService', 'TipService', function( SankeyService, TipService) {

    function link(scope, element, attrs) {

        var margin = {
                top: 10,
                right: 10,
                bottom: 10,
                left: 10
            },
            parentElement = element[0],
            width = parentElement.clientWidth - margin.left - margin.right,   // diagram width
            height = parentElement.clientHeight - margin.top - margin.bottom,  // diagram height
            sankeyWidth = width - 150, // leave room for labels on right
            svg,
            color = d3.scale.ordinal(),
            // TODO : make sankey link colors configurable
            linkColors = { positive : colorbrewer.Set2[8][6], negative : colorbrewer.Set2[8][7] },
            /**
             * sankey variables
             */
            sankey = SankeyService
                .nodeWidth(20)
                .nodePadding(20)
                .size([sankeyWidth, height]),
            graph = {},
            baseValue = 1E-14,  // sankey link base value (replaces 0).
            minNodeHeight = 3,  // Minimum height of sankey node/link
            opacity = { node: 1, link: 0.5 }; // default opacity settings

        /**
         * Initial preparation of svg element.
         */
        function prepareSvg() {

            svg = d3.select(parentElement)
                .append("svg")
                .attr("width", width + margin.left + margin.right)
                .attr("height", height + margin.top + margin.bottom)
                .on("mouseleave", restoreView)
                .append("g")
                .attr("transform", "translate(" + margin.left + "," + margin.top + ")");
        }

        /**
         * Get string for node label
         * @param n     A graph node
         * @return string for node label
         */
        function getNodeLabel(n) {
            return n.nodeName;
        }

        function onGraphChanged(newVal) {
            TipService.hide();
            if (newVal) {
                graph = scope.graph;
                drawSankey(graph["isNew"]);
            }
        }

        function restoreView() {
            svg.selectAll(".node")
                .transition()
                .style("opacity", opacity.node)
                .select(".node-label")
                .style("visibility", "visible");
            svg.selectAll(".link")
                .transition()
                .select("path")
                .style("stroke-opacity", opacity.link);
            svg.selectAll(".link-label")
                .style("visibility", "hidden");
            TipService.hide();
            scope.$apply(function(){
                scope.mouseOverNode = null;
            });
        }

        /**
        * Respond to click on node
        *
        * @param {Object}  node    Reference to graph node data
        */
        function onNodeClick(node) {
            d3.event.stopPropagation();
            TipService.hide();
            scope.$apply(function(){
                scope.selectedNode = node;
            });
        }

        /**
         * Respond to mouse over Sankey node
         * Fade other nodes and unconnected links
         *
         * @param {Object}  node    Reference to graph node data
         * @param {Number}  index   D3 data index
         */
        function onMouseOverNode(node, index) {
            var nodeSelection, linkSelection, linkLabels;

            d3.event.stopPropagation();
            nodeSelection = svg.selectAll(".node")
                .transition()
                .style("opacity", function (d, i) {
                    return i === index ? 0.9 : 0.2;
                });

            linkSelection = svg.selectAll(".link");
            linkSelection.transition()
                .select("path")
                .style("stroke-opacity", function (l) {
                    return (
                        (l.source.nodeID === node.nodeID || l.target.nodeID === node.nodeID) ?
                            opacity.link : 0.2);
                });

            TipService.show(node, index);
            nodeSelection.selectAll(".node-label").style("visibility", "hidden");

            linkLabels = linkSelection.select(".link-label")
                .style("visibility", "hidden")
                .filter( function(l) {
                    return l.hasOwnProperty("source") && l.hasOwnProperty("target") &&
                        l.source && l.target;
                });

            linkLabels.filter( function(l) {
                    return l.source.nodeID === node.nodeID;
                })
                .attr("x", function(l) {
                    return l.target.x - 6;
                })
                .attr("text-anchor", "end")
                .attr("y", function(l) {
                    return l.target.y + l.ty + l.dy / 2
                })
                .style("visibility", "visible");

            linkLabels.filter( function(l) {
                    return l.target.nodeID === node.nodeID;
                })
                .attr("x", function(l) {
                    return l.source.x + 6 + sankey.nodeWidth();
                })
                .attr("text-anchor", "start")
                .attr("y", function(l) {
                    return l.source.y + l.sy + l.dy / 2
                })
                .style("visibility", "visible");

            scope.$apply(function(){
                scope.mouseOverNode = node;
            });
        }

        /**
         * Handle node drag move
         * @param {{ x: number, y: number }} d Reference to graph node data

        function onDragMove(d) {
            d3.select(this).attr("transform",
                "translate(" + d.x + "," + (
                    d.y = Math.max(0, Math.min(height - d.dy, d3.event.y))
                ) + ")");
            sankey.relayout();
            svg.select("#linkGroup").selectAll(".link").attr("d", sankey.link());
        }
         */

        /**
         * Draw the sankey graph
         * @param {Boolean}  rebuild    Flag to indicate if this is a full redraw or an update
         */
        function drawSankey(rebuild) {

            var link, node,
                path = sankey.link(),
                transitionTime = 250;

            if (rebuild) {
                svg.selectAll("g").remove();
                svg.append("g").attr("id", "linkGroup");
                transitionTime = 0;
            }

            sankey.nodes(graph.nodes)
                .links(graph.links)
                .layout(20);

            link = svg.select("#linkGroup").selectAll(".link")
                .data(graph.links);
            if (rebuild) {
                link.enter().append("g")
                    .attr("class", "link");
                link.append("path").append("title");
            }
            link.select("path").transition().duration(transitionTime)
                .attr("d", path)
                .style("stroke-width", function (d) {
                    return Math.max(minNodeHeight, d.dy);
                })
                .style("stroke-dasharray", function (d) {
                    return (d.value === baseValue) ? "5,5" : null;
                })
                .style("stroke", function (d) {
                    return (d.hasOwnProperty("magnitude") && d.magnitude > 0) ? linkColors.positive : linkColors.negative;
                })
                .style("stroke-opacity", opacity.link)
                .sort(function (a, b) {
                    return b.dy - a.dy;
                })
            ;
            link.select("path").select("title")
                .text(function (d) {
                    return d["toolTip"];
                });


            if (rebuild) {
                svg.append("g").attr("id", "nodeGroup");
            }
            node = svg.select("#nodeGroup").selectAll(".node")
                .data(graph.nodes);
            if (rebuild) {
                node.enter().append("g")
                    .attr("class", "node");
                node.append("rect");
                node.append("text").attr("class", "node-label");
                link.append("text").attr("class", "link-label");
            }
            node.transition().duration(transitionTime)
                .attr("transform", function (d) {
                    return "translate(" + d.x + "," + d.y + ")";
                })
                .style("opacity", opacity.node);

            //node.call(d3.behavior.drag()
            //        .origin(function(d) { return d; })
            //        .on("dragstart", function() {
            //            this.parentNode.appendChild(this); })
            //        .on("drag", onDragMove));

            node.selectAll("rect")
                .transition().duration(transitionTime)
                .attr("height", function (n) {
                    return Math.max(minNodeHeight, n.dy);
                })
                .attr({
                    width: sankey.nodeWidth()
                })
                .style("fill", function (n) {
                    n.color = color(n[scope.color.property]);
                    return n.color;
                });

            node.on('mouseover', onMouseOverNode);

            //
            // Position fragment flow name to the right of node.
            //
            node.selectAll(".node-label")
                .transition().duration(transitionTime)
                .attr("y", function (d) {
                    return Math.max(minNodeHeight, d.dy) / 2;
                })
                .attr("dy", ".35em")
                .attr("transform", null)
                .text(function (d) {
                    return getNodeLabel(d);
                })
                .attr("x", 6 + sankey.nodeWidth())
                .attr("text-anchor", "start");
            link.select(".link-label")
                .style("visibility", "hidden")
                .attr("dy", ".35em")
                .attr("transform", null)
                .text(function (d) {
                    return d["toolTip"];
                });
            //
            // Nodes with click behavior
            //
            node.selectAll("rect")
                .filter(function (d) {
                    return (d.selectable);
                })
                //.style("cursor", "pointer")
                .on("click", onNodeClick)
                .style("stroke", function (n) {
                    return d3.rgb(n.color).darker(1);
                });

        }

        function prepareToolTip() {
            // Initialize tooltip plugin
            TipService
                .offset([-10, 0])
                .direction('n')
                .html(function (d) {
                    if ("toolTip" in d) {
                        return d["toolTip"];
                    }
                })
                .style("background", function (d) {
                    if (d && scope.color.property in d) {
                        return color(d[scope.color.property]);
                    }
                });
            svg.call(TipService);
            TipService.hide();
        }

        prepareSvg();
        color.domain(scope.color.domain);
        color.range(scope.color.range);
        scope.$watch('graph.links', onGraphChanged);
        prepareToolTip();

    }

    return {
        restrict: 'E',
        scope: { graph: '=', color: '=', selectedNode: '=selectedNode', mouseOverNode: '=mouseOverNode'},
        link: link
    }
}]);