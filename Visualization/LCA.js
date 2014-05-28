/**
 * Shared module for Used Oil LCA Visualization
 */
// library globals
/*global d3, window, console */

var LCA = {
    baseURI: "http://kbcalr.isber.ucsb.edu/api/"
};

/**
 * Initialize LCA module object
 * @param {function} callback	Function to call after module is sucessfully initialized
 */
LCA.init = function (callback) {
    // Load configurable settings
    // IIS won't allow this. Hard code setting above.
    //d3.json("Settings.json", function (error, settings) {
    //    if (error) {
    //        window.alert("Error loading Settings.json.");
    //        console.error(error);
    //    } else {
    //        LCA.baseURI = settings.WebAPI.BaseURI;
    //        callback.call();
    //    }
    //});
    callback.call();
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
            window.alert("Error executing GET on " + jsonURL);
            console.error(error);
            return false;
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
        selectOptions.filter(function (d) {
            return d[oidName] === initialValue;
        })
            .attr("selected", true);
    });

};