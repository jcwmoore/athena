using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Linq;
using System.Text;

namespace System.Data.SQLite.Entity
{
    public class SQLiteConfiguration : DbConfiguration
    {
        const string SQLITE = "System.Data.SQLite";
        const string SQLITE_CONNECTION = "System.Data.SQLite.SQLiteConnection";

        public SQLiteConfiguration()
        {
            SetDefaultConnectionFactory(SQLiteClientFactory.Instance);
            SetProviderFactory(SQLITE, SQLiteClientFactory.Instance);
            SetProviderServices(SQLITE, SQLiteProviderServices.Instance);
            AddDependencyResolver(new SQLiteDependencyResolver());
        }

    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class SQLiteConfigurationAttribute : DbConfigurationTypeAttribute
    {
        public SQLiteConfigurationAttribute() : base(typeof(SQLiteConfiguration))
        {
            GenerateDatabase = true;
            GenerateForeignKeys = false;
        }

        /// <summary>
        /// Indicates if we should try to generate the database using the Initializer, <c>CreateDatabaseIfNotExists</c>.  Default value is true
        /// </summary>
        public bool GenerateDatabase { get; set; }

        /// <summary>
        /// Foreign Keys are not possible with EF generated SQLite databases, but we enforce them with Triggers.  
        /// Setting the option will create three triggers per association to enforce foreign keys.  Default value is false.
        /// </summary>
        public bool GenerateForeignKeys { get; set; }
    }
}
