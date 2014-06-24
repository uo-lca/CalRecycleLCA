Database Initialization Tool
============================

The Database solution contains the following 2 projects:
  * LcaDataLoader - Console app creates and populates a database with data from ILCD archives and CSVs exported from MatLab.
  * LcaDataModel - Class library containing code first Entity Framework data model used by LcaDataLoader.
  
DataImport is a web app that loads individual data files. It is not currently used nor maintained.

The console app is named LcaDataLoader.exe and has the following arguments:
  * -r The full data root path Default: C:\\CalRecycleLCA-DATA_ROOT
  * -s ILCD archive source name
  * -d Delete database and recreate.
  * -c Load CSV files
 
The console app creates a log file in the current directory. Logging can be configured by editing app.Config. (See log4net documentation for instructions).

Data files loaded by LcaDataLoader are in the LCA_Data repository.
 
Examples: 

 Recreate database and load ILCD archive using
  <pre><code>LcaDataLoader -r "C:\CalRecycleLCA-DATA_ROOT" -s "Full UO LCA Flat Export BK 2014_05_05" -d</pre></code>

 Update database and load ILCD archive using
  <pre><code>LcaDataLoader -r "C:\CalRecycleLCA-DATA_ROOT" -s "Full UO LCA Flat Export Ecoinvent 2014_04_24"</pre></code>

  Update database and load CSV files using
  <pre><code>LcaDataLoader -r "C:\CalRecycleLCA-DATA_ROOT" -c</pre></code>
  
  
 
 

