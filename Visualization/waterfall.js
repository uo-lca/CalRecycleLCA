/**
 * D3 plugin for waterfall charts
 * Calculates size and position of chart components.
 * Developed to enable reusabilty.
 */
d3.waterfall = function () {
    var waterfall = {},
        size = [500, 200],  // chart width, height
        scenarios = [],     // each scenario gets a chart
        stages = [],        // stages within each scenario 
        values = [[]];      // 2 dimensional array of values (for each scenario, then for each stage).


    waterfall.size = function (_) {
        if (!arguments.length) return size;
        size = _;
        return waterfall;
    };

    waterfall.scenarios = function (_) {
        if (!arguments.length) return scenarios;
        scenarios = _;
        return waterfall;
    };

    waterfall.stages = function (_) {
        if (!arguments.length) return stages;
        stages = _;
        return waterfall;
    };

    waterfall.values = function (_) {
        if (!arguments.length) return values;
        values = _;
        return waterfall;
    };

    waterfall.layout = function () {
        var i = 0, j = 0, minVal = 0.0, maxVal = 0.0;
        waterfall.xScale = d3.scale.linear().range([0, size[0]]).nice();
        waterfall.yScale = d3.scale.ordinal().rangeRoundBands([0, size[1]], 0.1);
        waterfall.segments = [[]];
        for (i = 0; i < scenarios.length; i++) {
            //first calculate the left edge for each stage
            var endVal = 0.0,
                scenarioStages = [];

            for (j = 0; j < stages.length; j++) {
                segment = {};
                segment.scenario = scenarios[i]
                segment.stage = stages[j];
                segment.startVal = endVal;
                segment.value = values[i][j];
                segment.endVal = segment.startVal + segment.value;
                segment.xVal = segment.startVal < segment.endVal ? segment.startVal : segment.endVal;
                segment.width = Math.abs(segment.value);
                endVal = segment.endVal;
                scenarioStages.push(segment);
                if (endVal < minVal) {
                    minVal = endVal;
                } else if (endVal > maxVal) {
                    maxVal = endVal;
                }
            }
            waterfall.segments.push(scenarioStages);
        }

        return waterfall;
    };

    return waterfall;
};