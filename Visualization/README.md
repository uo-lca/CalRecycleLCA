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

Two visualizations, listed below, get data from the latest version of the web API (under  [LCIAToolAPI](https://github.com/uo-lca/CalRecycleLCA/tree/master/vs/LCIAToolAPI)). A third visualization, for fragment LCIA, has been prototyped and will be completed after back end computation and caching has been completed.

#### FragmentFlows
Sankey diagram of fragment flows.
12 fragments have been defined. The web page defaults to fragmentID=6. 

Start with a different fragment by adding a fragmentid URL variable. Example:
`http://publictest.calrecycle.ca.gov/LCIAToolVisualization/FragmentFlows.html?fragmentid=2`

Navigate to another fragment by clicking on a node with fragment type (node type is shown on hover). 


#### LciaComputation

User selects LCIA Method, Impact Category and Process from drop down selection lists. LCIA computation is performed and the total impact score is displayed. A stacked bar chart displays the associated elementary flows, where the size of each flow is proportional to the flow's positive contribution to the total score.

#### LciaWaterfall

Prototype waterfall diagram. Fragment LCIA results will be visualized in a waterfall diagram.
