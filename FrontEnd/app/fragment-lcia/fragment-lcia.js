'use strict';
/* Controller for Fragment LCIA Diagram View */
angular.module('lcaApp.fragment.LCIA',
                ['ui.router', 'lcaApp.resources.service', 'lcaApp.status.service',
                 'lcaApp.colorCode.service', 'lcaApp.waterfall',
                    'isteven-multi-select', 'lcaApp.selection.service',
                    'lcaApp.fragmentNavigation.service'])
    .constant('SELECTION_KEYS', {
        topLevelFragmentID : "TLF",
        fragmentScenarios : "FS"
    })
    .controller('FragmentLciaCtrl',
        ['$scope', '$stateParams', '$state', 'StatusService', '$q', 'ScenarioService', 'MODEL_BASE_CASE_SCENARIO_ID',
         'FragmentService', 'FragmentStageService',
         'LciaMethodService', 'LciaResultForFragmentService',
         'ColorCodeService', 'WaterfallService', 'SelectionService', 'SELECTION_KEYS',
            'FragmentNavigationService',
        function ($scope, $stateParams, $state, StatusService, $q, ScenarioService, MODEL_BASE_CASE_SCENARIO_ID,
                  FragmentService, FragmentStageService,
                  LciaMethodService, LciaResultForFragmentService,
                  ColorCodeService, WaterfallService, SelectionService, SELECTION_KEYS,
                  FragmentNavigationService ) {

            var fragmentID,
                stages = [],
                results = {};

            $scope.onFragmentChange = function () {
                var selectedFragment = $scope.fragmentSelection.model[0];
                if (selectedFragment.fragmentID !== fragmentID) {
                    clearWaterfalls();
                    $scope.fragment = selectedFragment;
                    fragmentID = selectedFragment.fragmentID;
                    getFragmentScenarios();
                }
            };

            /**
             * Remove LCIA method. Used to close panel.
             * @param m Method displayed by panel to be closed
             */
            $scope.removeMethod = function(m) {
                var index = $scope.methods.indexOf(m);
                if (index > -1) {
                    $scope.methods.splice(index, 1);
                }
            };

            $scope.scenarios = [];
            $scope.methods = [];
            $scope.colors = {};
            $scope.waterfalls = {};
            /**
             * Navigation service set if current view supports fragment navigation.
             * If null, then current view supports selection of top-level fragment and associated scenarios.
             */
            $scope.navigationService = null;


            /**
             * Called when a parent fragment is selected from fragment breadcrumbs.
             * Updates fragment breadcrumbs and gets new fragment data.
             * @param fragment  Fragment selected
             * @param index     Breadcrumb index
             */
            $scope.onParentFragmentSelected = function (fragment, index) {
                fragmentID = fragment.fragmentID;
                $scope.fragment = fragment;
                FragmentNavigationService.setLast(index);
                clearWaterfalls();
                getLciaResults();
            };

            function getSelectionResults() {
                $scope.scenarios = $scope.scenarioSelection.model;
                getLciaResults();
                // Save selections for return to view in same session
                SelectionService.set(SELECTION_KEYS.topLevelFragmentID, $scope.fragment.fragmentID);
                SelectionService.set(SELECTION_KEYS.fragmentScenarios, $scope.scenarioSelection.model);
            }

            /**
             * Get LCIA results for a scenario and method.
             * Multiply cumulativeResult by scenario's activity level.
             * Store in local cache indexed by (methodID, scenarioID, fragmentStageID).
             * @param {{ lciaMethodID : Number, lciaScore : Array }} lciaResult
             */
            function extractResult(lciaResult) {
                var scenario = ScenarioService.get(lciaResult.scenarioID),
                    result = {},
                    activityLevel = scenario.activityLevel;

                if ($scope.navigationService && $scope.fragment.hasOwnProperty("activityLevel")) {
                    activityLevel = $scope.fragment.activityLevel;
                }
                lciaResult.lciaScore.forEach(
                    /* @param score {{ fragmentStageID : Number,  cumulativeResult : Number }} */
                    function ( score) {
                        result[score["fragmentStageID"]] = score.cumulativeResult * activityLevel;
                });
                if (! (lciaResult.lciaMethodID in results)) {
                    results[lciaResult.lciaMethodID] = {};
                }
                results[lciaResult.lciaMethodID][lciaResult.scenarioID] = result;
            }

            function getName(o) {
                return o.name;
            }

            function clearWaterfalls() {
                $scope.waterfalls = {};
            }

            /**
             * Create data model for waterfall directive using waterfall service.
             * Results are grouped by method. All scenarios for a method must be
             * in the same waterfall instance because the extent of the value axis
             * is determined by the full range of values across all scenarios.
             */
            function buildWaterfalls() {
                var stageNames;
                stages = FragmentStageService.getAll();
                stageNames = stages.map(getName);
                StatusService.stopWaiting();
                $scope.methods.forEach( function (m) {
                    var wf;
                    if (m.lciaMethodID in results) {
                        var values = [], methodResults = results[m.lciaMethodID];
                        $scope.scenarios.forEach( function (scenario) {
                            var stageValues = [];
                            stages.forEach(
                                /* @param stage {{ fragmentStageID : Number, name : String }} */
                                function (stage) {
                                    var stageID = stage["fragmentStageID"];
                                    if (scenario.scenarioID in methodResults &&
                                        stageID in methodResults[scenario.scenarioID]) {
                                        stageValues.push(methodResults[scenario.scenarioID][stageID]);
                                    } else {
                                        stageValues.push(null);
                                    }
                            });
                            values.push(stageValues.slice(0));
                        });
                        wf = new WaterfallService.createInstance();
                        wf.scenarios($scope.scenarios)
                            .stages(stageNames)
                            .values(values.slice(0))
                            .width(300);
                        wf.layout();
                        $scope.waterfalls[m.lciaMethodID] = wf;
                    } else {
                        $scope.waterfalls[m.lciaMethodID] = null;
                    }
                });
            }

            /**
             * Request fragment stages, then
             * for each scenario and method combination, request LCIA results.
             * When all results are in, build waterfalls.
             */
            function getLciaResults() {
                var promises = [],
                    result;
                StatusService.startWaiting();
                result = FragmentStageService.load({fragmentID: fragmentID});
                promises.push(result.$promise);
                $scope.methods.forEach(function (method) {
                    $scope.scenarios.forEach( function (scenario){
                        result = LciaResultForFragmentService
                            .get({ scenarioID: scenario.scenarioID,
                                lciaMethodID: method.lciaMethodID,
                                fragmentID: fragmentID },
                            extractResult);
                        promises.push(result.$promise);
                    });
                });
                $q.all(promises).then(buildWaterfalls, StatusService.handleFailure);
            }

            function getFragmentScenarios() {
                var scenarios = ScenarioService.getAll(),
                    fragmentScenarios;

                fragmentScenarios = scenarios.filter(function (s) {
                    return (s.topLevelFragmentID === $scope.fragment.fragmentID);
                });
                fragmentScenarios.forEach(function(fs) {
                    fs.selected = true;
                });
                $scope.scenarioSelection.options = fragmentScenarios;
            }

            function getTopLevelFragments() {
                var fragments = FragmentService.getAll(),
                    scenarios = ScenarioService.getAll();
                $scope.fragmentSelection.options = fragments.filter(function (f) {
                    return scenarios.some(function (s) {
                        return s.topLevelFragmentID === f.fragmentID;
                    });
                });
            }

            function getBaseCase() {
                var baseScenario = ScenarioService.get(MODEL_BASE_CASE_SCENARIO_ID);
                if (baseScenario) {
                    $scope.scenarios.push(baseScenario);
                }
            }

            function displayFragmentNavigation() {
                var context = FragmentNavigationService.getContext(),
                    scenario = ScenarioService.get(context.scenarioID),
                    done = false;

                if (scenario) {
                    var fragState = FragmentNavigationService.getLast();
                    $scope.navigationScenario = scenario;
                    if (scenario.scenarioID !== MODEL_BASE_CASE_SCENARIO_ID) {
                        $scope.scenarios.push(scenario);
                    }
                    if (fragState) {
                        $scope.fragment = fragState;
                        fragmentID = $scope.fragment.fragmentID;
                        done = true;
                    } else {
                        $scope.fragment = FragmentService.get(context.fragmentID);
                        if ($scope.fragment) {
                            $scope.fragment.activityLevel = scenario["activityLevel"];
                            FragmentNavigationService.add($scope.fragment);
                            done = true;
                        } else {
                            StatusService.handleFailure("Invalid fragment ID : " + fragmentID);
                        }
                    }
                } else {
                    StatusService.handleFailure("Invalid scenario ID : ", context.scenarioID);
                }
                return done;
            }

            function displayTopLevelFragmentScenarios () {
                var scenarios = ScenarioService.getAll();

                getTopLevelFragments();
                if (scenarios.length > 0) {
                    //
                    // If fragment and scenarios were previously selected,
                    // apply the same selections and display results.
                    // Otherwise, select first fragment and all its scenarios. Wait for user to click button.
                    //
                    if (SelectionService.contains(SELECTION_KEYS.topLevelFragmentID)) {
                        fragmentID = SelectionService.get(SELECTION_KEYS.topLevelFragmentID);
                        $scope.fragmentSelection.options.forEach(function (fs) {
                            fs.selected = (fs.fragmentID === fragmentID);
                        });
                    }
                    else {
                        if ($scope.fragmentSelection.options.length > 1) {
                            var firstFragment = $scope.fragmentSelection.options[0];
                            fragmentID = firstFragment.fragmentID;
                            firstFragment.selected = true;
                        }
                    }
                    $scope.fragment = FragmentService.get(fragmentID);
                    getFragmentScenarios();
                    if (SelectionService.contains(SELECTION_KEYS.fragmentScenarios)) {
                        var selectedScenarios = SelectionService.get(SELECTION_KEYS.fragmentScenarios);
                        $scope.scenarioSelection.options.forEach(function (fs) {
                            fs.selected = selectedScenarios.some(function (s) {
                                return fs.scenarioID === s.scenarioID;
                            });
                        });
                        $scope.scenarios = selectedScenarios;
                    }
                }
            }

            /**
             * Collect methods and scenarios, then get LCIA results.
             */
            function displayLoadedData() {
                var methods = LciaMethodService.getAll();
                StatusService.stopWaiting();
                $scope.methods = methods.filter( function (m) {
                   return m.getIsActive();
                });
                if ( $scope.navigationService) {
                    getBaseCase();
                    if ( displayFragmentNavigation()) {
                        getLciaResults();
                    }
                }
                else {
                    displayTopLevelFragmentScenarios();
                }
            }

            function getStateParams() {
                if ( $stateParams.hasOwnProperty("scenarioID") && $stateParams.scenarioID !== undefined &&
                    $stateParams.hasOwnProperty("fragmentID") && $stateParams.fragmentID !== undefined) {
                    $scope.navigationService = FragmentNavigationService.setContext(+$stateParams.scenarioID,
                        +$stateParams.fragmentID);
                }
            }

            function init() {
                getStateParams();
                if (! $scope.navigationService) {
                    $scope.scenarioSelection = {
                        options: [],
                        model: []
                    };
                    $scope.selectionConfirmed = getSelectionResults;
                    $scope.fragmentSelection = {
                        options: [],
                        model: []
                    };
                }
            }

            /**
             * Get all data resources
             */
            function getData() {
                $q.all([FragmentService.load(), ScenarioService.load(),
                    LciaMethodService.load()
                    ])
                    .then(displayLoadedData,
                    StatusService.handleFailure);
            }

            init();
            StatusService.startWaiting();
            getData();

        }]);