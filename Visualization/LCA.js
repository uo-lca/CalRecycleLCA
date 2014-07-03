/**
 * Shared module for Used Oil LCA Visualization
 */
// library globals
/*global d3, window, console, Spinner */

var LCA = {
    baseURI: "http://publictest.calrecycle.ca.gov/lciatool/api/",
    testDataFolder: "TestData/",
    loadedData: [],
    spinner: null
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

LCA.startSpinner = function startSpinner(tgtElementId) {
    var opts = {
        lines: 6, // The number of lines to draw
        length: 3, // The length of each line
        width: 3, // The line thickness
        radius: 10, // The radius of the inner circle
        corners: 1, // Corner roundness (0..1)
        rotate: 0, // The rotation offset
        direction: 1, // 1: clockwise, -1: counterclockwise
        color: '#000', // #rgb or #rrggbb or array of colors
        speed: 1, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: false, // Whether to render a shadow
        hwaccel: false, // Whether to use hardware acceleration
        className: 'spinner', // The CSS class to assign to the spinner
        zIndex: 2e9, // The z-index (defaults to 2000000000)
        top: 'auto', // Top position relative to parent in px
        left: 'auto' // Left position relative to parent in px
    },
    target = window.document.getElementById(tgtElementId);
    LCA.spinner = new Spinner(opts).spin(target);
};

/**
 * GET resource data and save results
 * @param {String} resourceName     web service resource name
 * @param {Boolean} useTestData     Load json file for testing
 * @param {Function} callback       Function to call when done
 */
LCA.loadData = function (resourceName, useTestData, callback) {
   
    if (resourceName in LCA.loadedData) {
        callback.call();
        return false;
    }
    
    var jsonURL = (useTestData ? LCA.testDataFolder : LCA.baseURI) + resourceName;
    if (useTestData) {
        jsonURL += ".json";
    }
    d3.json(jsonURL, function (error, jsonData) {
        if (error) {
            window.alert("Error executing GET on " + jsonURL);
            console.error(error);
            LCA.loadedData[resourceName] = null;
        } else {
            LCA.loadedData[resourceName] = jsonData;
        }
        callback.call();
    });
    return true;
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