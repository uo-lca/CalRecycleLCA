using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using LcaDataModel;

namespace LcaDataLoader {
    /// <summary>
    /// Program to create and seed database. Loads ILCD data files.
    /// </summary>
    class Program {
        /// <summary>
        /// Directory holding ILCD data files. Default setting can be overwritten by argument.
        /// </summary>
        static string _IlcdDirName;
        static string _LogFileName;
        static bool _DeleteFlag;
        static StreamWriter _LogWriter = null;

        static void ParseArguments(string[] args) {
            string dataRoot = "C:\\CalRecycleLCA-DATA_ROOT";
            string ilcdSourceName = "Full UO LCA Flat Export BK 2014_05_05";
            _LogFileName = "C:\\CalRecycleLCA-DATA_ROOT\\Full UO LCA Flat Export BK 2014_05_05\\LcaDataLoaderLog.txt";
            _DeleteFlag = false;
            OptionSet options = new OptionSet() {
                {"r|root=", "The full {DATA_ROOT} path.", v => dataRoot = v },
                {"s|source=", "ILCD archive {source name}.", v => ilcdSourceName = v },
                {"d|delete", "Delete database and recreate.", v => _DeleteFlag = (v!=null)},
                {"l|log=", "Redirect output to {log file}.", v => _LogFileName = v }
            };
            List<string> extraArgs;
            try {
                extraArgs = options.Parse(args);
            }
            catch (OptionException e) {
                Console.Write("Usage Error: ");
                Console.WriteLine(e.Message);
            }
            _IlcdDirName = Path.Combine(dataRoot, ilcdSourceName, "ILCD");            
        }

        /// <summary>
        /// Redirect console output to file with name, _LogFileName.
        /// </summary>
        static void StartLogging() {
            try {
                // Attempt to open output file.
                _LogWriter = new StreamWriter(_LogFileName);
                _LogWriter.AutoFlush = true;
                // Redirect standard output from the console to the output file.
                Console.SetOut(_LogWriter);
            }
            catch (IOException e) {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Redirect console output to stdout.
        /// </summary>
        static void StopLogging() {
            _LogWriter.Close();
            // Recover the standard output stream so that a  
            // completion message can be displayed.
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
        }

        /// <summary>
        /// Entry point of console app.
        /// </summary>
        static int Main(string[] args) {
            int exitCode = 0;
            try {
                ParseArguments(args);
                StartLogging();
                if (_DeleteFlag) {
                    Database.SetInitializer<EntityDataModel>(new DropCreateDatabaseInitializer());
                }
                if (Directory.Exists(_IlcdDirName)) {
                    IlcdImporter ilcdImporter = new IlcdImporter();
                    ilcdImporter.LoadAll(_IlcdDirName);
                    Console.WriteLine("SUCCESS: Loaded ILCD archive from {0}.", _IlcdDirName);
                }
                else {
                    Console.WriteLine("ERROR: ILCD folder, {0}, does not exist.", _IlcdDirName);
                    exitCode = 1;
                }
            }
            catch (Exception e) {
                Console.WriteLine("ERROR: Exception Message = {0}", e.Message);
                Console.Write(e.ToString());
                exitCode = 1;
            }
            finally {
                StopLogging();
            }
            return exitCode;
        }
    }
}
