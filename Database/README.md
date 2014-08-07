Database Initialization Tool
============================

The Database solution contains the following 2 projects:
  * LcaDataLoader - Console app creates and populates a database with data from ILCD archives and CSVs exported from MatLab.
  * LcaDataModel - Class library containing code first Entity Framework data model used by LcaDataLoader and the back end solution (../vs/LCIAToolAPI).
  
DataImport is a web app that loads individual data files. It is not currently used nor maintained.

The console app is named LcaDataLoader.exe. 
Options
=======
  -r, --root=DATA_ROOT       The full DATA_ROOT path.
  -s, --source=source name   ILCD archive source name.
  -c, --csv                  Load CSV files.
  -i, --initialize           Create database and seed.
  -d, --delete               Delete database, then initialize.
  -h, --help                 List options and exit


The default root path is current directory. If no options are selected, the app will list options and exit.

Database connection configuration: Edit LcaDataLoader/App.Config. Change Data Source to the database server name. 

The console app is configured to create log files in the same directory as the executable. This can be changed by editing the log4net File param in LcaDataLoader/App.Config. 

Examples:


Create the database using 
<pre><code>LcaDataLoader -i</pre></code>

Drop and recreate the database using 
<pre><code>LcaDataLoader -d</pre></code>

Data files loaded by LcaDataLoader are in the LCA_Data repository. 
Suppose those files are downloaded to C:\LCA_Data. They can be loaded as follows.
<pre><code>
LcaDataLoader -r "C:\LCA_Data" -s "Full UO LCA Flat Export BK 2014_05_05"
LcaDataLoader -r "C:\LCA_Data" -s "Full UO LCA Flat Export Ecoinvent 2014_04_24"
LcaDataLoader -r "C:\LCA_Data" -s "Full UO LCA Flat Export Improper 2014_04_01"
LcaDataLoader -r "C:\LCA_Data" -s "Full UO LCA Flat Export PE 2014_04_24"
LcaDataLoader -r "C:\LCA_Data" -s "ELCD-LCIA"
LcaDataLoader -r "C:\LCA_Data" -c
</pre></code>


  
  
 
 

