Backend - LCA Engine and Web Services
=====================================

Domain logic and web API are implemented here.

#Deployment Instructions

## XML File Server

The code now includes a file server for ILCD-formatted XML files for
transparency.  It leverages the ILCD format's existing high-quality schema
and stylesheet infrastructure to display ILCD files in a format that is
(ostensibly) pleasing to the eye.

In order for this mechanism to work, the ILCD-generic stylesheets and
schemas must be exposed via virtual directories on the web server (security
concerns require the XML files, schemas, and stylesheets to all be served
from the same host and port).

The `ILCD-generic\stylesheets` and `ILCD-generic\schemas` directories
already exist in the LCA_Data repository, so all that is required is to
expose them as virtual directories:

 * `<virtualDirectory path="/stylesheets" physicalPath="C:\path\to\GitHub\LCA_Data\ILCD-generic\stylesheets" />`
 * `<virtualDirectory path="/schemas" physicalPath="C:\path\to\GitHub\LCA_Data\ILCD-generic\schemas" />`

Note: on a debugging machine, the file to be edited is
`applicationhost.config` and it can be found in
`$USER_HOME\Documents\IISExpress\config\`.  Look for  (XPath)
`//configuration/system.applicationHost/sites/site` that matches the
project.

## Data Path Configuration

The XML file server also requires access to the physical directory that
stores the XML files.  This is the same as the directory used by the data
loader.

Currently that directory is configured in the `DataRoot` variable, stored
in [web.config](LCIAToolAPI/web.config) in the `appSettings` block.

## Access-Restricted Controller

The `LCIAComputationController` is used for debugging and for certain
maintenance tasks, including the initial population of the NodeCache and
ScoreCache (`GET /config/init`), clearing and recomputing the cache in the
event of a problem (`POST /config/scenarios/x/clearscorecaches` and `POST
/config/scenarios/x/clearnodecaches`); and creating new scenario groups
(`POST /config/scenariogroups/add`).

All the routes in this controller have the prefix `/config` instead of
`/api`.  Access to these routes should be limited at the server level to
computers authorized to make configuration changes.

Originally, this controller's access was restricted to `localhost` in the
`EnableCors` attribute specification in
[LCIAComputationController.cs](LCIAToolAPI/API/LCIAComputationController.cs).
Since CORS is not binding anyway, the CORS restriction is deprecated in
favor of host-level access control implemented by the web server.


## Documentation

The solution automatically builds an XML-formatted documentation file
called `LCAToolAPI.XML` which is generated in the `bin` directory.  This
file must be published with the rest of the project in order for the
documentation page to be available.

## Deployment

0. Create and populate the database using the [Data Loader](../Database/DataLoader).
1. Build solution (..\CalRecycleLCA.sln), Release configuration
2. Publish project, LCIAToolAPI. A publishing profile must first be configured. FTP is used at UCSB to publish to a test server. The profile is saved as LCIAToolAPI\Properties\PublishProfiles\kbcalr.pubxml.
3. Edit web.config in the publish destination.

    * In the connection string with name=UsedOilLCAContext, change the Data
      Source to the name of the server hosting a deployed database (see
      database deployment instructions in [..\..\Database\README](..\..\Database\README).
    * In the appSettings, edit the value for key `DataRoot` to point to the
      LCA data repository.
4. In the deployed database, add user IIS APPPOOL\DefaultAppPool and grant it connect, read, and write privileges to the database.
5. IIS Configuration

    * Configure virtual directories for XML schemas and stylesheets.
    * Configure access to configuration routes if desired.
    * Restart the published web app in IIS.

6. To initialize a newly deployed database, visit the API endpoint `GET
   /config/init` to generate the cache for all scenarios.

#Usage Instructions


The URL for the web API is the publish URL + /api/ + resource

Resource routes are defined in [ResourceController.cs](https://github.com/uo-lca/CalRecycleLCA/blob/master/LCIAToolAPI/LCIAToolAPI/API/ResourceController.cs)

Resources are defined in [Models](https://github.com/uo-lca/CalRecycleLCA/tree/master/LCIAToolAPI/Entities/Models)/*Resource.cs

API Resources
-------------

The API aspires to HATEOAS, but for now is documented in
LCIAToolAPI/README.md.  Note that this documentation is out of date.

### Process

GET:

    api/processes 
    api/processes/{processId}
    api/processes/{pid}/processflows
    api/processes/{pid}/flowproperties
    api/processes/{pid}/lciaresults

no PUT, POST, or DELETE.  Process data are referenced to ILCD-formatted
data files by UUID.

Processes from archives marked as private will not reveal processflows and
will only reveal aggregated LCIA results.


### Flow

GET:

    api/flows
    api/flows/{flowId}
    
    api/flowtypes 
    api/flowtypes/[1-2]/flows 
    api/flowtypes/[1-2]/processes - processes with (one or more) flows of type

 	       flowTypeID 1 = IntermediateFlow; 
	       		  2 = ElementaryFlow

no PUT, POST, or DELETE.  Process data are referenced to ILCD-formatted
data files by UUID.




### Fragment

GET:

    api/fragments
    api/fragments/{fragmentId}
    api/fragments/{fid}/fragmentstages
    api/fragments/{fid}/stages (synonym)
     
    api/fragments/{fid}/fragmentflows
    
    api/fragments/{fid}/flowproperties
    api/fragments/{fid}/flows
    
    api/fragments/{fid}/lciaresults
    

no PUT, POST, or DELETE.  Process data are referenced by UUID, but the
reference is to CSV data.


### LCIA Method

GET:

    api/lciamethods
    api/lciamethods/{lciaMethodId}
    api/lciamethods/{lciaMethodId}/lciafactors
    api/lciamethods/{lciaMethodId}/factors (synonym)
    
    api/impactcategories
    api/impactcategories/{categoryId}/lciamethods

no PUT, POST, or DELETE.  LCIA Method data are referenced to ILCD-formatted
data files by UUID.


### Scenario

Access to scenario data is governed by the ScenarioGroupID.  All scenarios
belonging to ScenarioGroupID = 1 may be viewed without authentication.

Scenarios belonging to a group other than ScenarioGroupID 1 are private.

CalRecycle's authentication server should give the client a token that
identifies the user's ScenarioGroup membership.  The backend will then
only reveal information about scenarios belonging to that group.

For the time being, the ScenarioGroup table has a field called "Secret".
API requests that require authorization will include a URL parameter called
"auth".  If the parameter value matches an entry in ScenarioGroup.Secret,
the matching ScenarioGroupID will be authorized.

GET: 

 (unauthorized)
    api/scenariogroups - returns empty
    api/scenarios - base scenario group
    api/scenarios/{scenarioId}... - must belong to base scenario group

 (authorized)
    api/scenariogroups - returns authorized group
    api/scenarios - belonging to authorized group
    api/scenarios/{scenarioId}... - must belong to authorized group
    
    api/scenarios/{sid}/params
    api/scenarios/{sid}/params/{paramId}
    
    api/scenarios/{sid}/nodesubstitutions
    api/scenarios/{sid}/nodesubstitutions/{fragmentFlowId}
 
   
Authorization is required for all POST, PUT, and DELETE.

POST:

    api/scenarios - create new scenario in authorized group

    api/scenarios/{sid}/params - create new param in named scenario

    api/scenarios/{sid}/nodesubstitutions/{fragmentFlowId} 
 - create new node substitution.
 - this is not really a POST


PUT / DELETE:

    api/scenarios/{sid} - update scenario

    api/scenarios/{sid}/params/{pid} - update param

    api/scenarios/{sid}/nodesubstitutions/{ffid} - update substitution




### LCA Result Computations and Analysis

All analysis routes can be filtered by LCIA method (i.e. replace
lciaresults with lciamethods/{lmid}/lciaresults)

* Process Analysis

    GET api/processes/{pid}/lciaresults

    GET api/scenarios/{sid}/processes/{pid}/lciaresults

* Contribution Analysis

  returns an LCIAResultResource (list) where results are grouped by
  FragmentStages.

    GET api/fragments/{fid}/fragmentflows
    GET api/fragments/{fid}/lciaresults
    
    GET api/scenarios/{sid}/fragments/{fid}/fragmentflows
    GET api/scenarios/{sid}/fragments/{fid}/lciaresults

* Sensitivity Analysis

  Returns the same LCIAResultResource (additive with the above resource)
  where the sensitivity of FragmentStages to the identified parameter.

    GET api/scenarios/{sid}/params/{pid}/lciasensitivity
    GET api/scenarios/{sid}/params/{pid}/fragments/{fid}/lciasensitivity
    
    POST api/scenarios/{sid}/lciasensitivity
    POST api/scenarios/{sid}/fragments/{fid}/lciasensitivity

  in the POST case, report sensitivity to an ad hoc Param Resource
  contained in POST data (may not be accurate for all param types) 
  (does NOT get added to scenario)





### Output

Get methods return entity properties in json format. Null properties are omitted.

#### Examples

http://publictest.calrecycle.ca.gov/LCAToolAPI/api/fragments

<pre><code>
[
  {
    "fragmentID": 1,
    "name": "Electricity, at grid",
    "referenceFragmentFlowID": 1
  },
  {
    "fragmentID": 2,
    "name": "Natural Gas Supply Mixer",
    "referenceFragmentFlowID": 86
  },
  {
    "fragmentID": 3,
    "name": "Local Collection Mixer",
    "referenceFragmentFlowID": 116
  },
  {
    "fragmentID": 4,
    "name": "Haz Waste Landfill Output",
    "referenceFragmentFlowID": 120
  },
  {
    "fragmentID": 5,
    "name": "Haz Waste Incineration Output",
    "referenceFragmentFlowID": 138
  },
  {
    "fragmentID": 6,
    "name": "Waste Oil Preprocessing",
    "referenceFragmentFlowID": 139
  },
  {
    "fragmentID": 7,
    "name": "Inter-Facility Mixer",
    "referenceFragmentFlowID": 144
  },
  {
    "fragmentID": 8,
    "name": "Local Collection Mixer",
    "referenceFragmentFlowID": 153
  },
  {
    "fragmentID": 9,
    "name": "Wastewater treatment plant (used oil)",
    "referenceFragmentFlowID": 191
  },
  {
    "fragmentID": 10,
    "name": "Waste incineration of used oil in municipal solid waste (MSW)",
    "referenceFragmentFlowID": 195
  },
  {
    "fragmentID": 11,
    "name": "Used oil, transport to landfill or incineration",
    "referenceFragmentFlowID": 202
  },
  {
    "fragmentID": 12,
    "name": "Improper disposal (splitter)",
    "referenceFragmentFlowID": 209
  }
]
</pre></code>


