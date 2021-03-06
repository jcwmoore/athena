//
// SQLiteClinetFactory.Entity.cs
//
// Author:
//       John Moore <jcwmoore@gmail.com>
//
// Copyright (c) 2013 John Moore
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace System.Data.SQLite
{
    public partial class SQLiteClientFactory : IServiceProvider, IDbConnectionFactory, IDbProviderFactoryResolver
	{
		#region IServiceProvider implementation
		
		public object GetService (Type serviceType)
		{
			return SQLiteProviderServices.Instance;
		}
		
		#endregion


        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            return new SQLiteConnection(nameOrConnectionString);
        }

        public DbProviderFactory ResolveProviderFactory(DbConnection connection)
        {
            if (connection is SQLiteConnection)
            {
                return this;
            }
            throw new InvalidOperationException();
        }
    }
}

