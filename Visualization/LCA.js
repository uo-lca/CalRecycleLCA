/**
 * Shared module for Used Oil LCA Visualization
 */
// global directives to JSLint
/*global d3, window, console, Spinner */
// reference directives to Visual Studio Intellisense
/// <reference path="d3.min.js" />
/// <reference path="spin.min.js" />
var LCA = {
    baseURI: "http://publictest.calrecycle.ca.gov/lciatool/api/",
    testDataFolder: "TestData/",
    loadedData: [],  // Data loaded via web API (or from TestData)
    spinner: null,
    indexedData: [], // Associative arrays of loaded data, ID -> data object
    enumData: []     // Associative arrays of enum data, ID -> name
};

/**
 * Create an associative array for enumerated data type (JavaScript has no enum type)
 * Key for first value is 1 and is incremented by 1 for each of the subsequent values.
 * @param {Array} values	Array of string values
 * @return {Array} the associative array.
 */
LCA.createEnumData = function (values) {
    var enumData = [];
    for (var i = 0; i < values.length; ++i) {
        enumData[i + 1] = values[i];
    }
    return enumData;
};

/**
 * Initialize LCA module object
 * @param {function} callback	Function to call after module is successfully initialized
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
    LCA.enumData.nodeTypes = LCA.createEnumData(["Process", "Fragment", "InputOutput", "Background"]);
    LCA.shortNameBreakChars = d3.set([",", "(", ".", ";"]);
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
        top: '50%', // Top position relative to parent in px
        left: '50%' // Left position relative to parent in px
    },
    target = window.document.getElementById(tgtElementId);
    LCA.spinner = new Spinner(opts).spin(target);
};

/**
 * Create an associative array for loaded data.
 * @param {String} resourceName	    Loaded data resource name
 * @param {String} indexProperty    Data property to be used as key
 */
LCA.indexData = function (resourceName, indexProperty) {
    if (resourceName in LCA.loadedData && LCA.loadedData[resourceName] !== null) {
        var indexedData = [];
        LCA.loadedData[resourceName].forEach(function (d) {
            indexedData[+d[indexProperty]] = d;
        });
        return indexedData;
    }
    else {
        return null;
    }   
};

/**
 * Shorten a name so that it does not exceed maximum length.
 * Cut-off postion decided in the following order of preference
 *  1. last position of character in shortNameBreakChars
 *  2. last position of space char
 *  3. max length
 * 
 * @param {String} name	    The name to be shortened.
 * @param {String} maxLen   maximum length
 * @return {String} shortened name
 */
LCA.shortName = function (name, maxLen) {
    if (name.length > maxLen) {
        var endIndex = - 1;
        for (var i = maxLen - 1; i > 0 && endIndex === -1; --i) {
            if (LCA.shortNameBreakChars.has(name.charAt(i))) endIndex = i;
        }
        if (endIndex === -1) endIndex = name.lastIndexOf(" ", maxLen -1);
        if (endIndex === -1) endIndex = maxLen;
        return name.slice(0, endIndex);
    } else {
        return name;
    }
};

/**
 * GET resource data and save results
 * @param {String} resourceName     web service resource name
 * @param {Boolean} useTestData     Load json file for testing
 * @param {Function} callback       Function to call when done
 */
LCA.loadData = function (resourceName, useTestData, callback, paramString) {
   
    if (resourceName in LCA.loadedData) {
        callback.call();
        return false;
    }
    
    var jsonURL = (useTestData ? LCA.testDataFolder : LCA.baseURI) + resourceName;
    if (paramString) {
        jsonURL += paramString;
    }
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
 * @param {Array}  objects          Array of objects with ID and Name
 * @param {String} selectID         SELECT HTML element id
 * @param {String} oidName          Property name of object ID field.
 * @param {function} changeHandler  Function for handling selection update.
 * @param {Number} initialValue     Default value (selected object ID).
 */
LCA.loadSelectionList = function (objects, selectID, oidName, changeHandler, initialValue) {

    objects.sort(LCA.compareNames);

    var selectOptions = d3.select(selectID)
        .on("change", changeHandler)
        .selectAll("option")
        .data(objects)
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
};

// TODO : replace usage of following function with newer functions above
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