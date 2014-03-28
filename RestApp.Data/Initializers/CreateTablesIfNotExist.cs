using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;
using System.Data.SqlClient;
using RestApp.Core.Data;
using RestApp.Core.Infrastructure;

namespace RestApp.Data.Initializers
{
    //TODO: analizar la creacion de base de datos
    public class CreateTablesIfNotExist<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        private readonly string[] gTablesToValidate;
        private readonly string[] gCustomCommands;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tablesToValidate">A list of existing table names to validate; null to don't validate table names</param>
        /// <param name="customCommands">A list of custom commands to execute</param>
        public CreateTablesIfNotExist(string[] tablesToValidate, string[] customCommands)
            : base()
        {
            this.gTablesToValidate = tablesToValidate;
            this.gCustomCommands = customCommands;
        }

        public void InitializeDatabase(TContext context)
        {
            bool dbExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                dbExists = context.Database.Exists();
            }
            if (dbExists)
            {
                bool createTables = false;
                if (gTablesToValidate != null && gTablesToValidate.Length > 0)
                {
                    //we have some table names to validate
                    var existingTableNames = new List<string>(context.Database.SqlQuery<string>("SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'"));
                    createTables = existingTableNames.Intersect(gTablesToValidate, StringComparer.InvariantCultureIgnoreCase).Count() == 0;
                }
                else
                {
                    //check whether tables are already created
                    int numberOfTables = 0;
                    foreach (var t1 in context.Database.SqlQuery<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' "))
                        numberOfTables = t1;

                    createTables = numberOfTables == 0;
                }

                if (createTables)
                {
                    //create all tables
                    var dbCreationScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
                    context.Database.ExecuteSqlCommand(dbCreationScript);

                    //Seed(context);
                    context.SaveChanges();

                    if (gCustomCommands != null && gCustomCommands.Length > 0)
                    {
                        foreach (var command in gCustomCommands)
                            context.Database.ExecuteSqlCommand(command);
                    }
                }
            }
            else
            {
                throw new ApplicationException("No database instance");
            }
        }
    }
}
