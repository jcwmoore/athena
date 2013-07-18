//
// System.Data.SQLiteiteClient.SqliteClientFactory.cs
//
// Author:
//   Chris Toshok (toshok@ximian.com)
//
//
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Data;
using System.Data.Common;

namespace System.Data.SQLite
{
	public partial class SQLiteClientFactory : DbProviderFactory//, IServiceProvider
	{
		public static readonly SQLiteClientFactory Instance = null;
		public static object lockStatic = new object();

		private SQLiteClientFactory()
		{
		}

		static SQLiteClientFactory()
		{
			lock (lockStatic)
			{
				if (Instance == null)
					Instance = new SQLiteClientFactory();
			}
		}

		public override bool CanCreateDataSourceEnumerator
		{
			get
			{
				return false;
			}
		}

		public override DbCommand CreateCommand()
		{
			return new SQLiteCommand();
		}
		
		public override DbCommandBuilder CreateCommandBuilder()
		{
			return new SQLiteCommandBuilder();
		}

		public override DbConnection CreateConnection()
		{
			return new SQLiteConnection();
		}

		public override DbDataAdapter CreateDataAdapter()
		{
			return new SQLiteDataAdapter();
		}

		public override DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			return new SQLiteDataSourceEnumerator();
		}

		public override DbParameter CreateParameter()
		{
			return new SQLiteParameter();
		}

		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return new SQLiteConnectionStringBuilder();
		}



	}

}
