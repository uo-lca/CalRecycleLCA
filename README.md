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

FrontEnd contains a single page app for data visualization.

Details are described in subfolder README files:

 * [Back end](tree/master/LCIAToolAPI/)
 * [Front end](tree/master/FrontEnd/)
 * [Data loader](tree/master/Database/)

The Data loader (and by extension the back end) requires an LCA data
directory, which is [a separate repository](../LCA_Data/).

