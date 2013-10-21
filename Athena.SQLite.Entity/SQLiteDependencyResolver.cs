using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Linq;
using System.Text;

namespace System.Data.SQLite.Entity
{
    /// <summary>
    /// This class provides SQLite customized dependencies.
    /// </summary>
    internal class SQLiteDependencyResolver : IDbDependencyResolver
    {
        public object GetService(Type type, object key)
        {
            if (type.Name.StartsWith("IDatabaseInitializer"))
            {
                var attr = type.GetGenericArguments().First().GetCustomAttributes(typeof(SQLiteConfigurationAttribute), false).OfType<SQLiteConfigurationAttribute>().FirstOrDefault();
                if (attr != null && attr.GenerateDatabase)
                {
                    var target = typeof(SQLiteDatabaseGenerator<>).MakeGenericType(type.GetGenericArguments());
                    return Activator.CreateInstance(target);
                }
            }
            return null;
        }

        public IEnumerable<object> GetServices(Type type, object key)
        {
            return Enumerable.Empty<object>();
        }
    }
}
