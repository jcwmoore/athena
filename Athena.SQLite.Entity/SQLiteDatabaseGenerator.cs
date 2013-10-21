using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace System.Data.SQLite.Entity
{
    /// <summary>
    /// This class is the default generator for SQLite databases
    /// </summary>
    internal class SQLiteDatabaseGenerator<TContext> : CreateDatabaseIfNotExists<TContext>, IDatabaseInitializer<TContext> where TContext : DbContext
    {
        void IDatabaseInitializer<TContext>.InitializeDatabase(TContext context)
        {
            var attr = context.GetType().GetCustomAttributes(typeof(SQLiteConfigurationAttribute), false).OfType<SQLiteConfigurationAttribute>().FirstOrDefault();
            if (attr != null && attr.GenerateDatabase)
            {
                DdlBuilder.GenerateForeignKeys = attr.GenerateForeignKeys;
                base.InitializeDatabase(context);
            }
        }
    }
}
