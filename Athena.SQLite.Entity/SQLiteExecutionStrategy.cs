using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.SQLite.Entity
{
    public class SQLiteExecutionStrategy : System.Data.Entity.Infrastructure.IDbExecutionStrategy
    {
        public TResult Execute<TResult>(Func<TResult> operation)
        {
            try
            {
                return operation.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Execute(Action operation)
        {
            try
            {
                operation.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool RetriesOnFailure
        {
            get { return true; }
        }
    }
}
