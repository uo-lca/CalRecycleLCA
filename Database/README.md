LCA Data Model
==============

The Used Oil LCA tool uses Microsoft Entity Framework to maintain the data
model in code.  The LcaDataModel project is an Entity Framework model
containing all the database tables.  The LcaDataLoader project can create
and update databases generated with the model, and populate them with
data.

Migrations
----------

To perform a migration, first edit the data model to make the change. Then,
in Visual Studio open the package manager console (Tools -> Library package
manager -> package manager console) and run the command:

    PM> Add-Migration my-migration-name -ProjectName LcaDataModel  -StartupProjectName LCIAToolAPI
	
The migration tool will generate a file in `DataModel/Migrations`
containing the changes to the SQL server.

### Applying Migrations

Migrations can be applied manually in visual studio or via the data
loader.

In Visual Studio, from the package manager console enter

    PM> Update-Database my-migration-name -ProjectName LcaDataModel \
        -StartupProjectName LCIAToolAPI -ConnectionString "contents of connection \
        string as specified in web.config" -Verbose

Add the `-Script` switch to generate a SQL query without executing it.

To apply the migration via the data loader, first ensure that the migration
file is present in `DataModel/Migrations`, and then simply run the data
loader with the `-u` switch.


Database Initialization Tool
============================

The Database folder contains the following 2 projects:
  * LcaDataLoader - Console app creates and populates a database with data from ILCD archives and CSVs exported from MatLab.
  * LcaDataModel - Class library containing code first Entity Framework data model used by LcaDataLoader and the back end solution (../vs/LCIAToolAPI).


The console app is named LcaDataLoader.exe and it has the following options.


Option                     | Description
---------------------------|----------------------------------
-r, --root=DATA_ROOT       | The full DATA_ROOT path.
-s, --source=source name   | ILCD archive source name.
-c, --csv                  | Load CSV files.
-i, --initialize           | Create database and seed.
-u, --upgrade              | Upgrade database to latest version.
-d, --delete               | Delete database, then initialize.
-h, --help                 | List options and exit


The default root path is current directory. If no options are selected, the app will list options and exit.

Package Dependencies
--------------------
The projects in this folder depend on the following NuGet Packages. 
* CSV Reader
* EntityFramework
* log4net
* NDesk.Options

They are included in packages.config files and should be installed automatically at the start of the first build.

Deployment Instructions
-----------------------
1. Build solution (..\CalRecycleLCA.sln), Release configuration
2. Copy contents of DataLoader\bin\Release to the server where database initialization is to be executed.
3. In the server folder containing the release build, edit LcaDataLoader.exe.config
  1. In connectionStrings, change Data Source to the name of the SQL Server instance where the database is to be created.
  2. log4net is configured to create log files in the same directory as the executable. If you want to change the location of the log files, edit the value of the File param (prepend a path).
4. As a user who has access to create a database, open a command prompt on the server and cd to the directory containing the release build. 

 If this is a new installation, create the database by executing
 <pre><code>LcaDataLoader -i</pre></code>
 If the database was created by an earlier version, back it up, then upgrade it by executing
    <pre><code>LcaDataLoader -u</pre></code>
5. If other users will be loading data, grant them write access to the new database, UsedOilLCA. 

Usage Instructions
------------------
1. Download data from [GitHub Repository LCA_Data](https://github.com/uo-lca/LCA_Data/) to the server where data is to be loaded.
2. As a user who has access to update data in UsedOilLCA, load ILCD data archives first, then CSV files. The ILCD archives overlap. If an ILCD entity has been loaded from one archive, it will not be overwritten when an archive containing the same entity is loaded. CSV files contain references to ILCD entities, so they should be loaded last. Suppose that LCA_Data has been downloaded to C:\LCA_Data, the following sequence of commands will load all the files.
<pre><code>
LcaDataLoader -r "C:\LCA_Data" -s "Full UO LCA Flat Export BK"
LcaDataLoader -r "C:\LCA_Data" -s "Full UO LCA Flat Export Ecoinvent"
LcaDataLoader -r "C:\LCA_Data" -s "Full UO LCA Flat Export Improper"
LcaDataLoader -r "C:\LCA_Data" -s "Full UO LCA Flat Export PE-SP24"
LcaDataLoader -r "C:\LCA_Data" -s "Full UO LCA Flat Export PE-SP22"
LcaDataLoader -r "C:\LCA_Data" -s "ELCD-LCIA"
LcaDataLoader -r "C:\LCA_Data" -c
</pre></code>




  
  
 
 

