using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLHandler
{
    public class FlowTestService : IFlowTestService
    {
        //public void ViewXML()
        //{
        //    XmlDocument xmldoc = new XmlDocument();
        //    //XmlNodeList xmlnode;
        //    //int i = 0;
        //    //string str = null;
        //    //FileStream fs = new FileStream("C:/Users/Rachel/Source/calrecycle-repos/LCA_Data/Full UO LCA Flat Export BK/ILCD/flows/00a22787-7c52-4b7c-be06-b71c3c60f97a.xml", FileMode.Open, FileAccess.Read);
        //    //xmldoc.Load(fs);
        //    //xmlnode = xmldoc.GetElementsByTagName("flowDataSet");
        //    //for (i = 0; i <= xmlnode.Count - 1; i++)
        //    //{
        //    //    xmlnode[i].ChildNodes.Item(0).InnerText.Trim();
        //    //    str = xmlnode[i].ChildNodes.Item(0).InnerText.Trim() + "  " + xmlnode[i].ChildNodes.Item(1).InnerText.Trim() + "  " + xmlnode[i].ChildNodes.Item(2).InnerText.Trim();
                
        //    //}

        //    //return str;
        //    xmldoc.Load("C:/Users/Rachel/Source/calrecycle-repos/LCA_Data/Full UO LCA Flat Export BK/ILCD/flows/00a22787-7c52-4b7c-be06-b71c3c60f97a.xml");
        //}

        public XmlDocument ViewXML()
        {
            XmlDocument xmlDocument = null;

            xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load("C:/Users/Rachel/Source/calrecycle-repos/LCA_Data/Full UO LCA Flat Export BK/ILCD/flows/00a22787-7c52-4b7c-be06-b71c3c60f97a.xml");

            return xmlDocument;
        }
    }
}
