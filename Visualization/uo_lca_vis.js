// library globals
/*global d3, console, window, $, colorbrewer */

var LCA = {
    version: "1.1"
};

/**
 * Compare function used to sort array of objects by Name.
 */
LCA.compareNames = function (a, b) {
    return d3.ascending(a.Name, b.Name);
};

/**
 * Prepare select element. Load with data and initialize selection
 * @param {String} jsonURL          URL for JSON data (web API endpoint or file)
 * @param {String} selectID         SELECT HTML element id
 * @param {String} oidName          Property name of object ID field.
 * @param {function} changeHandler  Function for handling selection update.
 * @param {Number} initialValue     Default value (selected object ID).
 */
LCA.prepareSelect = function (jsonURL, selectID, oidName, changeHandler, initialValue) {

    d3.json(jsonURL, function (error, jsonData) {
        var selectOptions;

        if (error) {
            window.alert(error);
        }
        jsonData.sort(LCA.compareNames);

        selectOptions = d3.select(selectID)
            .on("change", changeHandler)
            .selectAll("option")
            .data(jsonData)
            .enter()
            .append("option")
            .attr("value", function (d) {
                return d[oidName];
            })
            .text(function (d) {
                return d.Name;
            });
        //
        // Initialize selection
        //
        selectOptions.filter(function (d, i) {
            return d[oidName] == initialValue;
        })
            .attr("selected", true);
    });

};