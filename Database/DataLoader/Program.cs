using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LcaDataLoader {
    /// <summary>
    /// Program to batch load ILCD data files into a new database
    /// </summary>
    class Program {
        /// <summary>
        /// Directory holding ILCD data files. Default setting can be overwritten by argument.
        /// </summary>
        static string _IlcdDirName = "C:\\ILCD_Data\\ILCD";


        static void ParseArguments(string[] args) {
            if (args.Length > 0) {
                _IlcdDirName = args[0];
            }
        }

        static void Main(string[] args) {
            ParseArguments(args);
            if (Directory.Exists(_IlcdDirName)) {
                IlcdImporter ilcdImporter = new IlcdImporter();
                ilcdImporter.LoadAll(_IlcdDirName);
            }
            else {
                Console.WriteLine("ERROR: ILCD folder, {0}, does not exist.", _IlcdDirName);
            }
        }
    }
}
