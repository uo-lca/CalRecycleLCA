using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlcdDataLoader
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                SQLXMLBULKLOADLib.SQLXMLBulkLoad4Class objBL = new SQLXMLBULKLOADLib.SQLXMLBulkLoad4Class();
                objBL.ConnectionString = "Provider=SQLNCLI11;Data Source=(localdb)\\Projects;Integrated Security=SSPI;Initial Catalog=LCAToolDev_1";
                objBL.ErrorLogFile = "error.xml";
                objBL.KeepIdentity = false;
                String ilcdFolder = "C:\\ILCD_Data\\ILCD\\";
                String unitgroupDataFile = ilcdFolder + "unitgroups\\1acf7c0f-aa9d-4022-a4b7-fe0b54ea0b91.xml";

                objBL.Execute("unitgroupMap.xml", unitgroupDataFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
