/**
 * D3 plugin for waterfall charts
 * Calculates size and position of chart components.
 * Developed to enable reusabilty.
 */
d3.waterfall = function () {
    /*global d3 */
    var waterfall = {},
        // Input
        width = 500,  // overall chart width
        segmentHeight = 24,
        segmentPadding = 8,
        scenarios = [],     // each scenario gets a chart
        stages = [],        // stages within each scenario 
        values = [],      // 2 dimensional array of values (first dimension: scenario, second dimension: stage)
        colors = [],        // Array of colors (one for every stage)
        // Output   
        segments = [],    // 2D array of waterfall segments, one for each non-null value
        xScale = d3.scale.linear(),             // d3 scale maps aggregate value to chart's x axis
        colorScale = d3.scale.ordinal(); // d3 scale maps stages to colors

    waterfall.colors = function (_) {
        if (!arguments.length) { return colors; }
        colors = _;
        return waterfall;
    };

    waterfall.width = function (_) {
        if (!arguments.length) { return width; }
        width = _;
        return waterfall;
    };

    waterfall.segmentHeight = function (_) {
        if (!arguments.length) { return segmentHeight; }
        segmentHeight = _;
        return waterfall;
    };

    waterfall.scenarios = function (_) {
        if (!arguments.length) { return scenarios; }
        scenarios = _;
        return waterfall;
    };

    waterfall.stages = function (_) {   
        if (!arguments.length) { return stages; }
        stages = _;
        return waterfall;
    };

    waterfall.values = function (_) {
        if (!arguments.length) { return values; }
        values = _;
        return waterfall;
    };

    function setGraphicAttributes(seg, index) {
        seg.x = xScale(Math.min(seg.startVal, seg.endVal));
        seg.y = (segmentPadding + segmentHeight) * index;
        seg.width = Math.abs(xScale(seg.value) - xScale(0));
        seg.color = colorScale(seg.stage);
    }

    waterfall.layout = function () {
        var i = 0, j = 0, minVal = 0.0, maxVal = 0.0;
        xScale.range([0, width]).nice();
        colorScale.range(colors);
        colorScale.domain(stages);
        segments = [];
        for (i = 0; i < scenarios.length; i++) {
            //first calculate the left edge for each stage
            var endVal = 0.0,
                scenarioStages = [];

            for (j = 0; j < stages.length; j++) {
                // NULL value means this stage is not relevant in this scenario,
                // no segment created
                if (values[i][j] !== null) {
                    var segment = {};
                    segment.scenario = scenarios[i];
                    segment.stage = stages[j];
                    segment.startVal = endVal;
                    segment.value = values[i][j];
                    segment.endVal = segment.startVal + segment.value;
                    endVal = segment.endVal;
                    scenarioStages.push(segment);
                    if (endVal < minVal) {
                        minVal = endVal;
                    } else if (endVal > maxVal) {
                        maxVal = endVal;
                    }
                }
            }
            segments.push(scenarioStages);
        }
        xScale.domain([minVal, maxVal]);       

        for (i = 0; i < scenarios.length; i++) {
            segments[i].forEach(setGraphicAttributes);
        }

        waterfall.xScale = xScale;  
        waterfall.segments = segments;  

        return waterfall;
    };

    return waterfall;
};