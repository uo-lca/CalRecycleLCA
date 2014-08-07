Database Initialization Tool
============================

The Database solution contains the following 2 projects:
  * LcaDataLoader - Console app creates and populates a database with data from ILCD archives and CSVs exported from MatLab.
  * LcaDataModel - Class library containing code first Entity Framework data model used by LcaDataLoader and the back end solution (../vs/LCIAToolAPI).
  
DataImport is a web app that loads individual data files. It is not currently used nor maintained.

The console app is named LcaDataLoader.exe and has the following arguments:
  * -r The full data root path
  * -s ILCD archive source name
  * -i Create database and seed it.
  * -d Delete database and recreate.
  * -c Load CSV files

Database connection configuration: Edit LcaDataLoader/App.Config. Change Data Source to the database server name. 

The console app creates log files in the current directory. The log file name and path can be configured by editing the log4net File param in LcaDataLoader/App.Config. 

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


  
  
 
 

