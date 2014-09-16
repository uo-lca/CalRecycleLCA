==========
Mon Sep 15 23:05:10 PDT 2014



toying with the idea that UserID and ScenarioGroupID are synonymous.  All
users can GET records associated with ScenarioGroup 1 and their own
ScenarioGroup.  All users can PUT and POST their own ScenarioGroup.


All starred routes, e.g. Route*("api/fragments/{fragmentID:int}/flows"),
require a ScenarioID, for which the default is 0 "Model Base Case".
ScenarioID specifications can be put anywhere in the route that the
ResourceController can easily parse. (e.g. can "scenario/[0-9]+/" be
removed and parsed automagically from any position in the route?)

"api/scenario/{ScenarioID}/fragments/{fragmentID}/flows"
"api/fragments/{fragmentID}/scenario/{ScenarioID}/flows"



=============
API Endpoints
=============


	Route("api/doc/{DocID}")

Returns a Doc record: comment, url.  (the url can be e.g. to a CMS node)
The comment can be set with "PUT".

A DocID for a Param table entry (or substitution table) belongs to the
owner of the ScenarioID associated with the param.



	Route("api/doc/{docID}")

	Route("api/scenario")

Returns a list of scenarios the user has access to.  

	Route("api/scenario/{ScenarioID:int}")

Returns a list of params + substitutions by DocID

	Route("api/fragments/{fragmentID:int}/fragmentflows")



	Route("api/fragments/{fragmentID:int}/scenarios/{scenarioID:int}/fragmentflows")
	Route("api/fragments/{fragmentID:int}/flows")
	Route("api/fragments/{fragmentID:int}/flowproperties")
	Route("api/impactcategories")
	Route("api/impactcategories/{impactCategoryID:int}/lciamethods")
	Route("api/lciamethods")
	Route("api/processes")
	Route("api/processes/{processID:int}/flows")
	Route("api/processes/{processID:int}")
	Route("api/processes/{processID:int}/lciamethods/{lciaMethodID:int}/lciaresults")
	Route("api/fragments/{fragmentID:int}/lciaresults")
