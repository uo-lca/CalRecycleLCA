using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;

namespace LcaDataLoader {
    /// <summary>
    /// Program to create and seed database. Loads ILCD data files.
    /// </summary>
    class Program {
        /// <summary>
        /// Directory holding ILCD data files. Default setting can be overwritten by argument.
        /// </summary>
        static string _IlcdDirName = "C:\\CalRecycleLCA-DATA_ROOT\\Full UO LCA Flat Export BK 2014_05_05\\ILCD";
        static string _logFileName = "C:\\CalRecycleLCA-DATA_ROOT\\LcaDataLoaderLog.txt";
        static StreamWriter _LogWriter = null;

        static void ParseArguments(string[] args) {
            if (args.Length > 0) {
                _IlcdDirName = args[0];
            }
            if (args.Length > 1) {
                _logFileName = args[1];
            }
        }

        /// <summary>
        /// Redirect console output to file with name, _logFileName.
        /// </summary>
        static void StartLogging() {
            try {
                // Attempt to open output file.
                _LogWriter = new StreamWriter(_logFileName);
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
                Database.SetInitializer<EntityDataModel>(new DbInitializer());
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
