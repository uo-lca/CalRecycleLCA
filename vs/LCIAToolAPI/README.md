Backend - LCA Engine and Web Services
=====================================

Domain logic and web API are implemented here.

Deployment Instructions
-----------------------
1. Build solution (..\CalRecycleLCA.sln), Release configuration
2. Publish project, LCIAToolAPI. A publishing profile must first be configured. FTP is used at UCSB to publish to a test server. The profile is saved as LCIAToolAPI\Properties\PublishProfiles\kbcalr.pubxml.
3. Edit web.config in the publish destination. In the connection string with name=UsedOilLCAContext, change the Data Source to the name of the server hosting a deployed database (see database deployment instructions in ..\..\Database\README).
4. In the deployed database, add user IIS APPPOOL\DefaultAppPool and grant it connect, read, and write privileges to the database.
5. Restart the published web app in IIS.

Usage Instructions
------------------

The URL for the web API is the publish URL + /api/ + resource

Resource routes are defined in [ResourceController.cs](https://github.com/uo-lca/CalRecycleLCA/blob/master/vs/LCIAToolAPI/LCIAToolAPI/API/ResourceController.cs)

Resources are defined in [Models](https://github.com/uo-lca/CalRecycleLCA/tree/master/vs/LCIAToolAPI/Entities/Models)/*Resource.cs

### Output

Get methods return entity properties in json format. Null properties are omitted.

#### Examples

http://kbcalr.isber.ucsb.edu/api/fragments

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

http://kbcalr.isber.ucsb.edu/api/fragments/8/fragmentflows

<pre><code>
[
  {
    "fragmentFlowID": 151,
    "name": "UO_Transfer to DK",
    "nodeTypeID": 3,
    "flowID": 16,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "linkMagnitudes": [
      {
        "flowPropertyID": 23,
        "magnitude": 0.547163
      }
    ]
  },
  {
    "fragmentFlowID": 152,
    "name": "Scenario",
    "nodeTypeID": 1,
    "flowID": 373,
    "directionID": 2,
    "parentFragmentFlowID": 153,
    "processID": 44,
    "linkMagnitudes": [
      {
        "flowPropertyID": 23,
        "magnitude": 1.0
      }
    ]
  },
  {
    "fragmentFlowID": 153,
    "name": "Local Collection Mixer",
    "nodeTypeID": 2,
    "directionID": 2,
    "subFragmentID": 2
  },
  {
    "fragmentFlowID": 154,
    "name": "Used Oil Collected",
    "nodeTypeID": 3,
    "flowID": 860,
    "directionID": 2,
    "parentFragmentFlowID": 153,
    "linkMagnitudes": [
      {
        "flowPropertyID": 12,
        "magnitude": 1.0
      }
    ]
  },
  {
    "fragmentFlowID": 155,
    "name": "Inter-Facility Mixer",
    "nodeTypeID": 2,
    "flowID": 820,
    "directionID": 1,
    "parentFragmentFlowID": 152,
    "subFragmentID": 3,
    "linkMagnitudes": [
      {
        "flowPropertyID": 15,
        "magnitude": 360.985
      }
    ]
  },
  {
    "fragmentFlowID": 156,
    "name": "Lost or Unknown",
    "nodeTypeID": 3,
    "flowID": 346,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "linkMagnitudes": [
      {
        "flowPropertyID": 12,
        "magnitude": 0.007113
      }
    ]
  },
  {
    "fragmentFlowID": 157,
    "name": "Recycled Oil Exported",
    "nodeTypeID": 3,
    "flowID": 699,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "linkMagnitudes": [
      {
        "flowPropertyID": 12,
        "magnitude": 0.101098
      }
    ]
  },
  {
    "fragmentFlowID": 158,
    "name": "Recycled Oil Reprocessed",
    "nodeTypeID": 3,
    "flowID": 475,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "linkMagnitudes": [
      {
        "flowPropertyID": 12,
        "magnitude": 0.916165
      }
    ]
  },
  {
    "fragmentFlowID": 159,
    "name": "UO_Exports",
    "nodeTypeID": 3,
    "flowID": 690,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "linkMagnitudes": [
      {
        "flowPropertyID": 23,
        "magnitude": 0.101098
      }
    ]
  },
  {
    "fragmentFlowID": 160,
    "name": "Waste Oil Preprocessing",
    "nodeTypeID": 2,
    "flowID": 675,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "subFragmentID": 4,
    "linkMagnitudes": [
      {
        "flowPropertyID": 31,
        "magnitude": 0.08368
      }
    ]
  },
  {
    "fragmentFlowID": 161,
    "name": "Waste to Disposal",
    "nodeTypeID": 3,
    "flowID": 351,
    "directionID": 2,
    "parentFragmentFlowID": 160,
    "linkMagnitudes": [
      {
        "flowPropertyID": 12,
        "magnitude": 0.0079496
      }
    ]
  },
  {
    "fragmentFlowID": 162,
    "name": "Transfer Losses",
    "nodeTypeID": 1,
    "flowID": 766,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "processID": 45,
    "linkMagnitudes": [
      {
        "flowPropertyID": 23,
        "magnitude": 0.013594
      }
    ]
  },
  {
    "fragmentFlowID": 163,
    "name": "Lost or Unknown",
    "nodeTypeID": 3,
    "flowID": 346,
    "directionID": 2,
    "parentFragmentFlowID": 162,
    "linkMagnitudes": [
      {
        "flowPropertyID": 12,
        "magnitude": 0.013594
      }
    ]
  },
  {
    "fragmentFlowID": 164,
    "name": "UO_Transfer to Evergreen",
    "nodeTypeID": 3,
    "flowID": 617,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "linkMagnitudes": [
      {
        "flowPropertyID": 23,
        "magnitude": 0.186743
      }
    ]
  },
  {
    "fragmentFlowID": 165,
    "name": "Haz Waste Landfill Output",
    "nodeTypeID": 2,
    "flowID": 649,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "subFragmentID": 5,
    "linkMagnitudes": [
      {
        "flowPropertyID": 23,
        "magnitude": 0.005305
      }
    ]
  },
  {
    "fragmentFlowID": 166,
    "name": "Waste to Disposal",
    "nodeTypeID": 3,
    "flowID": 351,
    "directionID": 2,
    "parentFragmentFlowID": 165,
    "linkMagnitudes": [
      {
        "flowPropertyID": 12,
        "magnitude": 0.005305
      }
    ]
  },
  {
    "fragmentFlowID": 167,
    "name": "Haz Waste Incineration Output",
    "nodeTypeID": 2,
    "flowID": 622,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "subFragmentID": 6,
    "linkMagnitudes": [
      {
        "flowPropertyID": 23,
        "magnitude": 0.001872
      }
    ]
  },
  {
    "fragmentFlowID": 168,
    "name": "Waste to Disposal",
    "nodeTypeID": 3,
    "flowID": 351,
    "directionID": 2,
    "parentFragmentFlowID": 167,
    "linkMagnitudes": [
      {
        "flowPropertyID": 12,
        "magnitude": 0.001872
      }
    ]
  },
  {
    "fragmentFlowID": 169,
    "name": "Wastewater to Treatment",
    "nodeTypeID": 1,
    "flowID": 672,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "processID": 46,
    "linkMagnitudes": [
      {
        "flowPropertyID": 23,
        "magnitude": 0.055951
      }
    ]
  },
  {
    "fragmentFlowID": 170,
    "name": "Waste to Disposal",
    "nodeTypeID": 3,
    "flowID": 351,
    "directionID": 2,
    "parentFragmentFlowID": 169,
    "linkMagnitudes": [
      {
        "flowPropertyID": 12,
        "magnitude": 0.055951
      }
    ]
  },
  {
    "fragmentFlowID": 171,
    "name": "Waste water treatment (contains organic load)",
    "nodeTypeID": 1,
    "flowID": 602,
    "directionID": 2,
    "parentFragmentFlowID": 169,
    "processID": 47,
    "linkMagnitudes": [
      {
        "flowPropertyID": 23,
        "magnitude": 0.055951
      },
      {
        "flowPropertyID": 30,
        "magnitude": 5.5951E-05
      }
    ]
  },
  {
    "fragmentFlowID": 172,
    "name": "Secondary fuel",
    "nodeTypeID": 4,
    "flowID": 421,
    "directionID": 1,
    "parentFragmentFlowID": 171,
    "linkMagnitudes": [
      {
        "flowPropertyID": 24,
        "magnitude": 8.18265E-08
      }
    ]
  },
  {
    "fragmentFlowID": 173,
    "name": "Secondary fuel renewable",
    "nodeTypeID": 4,
    "flowID": 683,
    "directionID": 1,
    "parentFragmentFlowID": 171,
    "linkMagnitudes": [
      {
        "flowPropertyID": 24,
        "magnitude": 7.84079E-09
      }
    ]
  },
  {
    "fragmentFlowID": 174,
    "name": "Used Oil Rejuvenation/Other",
    "nodeTypeID": 1,
    "flowID": 619,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "processID": 48,
    "linkMagnitudes": [
      {
        "flowPropertyID": 23,
        "magnitude": 0.009339
      }
    ]
  },
  {
    "fragmentFlowID": 175,
    "name": "Rejuvenated Dielectric fluid",
    "nodeTypeID": 3,
    "flowID": 651,
    "directionID": 2,
    "parentFragmentFlowID": 174,
    "linkMagnitudes": [
      {
        "flowPropertyID": 12,
        "magnitude": 0.009339
      }
    ]
  },
  {
    "fragmentFlowID": 176,
    "name": "UO_Transfer to RFO",
    "nodeTypeID": 3,
    "flowID": 808,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "linkMagnitudes": [
      {
        "flowPropertyID": 23,
        "magnitude": 0.071822
      }
    ]
  },
  {
    "fragmentFlowID": 177,
    "name": "UO water fraction",
    "nodeTypeID": 1,
    "flowID": 782,
    "directionID": 2,
    "parentFragmentFlowID": 152,
    "processID": 49,
    "linkMagnitudes": [
      {
        "flowPropertyID": 31,
        "magnitude": 0.175463
      }
    ]
  },
  {
    "fragmentFlowID": 178,
    "name": "Collected UO Water Content",
    "nodeTypeID": 3,
    "flowID": 655,
    "directionID": 2,
    "parentFragmentFlowID": 177,
    "linkMagnitudes": [
      {
        "flowPropertyID": 12,
        "magnitude": 0.175463
      }
    ]
  },
  {
    "fragmentFlowID": 179,
    "name": "Used Oil, for collection",
    "nodeTypeID": 3,
    "flowID": 446,
    "directionID": 2,
    "parentFragmentFlowID": 177,
    "linkMagnitudes": [
      {
        "flowPropertyID": 23,
        "magnitude": 0.175463
      }
    ]
  }
]
</pre></code>



