CalRecycleLCA
=============

CalRecycle Used Oil LCA Online Tool

CalRecycleLCA.sln is a Visual Studio solution containing active projects in
the Database and LCIAToolAPI folders plus Documentation files.

The Database folder contains a shared Entity Framework data model and a
database initialization utility.

Documentation contains application specifications.

LCIAToolAPI contains backend projects implementing the core LCA engine and
Web API.

Details are described in subfolder README files:

 * [Back end](LCIAToolAPI/)
 * [Data loader](Database/)

The project also includes a front end application for data visualization.
The front end is a single page app that runs in the user's browser and can
connect to any back end server.  The front end code is in
[a separate repository](https://github.com/uo-lca/FrontEnd).

The Data loader (and by extension the back end) requires an LCA data
directory, which is [a separate repository](https://github.com/uo-lca/LCA_Data).

