select ff.FragmentFlowID, concat('"', ff.Name, '"') as Name, ff.FlowID, ff.ReferenceFlowPropertyID, ff.NodeTypeID, ff.DirectionID, ff.Quantity, ff.ParentFragmentFlowID,
fnp.ProcessID as NodeID
from
[dbo].[FragmentFlow] ff 
inner join [dbo].[FragmentNodeProcess] fnp on ff.FragmentFlowID = fnp.FragmentFlowID
where FragmentID = 2 and ff.NodeTypeID = 1
union
select ff.FragmentFlowID, concat('"', ff.Name, '"') as Name, ff.FlowID, ff.ReferenceFlowPropertyID, ff.NodeTypeID, ff.DirectionID, ff.Quantity, ff.ParentFragmentFlowID,
fnf.SubFragmentID as NodeID
from
[dbo].[FragmentFlow] ff 
inner join [dbo].[FragmentNodeFragment] fnf on ff.FragmentFlowID = fnf.FragmentFlowID
where FragmentID = 2 and ff.NodeTypeID = 2
union
select ff.FragmentFlowID, concat('"', ff.Name, '"') as Name, ff.FlowID, ff.ReferenceFlowPropertyID, ff.NodeTypeID, ff.DirectionID, ff.Quantity, ff.ParentFragmentFlowID,
0 as NodeID
from
[dbo].[FragmentFlow] ff 
where FragmentID = 2 and ff.NodeTypeID not in (1,2)