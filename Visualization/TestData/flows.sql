SELECT FlowID,
    concat('"', Name, '"') as Name,
    --Name
    ISNULL(CASNumber, '') as CASNumber,
    FlowTypeID,
    ReferenceFlowProperty AS ReferenceFlowPropertyID
FROM Flow f 