CalRecycleLCA
=============

CalRecycle Used Oil LCA Online Tool

Web API Specification
=====================



### lcia_impact_categories


Lists all Impact Categories.

Returns:

 - ImpactCategoryID
 - Name

### lcia_methods

Lists all LCIA methods, optionally  within an impact category.

Parameters:

 * ImpactCategoryID  (filter)

Returns:

 - LCIAMethodID
 - Name
 - FlowPropertyID.Name
 - UnitGroup.ReferenceUnit

### processes

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


### process_flows

Reports intermediate flows passing through the process.

Parameters:

 * ProcessID
 * (optional) Balance = 0|1 (default 0)

Returns:

 - ProcessFlowID
 - Flow.Name
 - Flow Direction (Input or Output) (or DirectionID)
 - ProcessFlow.Result
 - FlowProperty.Name (on Flow.FlowPropertyID)
 - UnitGroup.ReferenceUnit (on FlowProperty.UnitGroupID)
 - WHERE FlowType != 'Elementary Flow'

If Balance=1, leave out ProcessFlowID, Flow.Name; compute sum of
ProcessFlow.Result, grouping by reference flow property and unit. This is
tricky because "Input" and "Output" must map to +1 and -1 respectively and
multiplied by the flow quantities.

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
