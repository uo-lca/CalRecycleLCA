﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DataAccess;
using LcaDataModel;

namespace LcaDataLoader {
    class CsvImporter {

        /// <summary>
        /// Load all csv directories under a given root directory.
        /// </summary>
        /// <param name="dirName">Full path name of the root directory</param>
        public void LoadAll(string dirName) {
            using (DbContextWrapper dbContext = new DbContextWrapper()) {
                LoadAppend(Path.Combine(dirName, "append"), dbContext);
            }
        }

        private bool ImportCategorySystem(Row row, DbContextWrapper dbContext) {
            CategorySystem obj = new CategorySystem {
                DataTypeID = Convert.ToInt32(row["DataTypeID"]),
                Delimiter = row["Delimiter"],
                Name = row["Name"]
            } ;   
            return dbContext.AddEntity(obj);
        }


        private int ImportCSV(string fileName, Func<Row, DbContextWrapper, bool> importRow, DbContextWrapper dbContext) {
            int importCounter = 0;
            var table = DataAccess.DataTable.New.ReadCsv(fileName);
            foreach (Row row in table.Rows) {
                if (importRow(row, dbContext)) importCounter++;
            }
            Console.WriteLine("INFO: {0} of {1} records imported from {2}.", importCounter, table.Rows.Count(), fileName);
            return importCounter;
        }

        /// <summary>
        /// Load CSV files in append directory
        /// </summary>
        /// <param name="dirName">Full path name of append directory</param>
        public void LoadAppend(string dirName, DbContextWrapper dbContext) {
            int importCounter = 0;
            if (Directory.Exists(dirName)) {
                if (ImportCSV(Path.Combine(dirName, "CategorySystem.csv"), ImportCategorySystem, dbContext) > 0)
                    ++importCounter;
                Console.WriteLine("INFO: {0} of {1} CSV files loaded from {2}.", importCounter, 3, dirName);
            }
            else {
                Console.WriteLine("WARNING: CSV folder, {0}, does not exist.", dirName);
            }
        }
    }
}
