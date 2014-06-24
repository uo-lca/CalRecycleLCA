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
using log4net;
using log4net.Config;

namespace LcaDataLoader {
    /// <summary>
    /// Program to create and seed database. Loads ILCD data files.
    /// </summary>
    class Program {
        /// <summary>
        /// Directory holding ILCD data files. Default setting can be overwritten by argument.
        /// </summary>
        static string _DataRoot;
        static string _IlcdDirName = null;
        static string _IlcdSourceName = null;
        static bool _DeleteFlag;
        static bool _CsvFlag;

        public static readonly ILog Logger = LogManager.GetLogger("LcaDataLoader");

        static void ParseArguments(string[] args) {
            _DataRoot = "C:\\CalRecycleLCA-DATA_ROOT";
            _DeleteFlag = false;
            _CsvFlag = false;

            OptionSet options = new OptionSet() {
                {"r|root=", "The full {DATA_ROOT} path.", v => _DataRoot = v },
                {"s|source=", "ILCD archive {source name}.", v => _IlcdSourceName = v },
                {"c|csv", "Load CSV files.", v => _CsvFlag = (v!=null)}, 
                {"d|delete", "Delete database and recreate.", v => _DeleteFlag = (v!=null)} 
            };
            List<string> extraArgs;
            try {
                extraArgs = options.Parse(args);
                Logger.InfoFormat("Data root={0}, Load CSVs={1}, Delete={2}", _DataRoot, _CsvFlag.ToString(), _DeleteFlag.ToString());
                if (_IlcdSourceName != null)
                    Logger.InfoFormat("ILCD source={0}", _IlcdSourceName);
            }
            catch (OptionException e) {
                Logger.ErrorFormat("Usage Error: {0}", e.Message);
            }
            if (!String.IsNullOrEmpty(_IlcdSourceName)) {
                _IlcdDirName = Path.Combine(_DataRoot, _IlcdSourceName, "ILCD");
            }
        }

        /// <summary>
        /// Initialize logging.
        /// </summary>
        static void StartLogging() {
            // Configured in App.config
            BasicConfigurator.Configure();
            Logger.Info("****** START LOG ******");
        }

        static void StopLogging() {
            Logger.Info("******* END LOG *******");
        }

        /// <summary>
        /// Entry point of console app.
        /// </summary>
        static int Main(string[] args) {
            int exitCode = 0;
            try {
                StartLogging();
                ParseArguments(args);
                if (_DeleteFlag) {
                    Database.SetInitializer<EntityDataModel>(new DropCreateDatabaseInitializer());
                }
                if (!String.IsNullOrEmpty(_IlcdDirName)) {
                    if (Directory.Exists(_IlcdDirName)) {
                        IlcdImporter ilcdImporter = new IlcdImporter();
                        ilcdImporter.LoadAll(_IlcdDirName, _IlcdSourceName);
                        Logger.InfoFormat("Loaded ILCD archive from {0}.", _IlcdDirName);
                    }
                    else {
                        Logger.ErrorFormat("ILCD folder, {0}, does not exist.", _IlcdDirName);
                        exitCode = 1;
                    }
                }
                if (_CsvFlag) {
                    if (Directory.Exists(_DataRoot)) {
                        CsvImporter.LoadAll(_DataRoot);
                        Logger.InfoFormat("Loaded CSV folders under {0}.", _DataRoot);
                    }
                    else {
                        Logger.ErrorFormat("Data Root folder, {0}, does not exist.", _DataRoot);
                        exitCode = 1;
                    }
                }
            }
            catch (Exception e) {
                Logger.FatalFormat("Unexpected Exception. Message = {0}", e.Message);
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
