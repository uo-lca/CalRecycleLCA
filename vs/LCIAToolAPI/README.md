Web API Specification
=====================

HTTP Verb   | URI                   | Description
----------  | -------------         | ------------
GET         | api/fragments         | List all fragments 
GET         | api/fragments/{id}    | Get fragment with FragmentID = {id}
GET         | api/fragments/{id}/links    | List links for sankey diagram of fragment with FragmentID = {id}
GET         | api/fragments/{id}/flows    | List flows used in fragment with FragmentID = {id}
GET         | api/fragments/{id}/flowproperties    | List flow properties associated with fragment having FragmentID = {id}
GET         | api/processes    | List all processes