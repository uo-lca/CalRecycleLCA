using System;
using System.Collections.Generic;
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

                    
                        
                    LCIAMethod_DUPE lciaMethod = new LCIAMethod_DUPE();
                    lciaMethod.UUID = xDoc.Root
                        .Descendants(common + "UUID")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.Version = xDoc.Root
                        .Descendants(common + "dataSetVersion")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString());
                    lciaMethod.LCIAMethod = xDoc.Root
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

                   
                    List<LCIA_DUPE> lciaList = xDoc.Root.Descendants(df + "characterisationFactors").Elements(df + "factor").Select(d =>
                    new LCIA_DUPE
                    {
                         UUID = xDoc.Root
                        .Descendants(common + "UUID")
                        .Select(s => s.Value)
                        .Aggregate(
                            new StringBuilder(),
                            (s, i) => s.Append(i),
                            s => s.ToString()),
                        Flow = d.Element(df + "referenceToFlowDataSet").Attribute("refObjectId").Value,
                        Factor = float.Parse(d.Element(df + "meanValue").Value, System.Globalization.CultureInfo.InvariantCulture),
                        Direction = d.Element(df + "exchangeDirection").Value,
                    }).ToList();

                  

                    ILCD_matlab_Entities ent = new ILCD_matlab_Entities();

                    ent.LCIAMethod_DUPE.Add(lciaMethod);
                   
                    foreach (var i in lciaList)
                    {
                        ent.LCIA_DUPE.Add(i);
                    }
                    ent.SaveChanges();

                    lblMessage.Text = "Import Done successfully!";
                }
                catch (Exception)
                {
                    lblMessage.Text = "Import was not successful";
                    //throw;
                }
            }
        }
    }
}