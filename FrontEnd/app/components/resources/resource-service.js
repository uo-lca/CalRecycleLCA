/**
 * Service to manage access to resources from web API
 */
angular.module('lcaApp.resources.service', ['ngResource', 'lcaApp.idmap.service', 'lcaApp.resources.lciaMethod' ])
    .constant('API_ROOT', "http://localhost:60393/api/")
    .constant('MODEL_BASE_CASE_SCENARIO_ID', 1)
    .factory('ResourceService', ['$resource', 'API_ROOT', 'IdMapService', '$q',
        function($resource, API_ROOT, IdMapService, $q){
            var resourceService = {},   // Singleton creates specific service type objects
                services = {};          // Services created for, and shared by controllers

            resourceService.ROUTES = {
                "flowForFragment" : API_ROOT + "fragments/:fragmentID/flows",
                "fragment" : API_ROOT + "fragments/:fragmentID",
                "fragmentFlow" : API_ROOT + "scenarios/:scenarioID/fragments/:fragmentID/fragmentflows",
                "fragmentStage" : API_ROOT + "fragments/:fragmentID/fragmentstages",
                "flowPropertyForFragment" : API_ROOT + "fragments/:fragmentID/flowproperties",
                "flowPropertyForProcess" : API_ROOT + "processes/:processID/flowproperties",
                "impactCategory" : API_ROOT + "impactcategories",
                "nodeType" : "components/resources/nodetypes.json",
                "lciaMethod" : API_ROOT + "lciamethods",
                "lciaMethodForImpactCategory" : API_ROOT + "impactcategories/:impactCategoryID/lciamethods",
                "lciaResultForFragment" : API_ROOT + "scenarios/:scenarioID/fragments/:fragmentID/lciamethods/:lciaMethodID/lciaresults",
                "lciaResultForProcess" : API_ROOT + "scenarios/:scenarioID/processes/:processID/lciamethods/:lciaMethodID/lciaresults",
                "process" : API_ROOT + "processes",
                "processForFlowType" : API_ROOT + "flowtypes/:flowTypeID/processes",
                "processFlow" : API_ROOT + "processes/:processID/processflows",
                "scenario" : API_ROOT + "scenarios"
            };

            resourceService.getResource = function( routeKey) {
                if ( routeKey in this.ROUTES) {
                    return $resource( this.ROUTES[routeKey], {}, {
                        get: {method: 'GET', cache: true, isArray: false},
                        query: {method: 'GET', cache: true, isArray: true}
                    });
                }
            };

            /**
             * Create service object for a particular route
             * @param {String} routeKey   Key to ROUTES
             * @param {String} idName     Name of ID property in query result objects
             * @returns the object
             */
            resourceService.createService = function( routeKey, idName) {
                var svc =
                    { loadFilter: null,
                      resource: resourceService.getResource(routeKey), // Instance of $resource
                      objects: null,    // Loaded objects
                      extensionFactory: null // Factory for extending loaded objects
                    };

                /**
                 * Load resources using filter. Cache results using IdMapService
                 * If extension objects exists, extend the resource objects.
                 * @param {Object} filter   Resource query filter
                 * @returns promise of loaded resources
                 */
                svc.load = function(filter) {
                    var d = $q.defer();
                    if (filter === svc.loadFilter && svc.objects) {
                        d.resolve(svc.objects);
                    } else {
                        svc.loadFilter = filter;
                        svc.objects = svc.resource.query( filter,
                            function(objects) {
                                if (svc.extensionFactory) {
                                    objects.forEach( function (o) {
                                        angular.extend(o, svc.extensionFactory.createInstance())
                                    });
                                }
                                if (svc.idName) {
                                    IdMapService.add(svc.idName, objects);
                                }
                                d.resolve(objects);
                            },
                            function(err) {
                                d.reject("Web API query failed. URL: " + err.config.url);
                            })
                    }
                    return d.promise;
                };

                /**
                 * comparison function for sorting query result objects
                 * by name.
                 */
                svc.compareByName = function (a, b) {
                    if (a.name > b.name) {
                        return 1;
                    }
                    if (a.name < b.name) {
                        return -1;
                    }
                    // a must be equal to b
                    return 0;
                };

                /**
                 * Get query results
                 * @return {Array} the results
                 */
                svc.getAll = function () {
                    return svc.objects;
                };

                if (idName) {
                    svc.idName = idName;
                    /**
                     * Get loaded resource by object ID
                     * @param id    Object ID value
                     * @returns loaded resource, if found.
                     */
                    svc.get = function(id) {
                       return IdMapService.get(svc.idName, id);
                    };
                }

                /**
                 * Set optional extension object.
                 * @param obj   The extension object
                 * @returns this service
                 */
                svc.setExtensionFactory = function (obj) {
                    svc.extensionFactory = obj;
                    return svc;
                };

                return svc;
            };

            /**
             * Create service if not already created.
             * @param {String} serviceName    Key to service object
             * @param {String} routeKey   Key to ROUTES
             * @param {String} idName     Name of ID property in query result objects
             * @returns loaded resource, if found.
             */
            resourceService.getService = function (serviceName, routeKey, idName) {
                if (! (serviceName in services)) {
                    services[serviceName] = resourceService.createService(routeKey, idName);
                }
                return services[serviceName];
            };

            return resourceService;
        }
    ]);
//
// Factories using functional inheritance
//
angular.module('lcaApp.resources.service')
    .factory('ScenarioService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('ScenarioService', "scenario", "scenarioID");
        }
    ])
    .factory('FragmentService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('FragmentService', "fragment", "fragmentID");
        }
    ])
    .factory('ProcessService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('ProcessService', "process", "processID");
        }
    ])
    .factory('ProcessForFlowTypeService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('ProcessForFlowTypeService', "processForFlowType", "processID");
        }
    ])
    .factory('FlowPropertyForFragmentService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('FlowPropertyForFragmentService', "flowPropertyForFragment", "flowPropertyID");
        }
    ])
    .factory('FlowPropertyForProcessService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('FlowPropertyForProcessService', "flowPropertyForProcess", "flowPropertyID");
        }
    ])
    .factory('FragmentFlowService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('FragmentFlowService', "fragmentFlow", "fragmentFlowID");
        }
    ])
    .factory('FragmentStageService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('FragmentStageService', "fragmentStage", "fragmentStageID");
        }
    ])
    .factory('FlowForFragmentService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('FlowForFragmentService', "flowForFragment", "flowID");
        }
    ])
    .factory('ImpactCategoryService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('ImpactCategoryService', "impactCategory", "impactCategoryID");
        }
    ])
    .factory('LciaMethodService', ['ResourceService', 'LciaMethodExtension',
        function(ResourceService, LciaMethodExtension){
            return ResourceService.getService('LciaMethodService', "lciaMethod", "lciaMethodID")
                .setExtensionFactory(LciaMethodExtension);
        }
    ])
    .factory('LciaMethodForImpactCategoryService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('LciaMethodForImpactCategoryService', "lciaMethodForImpactCategory",
                "lciaMethodID");
        }
    ])
    .factory('NodeTypeService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('NodeTypeService', "nodeType", "nodeTypeID");
        }
    ])
    .factory('ProcessFlowService', ['ResourceService',
        function(ResourceService){
            return ResourceService.getService('ProcessFlowService', "processFlow", null);
        }
    ]);

/**
 * LCIA results for multiple methods are queried simultaneously.
 * Also, the query returns only one object, so no need to use IdMapService
 */
angular.module('lcaApp.resources.service')
    .factory('LciaResultForProcessService', ['ResourceService',
        function(ResourceService){
            var resource = ResourceService.getResource("lciaResultForProcess"),
                resultSvc = {};

            resultSvc.get = function(filter, callback) {
                return resource.get(filter, callback);
            };

            return resultSvc;
        }
    ])
    .factory('LciaResultForFragmentService', ['ResourceService',
        function(ResourceService){
            var resource = ResourceService.getResource("lciaResultForFragment"),
                resultSvc = {};

            resultSvc.get = function(filter, callback) {
                return resource.get(filter, callback);
            };

            return resultSvc;
        }
    ]);