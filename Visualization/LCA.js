/**
 * Shared module for Used Oil LCA Visualization
 */
// global directives to JSLint
/*global d3, window, console, Spinner */
// reference directives to Visual Studio Intellisense
/// <reference path="d3.min.js" />
/// <reference path="spin.min.js" />
var LCA = {
    //baseURI: "http://kbcalr.isber.ucsb.edu/api/",
    baseURI: "http://localhost:60393/api/",
    formatNumber: d3.format("^.2g"),
    testDataFolder: "",
    loadedData: {}, // Data loaded via web API (or from TestData)
    spinner: null,
    indexedData: {}, // Associative array of loaded data, keyed by web api resource name
    enumData: {},   // Associative arrays of enum data
    urlVars: {},    // Associative array of parameters in querystring
    outstandingRequests: 0  // Used to decide when to start/stop spinner
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
    LCA.loadUrlVars();
    callback.call();
};

/**
 * Compare function used to sort array of objects by Name.
 */
LCA.compareNames = function (a, b) {
    return d3.ascending(a.name, b.name);
};

LCA.createSpinner = function (tgtElementId) {
    
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
        top: '25%', // Top position relative to parent in px
        left: '25%' // Left position relative to parent in px
    };
    LCA.spinTarget = window.document.getElementById(tgtElementId);
    LCA.spinner = new Spinner(opts);
    LCA.outstandingRequests = 0;
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
    } else {
        return null;
    }   
};

/**
 * Shorten a name so that it does not exceed maximum length.
 * Cut-off position decided in the following order of preference
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
            if (LCA.shortNameBreakChars.has(name.charAt(i))) { endIndex = i; }
        }
        if (endIndex === -1) { endIndex = name.lastIndexOf(" ", maxLen - 1); }
        if (endIndex === -1) { endIndex = maxLen; }
        return name.slice(0, endIndex);
    } else {
        return name;
    }
};

/**
 * Append a table, table header and table body
 * @param {Object} parSelection     d3 selection after which to append table
 * @param {Array} columns           column header names
 * @returns {Object}     d3 selection containing table body
 */
LCA.createTable = function (parSelection, columns) {
    var table = parSelection.append("table"),
        thead = table.append("thead");
        table.append("tbody");
    // append the header row
        thead.append("tr")
            .selectAll("th")
            .data(columns)
            .enter()
            .append("th")
            .text(function (column) { return column; });
        return table;
};

/**
 * Display data in table body
 * Hide table header when data empty.
 * @param {Object} table    d3 selection containing table
 * @param {Array} data      data indexed by column header names
 * @param {Array} columns   column header names
 */
LCA.updateTable = function (table, data, columns) {

    if (data.length > 0) {
        table.select("thead").style("display", "table-header-group");
    } else {
        table.select("thead").style("display", "none");
    }
    // create a row for each object in the data
    var rows = table.select("tbody").selectAll("tr")
        .data(data);

    rows.enter()
        .append("tr");

    // create a cell in each row for each column
    rows.selectAll("td")
        .data(function (row) {
            return columns.map(function (column) {
                return { column: column, value: row[column] };
            });
        })
        .enter()
        .append("td");

    rows.selectAll("td").html(function (d) { return d.value; });

    // remove rows that have no data
    rows.exit().remove();
};

/**
 * GET resource data and save results
 * @param {String} resourceName     web service resource name
 * @param {Boolean} useTestData     Load json file for testing
 * @param {Function} callback       Function to call when done
 * @param {String} routePrefix      Web API route prefix
 */
LCA.loadData = function (resourceName, useTestData, callback, routePrefix) {
    
    if (resourceName in LCA.loadedData) {
        delete LCA.loadedData[resourceName];
    }
    var jsonURL = (useTestData ? LCA.testDataFolder : LCA.baseURI);
    if (arguments.length === 4) {
        jsonURL = jsonURL + routePrefix + "/";
    }   
    jsonURL += resourceName;
    if (useTestData) {
        jsonURL += ".json";
    }
    console.log("Request data from  " + jsonURL);
    if (LCA.spinner && LCA.outstandingRequests === 0) {
        LCA.spinner.spin(LCA.spinTarget);
    }
    ++LCA.outstandingRequests;
    d3.json(jsonURL, function (error, jsonData) {
        --LCA.outstandingRequests;
        if (LCA.spinner && LCA.outstandingRequests === 0) {
            LCA.spinner.stop();
        }
        if (error) {
            window.alert("Error executing GET on " + jsonURL);
            console.error(error);
            LCA.loadedData[resourceName] = null;
        } else {
            console.log("Got data from " + jsonURL);
            LCA.loadedData[resourceName] = jsonData;
        }
        callback.call();
    });
    return true;
};

LCA.emptySelectionList = function (selectID) {
    d3.select(selectID)
        .selectAll("option")
        .remove();
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
    var selectOptions;

    objects.sort(LCA.compareNames);
    //
    // Internet Explorer does not display updated options correctly so remove/append only
    //
    d3.select(selectID)
        .selectAll("option")
        .remove();

    selectOptions = d3.select(selectID)
        .selectAll("option")
        .data(objects);

    selectOptions.enter()
        .append("option");
    // selectOptions.exit().remove();

    selectOptions.attr("value", function (d) {
            return d[oidName];
        })
        .text(function (d) {
            return d.name;
        })
        .attr("selected", function (d) {
            return d[oidName] === initialValue ? true : null;
        });

    d3.select(selectID).on("change", changeHandler);
};

/**
 * Read current page's URL variables and store them as an associative array.
 */
LCA.loadUrlVars = function() {
    var hash,
        varName,
        curPageURL = window.location.href,
        idRefIndex = curPageURL.indexOf('#'),
        hashes = [];
    if (idRefIndex >= 0) {
        curPageURL = curPageURL.slice(0, idRefIndex);
    }
    hashes = curPageURL.slice(curPageURL.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        varName = hash[0].toLowerCase();
        LCA.urlVars[varName] = hash[1];
    }
};

/**
 * Make a simple legend from chart data and color scale
 * @param {Object} svg          d3 selection to contain legend
 * @param {Array} data          Chart data
 * @param {Object} scale        d3 scale mapping data to colors
 * // TODO : Add optional property for legend text
 */
LCA.makeLegend = function (svg, data, scale) {
    var rowHeight = 20,
        boxSize = 18,
        colPadding = 6,
        textY = 9,
        colXs = [0, boxSize + colPadding],
        legend,
        newRows;

    // Update legend data
    legend = svg.selectAll(".legend").data(data);
    // Add rows, if necessary
    newRows = legend.enter().append("g").attr("class", "legend")
        .attr("transform", function (d, i) {
            var rowY = (i + 1) * rowHeight;
            return "translate(0," + rowY + ")";
        });
    newRows.append("rect")
        .attr("x", colXs[0])
        .attr("width", boxSize)
        .attr("height", boxSize)
        .style("fill", function (d) {
            return scale(d);
        });
    newRows.append("text")
        .attr("x", colXs[1])
        .attr("y", textY)
        .attr("dy", ".35em")
        .attr("class", "legend text")
        .text(function (d) {
            return d;
        });

    // Remove unused rows
    legend.exit().remove();
};

// TODO : replace usage of following function with newer loadSelectionList
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