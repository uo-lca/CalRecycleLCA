CalRecycleLCA
=============
 
 * auto-gen TOC:
{:toc}

CalRecycle Used Oil LCA Online Tool

Web API Specification
=====================

### Classes ###

Lists known classification categories, optionally filtered

Parameters (all optional):

 * DataType - only return values for named DataType
 * Level - 0, 1, 2... only return values with named hierarchical level
 * *DataSourceID - filter results by data source ID (once implemented)*

Note: Before this will work well, I need to provide a new list of
classification data to replace both `Class` and
`Classification` tables.

Note 2: `Category` table can probably go away- just stick
ParentClassID and HierarchyLevel fields onto `Class` table.  I
will plan to do this when I supply replacement tables.

Returns:

 - ClassificationID
 - CategorySystem
 - DataType.Name (via CategorySystem) AS DataType
 - ClassID
 - Class.Name AS ClassName
 - Class.ExternalClassID
 - Category.HierarchyLevel AS Level

#### Status ####

None (has not been reviewed nor assigned to a milestone).

### LciaImpactCategories ###


Lists all Impact Categories.

Returns:

 - ImpactCategoryID
 - Name

#### Status ####

Implemented in sprint 1. The above spec has changed since then.

Current resource name: ImpactCategory

Returns: same as above

Published at http://rachelscanlon.com/api/impactcategory

##### Issues #####

Spec resource name differs from implemented resource name. Need to decide what is our resource naming convention.

Question: Would it make sense to group LCIA resources like this:

- LCIA/ImpactCategories
- LCIA/Methods

etc.


### LciaMethods ###

Lists all LCIA methods, optionally  within an impact category.

Parameters:

 * ImpactCategoryID  (filter)

Returns:

 - LCIAMethodID
 - Name
 - FlowPropertyID.Name
 - UnitGroup.ReferenceUnit

 
#### Status ####

Implemented in sprint 1. The above spec has changed since then.

Current resource name: LCIAMethod

Parameters: Same as above.

Returns: 

- LCIAMethodID
- Name

Published at http://rachelscanlon.com/api/lciamethod.

##### Issues #####

Same naming convention issue as previous resource.

The 2 new output fields are not attributes of LCIA Method. 
Units are needed for the LCIA Computation visualization. Not sure this is the best way to provide them.

### Processes ###

Lists all processes in the database

Parameters:

 * Flows = 0, 1, 2 (optional)

  - 0 / 'All'  or omitted - list all processes
  - 1 / 'Elementary' - list processes with elementary flows
  - 2 / 'Model' - list processes that have no elementary flows

Returns:

 - ProcessID
 - Name

For Flows=1, the following query:

    SELECT   Process.ProcessID, Process.Name
	FROM [LCAToolDev].[dbo].[ProcessFlow]
	INNER JOIN Process on ProcessFlow.ProcessID = Process.ProcessID
	INNER JOIN Flow on ProcessFlow.FlowID = Flow.FlowID
	INNER JOIN FlowType on Flow.FlowTypeID = FlowType.FlowTypeID
	WHERE FlowType.Type = 'Elementary Flow'
	GROUP BY Process.ProcessID, Process.Name
	ORDER BY Process.Name

For Flows=2 we need processes that do not match Flows=1:

    select * from 
	( SELECT  count([ProcessFlowID]) AS emcount
      ,  ProcessID
	FROM [LCAToolDev].[dbo].[ProcessFlow]
	INNER JOIN Flow on ProcessFlow.FlowID = Flow.FlowID
	INNER JOIN FlowType on Flow.FlowTypeID = FlowType.FlowTypeID
	WHERE FlowType.Type = 'Elementary Flow'
	GROUP BY ProcessID, FlowType.Type
	) emissions
	RIGHT JOIN Process on emissions.ProcessID = Process.ProcessID
	WHERE emcount is null

#### Status ####

Implemented in sprint 1. The above spec has changed since then.

##### Current Implementation #####

Resource name: Process

Parameters: None

Returns: 

- ProcessID
- Name

Published at http://rachelscanlon.com/api/process

##### Issues #####

Naming convention : should resource name be singular or plural?


### Flows ###

Lists all flows.

Parameters:

 * FlowType = Elementary or Intermediate (optional??)
 * ClassID (optional) only list flows matching ClassID

Returns:

 - FlowID
 - Flow.Name
 - CASNumber
 - ReferenceProperty
 - ReferenceUnit

#### Status ####

None (has not been reviewed nor assigned to a milestone).

### FlowProperties ###

Reports (and future: allows to edit) flow properties associated with a
given intermediate flow.

Parameters:

 * FlowID

Returns (from `FlowFlowProperty` table unless otherwise specified):

 * FlowFlowPropertyID
 * FlowPropertyID
 * MeanValue
 * StDev
 * UnitGroup.ReferenceUnit

#### Status ####

None (has not been reviewed nor assigned to a milestone).

### IntermediateFlows ###

Reports intermediate flows passing through a process.

Parameters:

 * ProcessID
 * (optional) Balance = 0|1 (default 0)

Main Query:

    ( SELECT
	    pf.ProcessFlowID,
        f.Name AS FlowName,
		d.Name AS Direction,
		CASE 
         WHEN d.Name = 'Input' THEN +1 
         WHEN d.Name = 'Output' THEN -1 
	    END AS DirFlag, 
		pf.Result AS Quantity,
		fp.Name AS ReferenceProperty,
		ug.ReferenceUnit AS ReferenceUnit
	FROM ProcessFlow pf
	INNER JOIN Flow f on pf.FlowID = f.FlowID
	INNER JOIN Direction d on pf.DirectionID = d.DirectionID
	INNER JOIN FlowType ft on f.FlowTypeID = ft.FlowTypeID
	INNER JOIN FlowProperty fp on f.FlowPropertyID = fp.FlowPropertyID
	INNER JOIN UnitGroup ug on fp.UnitGroupID = ug.UnitGroupID

	WHERE ft.FlowTypeID <> 2
	and pf.ProcessID = @param
	) Main

selects all non-elementary flows, with ancillary information, for a given
process ID.

#### Balance = 0 : List Flows for Sankey diagram ####

Returns:

 * ProcessFlowID
 * FlowName
 * FlowDirection
 * Quantity
 * ReferenceProperty
 * ReferenceUnit
 * SankeyWidth

The flow quantities need to be normalized on a by-Reference-Unit basis in
order to scale properly in the visualization.  To this we need to add a
`SankeyWidth` column which is derived from the largest flow in
any given unit: 

    ( SELECT distinct
		max(Quantity),
		ReferenceUnit
	FROM Main
	GROUP BY ReferenceUnit ) MaxUnit

Now, we return:

    IF Balance == 0:

	SELECT ProcessFlowID,
		FlowName,
		FlowDirection,
		Quantity,
		ReferenceProperty,
		ReferenceUnit,
		Quantity / UnitMax AS SankeyWidth
	FROM Main
	INNER JOIN MaxUnit on Main.ReferenceUnit = MaxUnit.ReferenceUnit

#### Balance = 1 : List Net flow results by unit ####

Returns:

 * ReferenceProperty
 * ReferenceUnit
 * NetFlowIn

To do this, we need to multiply the quantities by the direction flag
created above and sum:

    else // Balance == 1

	SELECT ReferenceProperty,
		ReferenceUnit,
		sum( Quantity * DirFlag ) AS NetFlowIn
	FROM Main
	GROUP BY ReferenceProperty,ReferenceUnit

I don't know how to do this in plain SQL so someone will have to check it.

#### Status ####

Implemented in sprint 2.

Published URL: http://rachelscanlon.com/api/intermediateflow

### lcia_compute

Computes LCIA score for the selected process using the selected LCIA
method. 

Parameters:

 * LCIAMethodID
 * ProcessID

Returns:

 - FlowID
 - FlowName
 - LCIA.Factor
 - ProcessFlow.Result as Quantity
 - Quantity * Factor as LCIAResult
 - [ future: Elementary flow classification and/or classID ]
 
Reference query (null geograpy):

    SELECT ProcessID,
	LCIAMethodID,
	LCIA.FlowID,
	LCIA.DIrectionID,
	Result as Quantity,
	Factor,
	Factor * Result as Score
	from [dbo].ProcessFlow
	INNER JOIN LCIA ON ProcessFlow.FlowID = LCIA.FlowID
	where ProcessFlow.ProcessID = @ProcessID
		and LCIA.LCIAMethodID = @LCIAMEthod
		and ProcessFlow.DirectionID=LCIA.DirectionID
		and LCIA.Geography IS NULL
	order by Quantity

Geography-specific query requires a hierarchical "is parent of" test to
determine the best LCIA factor for a given process-flow geography
specification. [future]

#### Status ####

Implemented in sprint 1. Above spec has changed since then.

##### Current Implementation #####

Resource name: LCIAComputation
 
 Parameters:

 * LCIAMethodID (optional)
 * ProcessID    (optional)
 * ImpactCategoryID (optional)

Returns:

 - ProcessName 
 - FlowID
 - Flow
 - Direction
 - Quantity
 - STDev
 - Factor
 - LCIAResult

Published URL : http://rachelscanlon.com/api/LCIAComputation

##### Issues #####

Same naming convention issues as above.

Should parameters be optional? If so, more return fields are needed. 
If LCIAMethodID is not provided, how can the client determine which results apply to which methods?

Visualization needs unit - that is a flow property. There is currently no web api resource for Flow Properties.

