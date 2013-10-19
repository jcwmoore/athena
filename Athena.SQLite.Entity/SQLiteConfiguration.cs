using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace System.Data.SQLite.Entity
{
    public class SQLiteConfiguration : DbConfiguration
    {
        const string SQLITE = "System.Data.SQLite";

        public SQLiteConfiguration()
        {
            //SetExecutionStrategy("System.Data.SQLite", () => new SQLiteExecutionStrategy());
            SetDefaultConnectionFactory(SQLiteClientFactory.Instance);
            SetProviderFactory(SQLITE, SQLiteClientFactory.Instance);
            SetProviderServices(SQLITE, SQLiteProviderServices.Instance);
        }
    }

    public class SQLiteConfigurationAttribute : DbConfigurationTypeAttribute
    {
        public SQLiteConfigurationAttribute() : base(typeof(SQLiteConfiguration))
        {
        }
    }
}
