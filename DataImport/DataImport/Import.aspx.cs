using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace DataImport
{

    public partial class Import : System.Web.UI.Page
    {

        
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnImport_Click(object sender, EventArgs e)
        {

            

            if (FileUpload1.PostedFile.ContentType == "application/xml" || FileUpload1.PostedFile.ContentType == "text/xml")
            {
                try
                {
                    string fileName = Path.Combine(Server.MapPath("~/UploadDocuments"), Guid.NewGuid().ToString() + ".xml");
                    FileUpload1.PostedFile.SaveAs(fileName);

                    XDocument xDoc = XDocument.Load(fileName);
                    XNamespace df = xDoc.Root.Name.Namespace;
                    XNamespace common = "http://lca.jrc.it/ILCD/Common";

                    
                        
                    LCIAMethod lciaMethod = new LCIAMethod();
                    lciaMethod.LCIAMethodUUID = xDoc.Root
                        .Descendants(common + "UUID")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.LCIAMethodVersion = xDoc.Root
                        .Descendants(common + "dataSetVersion")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.LCIAMethod1 = xDoc.Root
                        .Descendants(common + "name")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.Methodology = xDoc.Root
                        .Descendants(df + "methodology")
                        .Select(s => s.Value + " ")
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString().TrimEnd());
                    lciaMethod.ImpactCategory = xDoc.Root
                        .Descendants(df + "impactCategory")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.ImpactIndicator = xDoc.Root
                        .Descendants(df + "impactIndicator")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.ReferenceYear = xDoc.Root
                        .Descendants(df + "referenceYear")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.Duration = xDoc.Root
                        .Descendants(df + "duration")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.ImpactLocation = xDoc.Root
                        .Descendants(df + "impactLocation")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.IndicatorType = xDoc.Root
                        .Descendants(df + "typeOfDataSet")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.Normalization = Convert.ToBoolean(xDoc.Root
                        .Descendants(df + "normalisation")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString()));
                    lciaMethod.Weighting = Convert.ToBoolean(xDoc.Root
                        .Descendants(df + "weighting")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString()));
                    lciaMethod.UseAdvice = xDoc.Root
                        .Descendants(df + "useAdviceForDataSet")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.Source = xDoc.Root
                        .Descendants(df + "referenceToModelSource")
                        .Attributes("uri")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.ReferenceQuantity = xDoc.Root
                        .Descendants(df + "referenceQuantity")
                        .Attributes("uri")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());

                    LCAToolDevEntities ent = new LCAToolDevEntities();

                    
                    List<Flow> flows = new List<Flow>();
                    flows = ent.Flows.ToList();

                    List<Direction> directions = new List<Direction>();
                    directions = ent.Directions.ToList();

                    List<LCIA> lciaList = xDoc.Root.Descendants(df + "characterisationFactors").Elements(df + "factor").Select(d =>
                    new LCIA
                    {
                         LCIAUUID = xDoc.Root
                        .Descendants(common + "UUID")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString()), 
                        Flow_SQL = d.Element(df + "referenceToFlowDataSet").Attribute("refObjectId").Value,
                        Factor = float.Parse(d.Element(df + "meanValue").Value, System.Globalization.CultureInfo.InvariantCulture),
                        Direction_SQL = d.Element(df + "exchangeDirection").Value,
                         LCIAFlowID = flows.Where(f => f.FlowUUID == d.Element(df + "referenceToFlowDataSet").Attribute("refObjectId").Value)
                         .Select(f => f.FlowID).FirstOrDefault(),
                         LCIADirectionID = directions.Where(dir => dir.Direction1 == d.Element(df + "exchangeDirection").Value)
                         .Select(dir => dir.DirectionID).FirstOrDefault(),
                        
                    }).ToList();

                  


                    ent.LCIAMethods.Add(lciaMethod);

                    //check for EntityValidationErrors before saving
                    try
                    {
                        ent.SaveChanges();
                    }
                    catch (DbEntityValidationException dbValEx)
                    {
                        var outputLines = new StringBuilder();
                        foreach (var eve in dbValEx.EntityValidationErrors)
                        {
                            outputLines.AppendFormat("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:"
                              , DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State);

                            foreach (var ve in eve.ValidationErrors)
                            {
                                outputLines.AppendFormat("- Property: \"{0}\", Error: \"{1}\""
                                 , ve.PropertyName, ve.ErrorMessage);
                            }
                        }

                        throw new DbEntityValidationException(string.Format("Validation errors\r\n{0}"
                         , outputLines.ToString()), dbValEx);
                    }

                    int lciaMethodId = lciaMethod.LCIAMethodID;
                    Response.Write(lciaMethodId);


                    foreach (var i in lciaList)
                    {
                        ent.LCIAs.Add(i);
                        i.LCIAMethodID = lciaMethodId;     
                    }

                    //check for EntityValidationErrors before saving
                    try
                    {
                        ent.SaveChanges();
                    }
                    catch (DbEntityValidationException dbValEx)
                    {
                        var outputLines = new StringBuilder();
                        foreach (var eve in dbValEx.EntityValidationErrors)
                        {
                            outputLines.AppendFormat("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:"
                              , DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State);

                            foreach (var ve in eve.ValidationErrors)
                            {
                                outputLines.AppendFormat("- Property: \"{0}\", Error: \"{1}\""
                                 , ve.PropertyName, ve.ErrorMessage);
                            }
                        }

                        throw new DbEntityValidationException(string.Format("Validation errors\r\n{0}"
                         , outputLines.ToString()), dbValEx);
                    }
                    

                    lblMessage.Text = "Import Done successfully!";
                }
                catch (Exception)
                {
                    lblMessage.Text = "Import was not successful";
                    throw;
                }
            }
        }

        protected void btnProcessImport_Click(object sender, EventArgs e)
        {
            if (uplProcess.PostedFile.ContentType == "application/xml" || uplProcess.PostedFile.ContentType == "text/xml")
            {
                try
                {
                    string fileName = Path.Combine(Server.MapPath("~/UploadDocuments/Process"), Guid.NewGuid().ToString() + ".xml");
                    uplProcess.PostedFile.SaveAs(fileName);

                    XDocument xDoc = XDocument.Load(fileName);
                    XNamespace p = xDoc.Root.Name.Namespace;
                    XNamespace common = "http://lca.jrc.it/ILCD/Common";
                    XNamespace gabi = "http://www.pe-international.com/GaBi";



                    Process process = new Process();
                    process.ProcessUUID = xDoc.Root
                        .Descendants(common + "UUID")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    process.ProcessVersion= xDoc.Root
                        .Descendants(common + "dataSetVersion")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    process.Process1 = xDoc.Root
                        .Descendants(p + "baseName")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    process.Year = xDoc.Root
                         .Descendants(common + "referenceYear")
                         .Select(s => s.Value)
                         .Aggregate(
                             new StringBuilder(),
                             (s, i) => s.Append(i),
                             s => s.ToString());
                    process.Geography = xDoc.Root
                       .Descendants(p + "locationOfOperationSupplyOrProduction")
                       .Attributes("location")
                       .Select(s => s.Value)
                       .Aggregate(
                           new StringBuilder(),
                           (s, i) => s.Append(i),
                           s => s.ToString());
                    //remove until we figure out why it is choosing an abitrary flow id from one of the several processflows
                    //process.ReferenceFlow_SQL = xDoc.Root
                    //    .Descendants(df + "impactIndicator")
                    //    .Select(s => s.Value)
                    //    .Aggregate(
                    //        new StringBuilder(),
                    //        (s, i) => s.Append(i),
                    //        s => s.ToString());
                    process.RefererenceType = xDoc.Root
                       .Descendants(p + "quantitativeReference")
                       .Attributes("type")
                       .Select(s => s.Value)
                       .Aggregate(
                           new StringBuilder(),
                           (s, i) => s.Append(i),
                           s => s.ToString());
                    process.ProcessType = xDoc.Root
                        .Descendants(p + "typeOfDataSet")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    //left out for now because only one process has this jpg file and it's in a weird location
                    //process.Diagram = xDoc.Root
                    //     .Descendants(common + "shortDescription")
                    //     .Select(s => s.Value)
                    //     .Aggregate(
                    //         new StringBuilder(),
                    //         (s, i) => s.Append(i),
                    //         s => s.ToString());

                    LCAToolDevEntities ent = new LCAToolDevEntities();

                    List<Flow> flows = new List<Flow>();
                    flows = ent.Flows.ToList();

                    List<Direction> directions = new List<Direction>();
                    directions = ent.Directions.ToList();

                    List<ProcessFlow> processFlowList = xDoc.Root.Descendants(p + "exchanges").Elements(p+"exchange").Select(d =>
                    new ProcessFlow
                    {
                        ProcessUUID = process.ProcessUUID,
                        ProcessFlowType = (string)d.Element(p + "referenceToVariable"),
                        VarName = (string)d.Element(common + "other").Element(gabi + "GaBi").Attribute("IOType"),
                       Magnitude = (double)d.Element(p + "meanAmount"),
                       // //Not sure where this value comes from.  Need to ask Brandon.
                       // //Result = d.Element(df + "meanAmount").Value,
                       STDev = (double)d.Element(p + "relativeStandardDeviation95In"),
                       Flow_SQL = (string)d.Element(p + "referenceToFlowDataSet").Attribute("refObjectId"),
                       Direction_SQL = (string)d.Element(p + "exchangeDirection"),
                       Geography = (string)d.Element(p + "location"),
                        ProcessFlowFlowID = flows.Where(f => f.FlowUUID == d.Element(p + "referenceToFlowDataSet").Attribute("refObjectId").Value)
                         .Select(f => f.FlowID).FirstOrDefault(),
                        ProcessFlowDirectionID = directions.Where(dir => dir.Direction1 == d.Element(p + "exchangeDirection").Value)
                        .Select(dir => dir.DirectionID).FirstOrDefault(),

                    }).ToList();




                    ent.Processes.Add(process);

                    //check for EntityValidationErrors before saving
                    try
                    {
                        ent.SaveChanges();
                    }
                    catch (DbEntityValidationException dbValEx)
                    {
                        var outputLines = new StringBuilder();
                        foreach (var eve in dbValEx.EntityValidationErrors)
                        {
                            outputLines.AppendFormat("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:"
                              , DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State);

                            foreach (var ve in eve.ValidationErrors)
                            {
                                outputLines.AppendFormat("- Property: \"{0}\", Error: \"{1}\""
                                 , ve.PropertyName, ve.ErrorMessage);
                            }
                        }

                        throw new DbEntityValidationException(string.Format("Validation errors\r\n{0}"
                         , outputLines.ToString()), dbValEx);
                    }

                    int processId = process.ProcessID;
                    Response.Write(processId);


                    foreach (var i in processFlowList)
                    {
                        ent.ProcessFlows.Add(i);
                        i.ProcessFlowProcessID = processId;
                    }

                    //check for EntityValidationErrors before saving
                    try
                    {
                        ent.SaveChanges();
                    }
                    catch (DbEntityValidationException dbValEx)
                    {
                        var outputLines = new StringBuilder();
                        foreach (var eve in dbValEx.EntityValidationErrors)
                        {
                            outputLines.AppendFormat("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:"
                              , DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State);

                            foreach (var ve in eve.ValidationErrors)
                            {
                                outputLines.AppendFormat("- Property: \"{0}\", Error: \"{1}\""
                                 , ve.PropertyName, ve.ErrorMessage);
                            }
                        }

                        throw new DbEntityValidationException(string.Format("Validation errors\r\n{0}"
                         , outputLines.ToString()), dbValEx);
                    }


                    lblProcessUploadError.Text = "Import Done successfully!";
                }
                catch (Exception)
                {
                    lblProcessUploadError.Text = "Import was not successful";
                    throw;
                }
            }
        }
    }
}