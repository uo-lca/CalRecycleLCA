CURL commands 
=============

for testing PUT, POST, and DELETE

Fri Jan 09 16:04:06 -0800 2015


In order to provide POST data to Web API, the Content-type header must be
set and the file must be properly formatted.  It is then parsed as whatever
resource type is specified in the route command.

Auth information should be provided via a URL parameter called 'auth' and
the string should match ScenarioGroup.Secret for the desired
ScenarioGroupID. 

for Publictest:
BASE_URL = http://publictest.calrecycle.ca.gov/LCAToolAPI

for localhost:
BASE_URL = http://localhost:60393

# Scenarios

## Creating Scenarios

To create a scenario:

curl -v -H "Content-Type: application/json" -X POST -d @POST_scenarios.json \
 $BASE_URL/api/scenarios?auth=2514bc8

## Updating scenarios

To modify a scenario's top-level fragment (necessitates recomputation):

curl -v -H "Content-Type: application/json" -X PUT -d @PUT_scenario_tlf.json \
 $BASE_URL/api/scenarios/7?auth=2514bc8

To modify a scenario's non-computation-related aspects (name, activity
level): 

curl -v -H "Content-Type: application/json" -X PUT -d @PUT_scenario_name.json \
 $BASE_URL/api/scenarios/5?auth=2514bc8 

## Deleting scenarios

To delete a scenario (by ID only):

curl -v -H "Content-Type: application/json" -X DELETE -d '' \
 $BASE_URL/api/scenarios/5?auth=4e49337

# Parameters

## Creating or Updating parameters

To create or modify a parameter, based on type and ID:

curl -v -H "Content-Type: application/json" -X POST  -d @POST_params.json \
 $BASE_URL/api/scenarios/7/params?auth=2514bc8

To modify a parameter's NAME or VALUE ONLY based on ParamID:

curl -v -H "Content-Type: application/json" -X PUT  -d @PUT_params.json \
 $BASE_URL/api/scenarios/7/params/13?auth=2514bc8

*note- for ParamTypeID 1 and 5, the ID must also be provided even for put,
 because the schema permits Param:one-to-many:DependencyParam and
 Param:one-to-many:CompositionParam  

## Deleting parameters

To delete a parameter (by ID only):

curl -v -H "Content-Type: application/json" -X DELETE -d '' \
 $BASE_URL/api/scenarios/5/params/15?auth=4e49337 
