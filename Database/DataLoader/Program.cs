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
using LcaDataModel.Migrations;
using System.Data.Entity.Migrations;

namespace LcaDataLoader {
    /// <summary>
    /// Program to create and seed database. Loads ILCD data files.
    /// </summary>
    class Program {
        /// <summary>
        /// Directory holding ILCD data files. Default setting can be overwritten by argument.
        /// </summary>
        static string _DataRoot = null;
        static string _IlcdDirName = null;
        static string _IlcdSourceName = null;
        static string _PrivateFileName = null;
        static bool _DeleteFlag;
        static bool _CsvFlag;
        static bool _InitFlag;
        static bool _UpgradeFlag;

        public static readonly ILog Logger = LogManager.GetLogger("LcaDataLoader");

        /// <summary>
        /// Process commandline arguments.
        /// </summary>
        /// <param name="args">the arguments</param>
        /// <returns>true iff any command mode was selected</returns>
        static bool ParseArguments(string[] args) {
            bool helpFlag = false;
            bool commandMode = false;
            string ilcdSourcePath = null;

            _DataRoot = ".";
            _DeleteFlag = false;
            _CsvFlag = false;
            _InitFlag = false;
            _UpgradeFlag = false;

            OptionSet options = new OptionSet() {
                {"r|root=", "The full {DATA_ROOT} path.", v => _DataRoot = v },
                {"s|source=", "ILCD archive {source name}.", v => _IlcdSourceName = v },
                {"c|csv", "Load CSV files.", v => _CsvFlag = (v!=null)}, 
                {"i|initialize", "Create database and seed.", v => _InitFlag = (v!=null)},
                {"u|upgrade", "Upgrade database.", v => _UpgradeFlag = (v!=null)},
                {"d|delete", "Delete database, then initialize.", v => _DeleteFlag = (v!=null)},
                {"h|help",  "List options and exit", v => helpFlag = v != null },
            };
            List<string> extraArgs;
            try {
                extraArgs = options.Parse(args);
                Logger.InfoFormat("Initialize={0}, Data root={1}, Load CSVs={2}, Delete={3}, Upgrade={4}", _InitFlag.ToString(), _DataRoot, _CsvFlag.ToString(), _DeleteFlag.ToString(), _UpgradeFlag.ToString());
                if (!String.IsNullOrEmpty(_IlcdSourceName)) {
                    Logger.InfoFormat("ILCD source={0}", _IlcdSourceName);
                    if (String.IsNullOrEmpty(_DataRoot)) {
                        throw new OptionException ("Missing root path.", "-r");
                    }
                    ilcdSourcePath = Path.Combine(_DataRoot, _IlcdSourceName);
                    _IlcdDirName = Path.Combine(ilcdSourcePath, "ILCD");
                    _PrivateFileName = Path.Combine(ilcdSourcePath, "private");
                }
                if (_CsvFlag) {
                    if (String.IsNullOrEmpty(_DataRoot)) {
                        throw new OptionException ("Missing root path.", "-r");
                    }
                }
                if (helpFlag || (!_DeleteFlag && !_InitFlag && !_UpgradeFlag && String.IsNullOrEmpty(_IlcdSourceName) && !_CsvFlag)) {
                    options.WriteOptionDescriptions(Console.Out);
                } else {
                    commandMode = true;
                }
            }
            catch (OptionException e) {
                Logger.ErrorFormat("Usage Error: {0}", e.Message);
            }
            
            return commandMode;
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
            bool loadedFiles = false;
            try {
                StartLogging();
                if (ParseArguments(args)) {
                    if (_DeleteFlag) {
                        Database.SetInitializer<EntityDataModel>(new DropCreateDatabaseInitializer());
                    }
                    else if (_InitFlag) {
                        Database.SetInitializer<EntityDataModel>(new CreateDatabaseInitializer());
                    }
                    else if (_UpgradeFlag) {
                        var configuration = new Configuration();
                        var migrator = new DbMigrator(configuration);
                        migrator.Update();
                        return 0;
                    }
                    else {
                        Database.SetInitializer<EntityDataModel>(null);
                    }
                    DbContextWrapper dbContext = new DbContextWrapper();

                    if (!String.IsNullOrEmpty(_IlcdDirName)) {
                        if (Directory.Exists(_IlcdDirName)) {
                            IlcdImporter ilcdImporter = new IlcdImporter();
                            ilcdImporter.LoadAll(_IlcdDirName, _IlcdSourceName, dbContext, File.Exists(_PrivateFileName));
                            Logger.InfoFormat("Loaded ILCD archive from {0}.", _IlcdDirName);
                            loadedFiles = true;
                        }
                        else {
                            Logger.ErrorFormat("ILCD folder, {0}, does not exist.", _IlcdDirName);
                            exitCode = 1;
                        }
                    }
                    if (_CsvFlag) {
                        if (Directory.Exists(_DataRoot)) {
                            CsvImporter.LoadAll(_DataRoot, dbContext);
                            Logger.InfoFormat("Loaded CSV folders under {0}.", _DataRoot);
                            loadedFiles = true;
                        }
                        else {
                            Logger.ErrorFormat("Data Root folder, {0}, does not exist.", _DataRoot);
                            exitCode = 1;
                        }
                    }
                    if (loadedFiles) { 
                        Program.Logger.InfoFormat("Update LCIA Flow reference...");
                        dbContext.UpdateLciaFlowID();
                    }                    
                }
            }
            catch (Exception e) {
                Logger.FatalFormat("Unexpected Exception: {0}", e.Message);
                for (var ie = e.InnerException; ie != null; ie = ie.InnerException) {
                    Program.Logger.FatalFormat("Inner exception: {0}", ie.Message);
                }
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
