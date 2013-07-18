//
// SimpleInsertFixture.cs
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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Data.Entity;


namespace System.Data.SQLite.Tests.Entity
{
	[TestFixture]
	public class SimpleUpdateFixture
	{
		string _db = string.Format("Data Source={0}.db3", Guid.NewGuid());

		[TestFixtureSetUp]
		public void Setup()
		{
			using (var conn = new SQLiteConnection(_db))
			using (var cmd = conn.CreateCommand())
			{
				conn.Open();
				cmd.CommandText = "CREATE TABLE IF NOT EXISTS Dinners (DinnerId INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT, EventDate DATETIME, Address TEXT, DinnerGuid TEXT NOT NULL, dv FLOAT NOT NULL);";
				cmd.ExecuteNonQuery();
			}
			Database.SetInitializer<NerdDinners>(null);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			System.IO.File.Delete(_db.Split('=')[1]);
		}



		[Test()]
		public void SimpleUpdateTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			var i = ctx.SaveChanges();
			ctx.Dispose();
			Assert.That(i, Is.EqualTo(5));
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var items = from d in ctx.Dinners where d.Address == "test2" select d;
			var update = items.First();
			int id = update.DinnerId;
			update.Address = "New Address";
			update.EventDate = DateTime.Today.AddDays(3);
			var oldId = Guid.NewGuid();
			update.Identifier = Guid.NewGuid();
			update.DoubleValue = 54.4;
			ctx.SaveChanges();
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var actuals = from d in ctx.Dinners where d.DinnerId == id select d;
			var actual = actuals.First();
			Assert.That(actual.Address, Is.EqualTo("New Address"));
			Assert.That(actual.DoubleValue, Is.EqualTo(54.4));
			Assert.That(actual.Identifier, Is.Not.EqualTo(oldId));
		}
	}
}
