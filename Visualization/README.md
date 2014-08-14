CalRecycleLCA - Visualization
=============================

Front end for CalRecycle Used Oil LCA Online Tool.

Interactive web application implemented in HTML, CSS, and JavaScript. 

Visualization is implemented with the help of D3.js and uses SVG. SVG requires a "modern" browser (see [Can I use SVG](http://caniuse.com/svg)). 

### LCA

LCA.js and LCA.css contain shared code.

### How to Publish

Copy Visualization folder to a web server host. 
In IIS, create a web app that points to the Visualization folder.

Edit LCA.js - at the top of the file change baseURI setting to the base URI of the web API.

### Current Status

A new web page, **FragmentFlows**, is being developed. 
It will use a new version of the web API, also in development.
The following pages currently depend on the old version of web API (published at CalRecycle).

#### LciaComputation

User selects LCIA Method, Impact Category and Process from drop down selection lists. LCIA computation is performed and the total impact score is displayed. A stacked bar chart displays the associated elementary flows, where the size of each flow is proportional to the flow's positive contribution to the total score.

#### IntermediateFlows

User selects Process from drop down selection list. Front end queries IntermediateFlows web API method with selected processid parameter. A sankey diagram is displayed. The center node represents the selected process. Each intermediate flow is represented by a node and a link to/from the process. Input flows are displayed to the left of the process, while output flows are displayed on the right.

An old prototype of waterfall charts displaying **Contribution Analysis** is also included. It reads data from a json file. 
A new version will be developed to display LCIA Computation results when that has been implemented server side. The dependency on bootstrap will also be eliminated.