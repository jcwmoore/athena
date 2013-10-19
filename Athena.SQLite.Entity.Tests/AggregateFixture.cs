//
// AggregateFixture.cs
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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Data.Entity;

namespace System.Data.SQLite.Tests.Entity
{
	[TestFixture()]
	/// <summary>
	/// http://code.msdn.microsoft.com/LINQ-Aggregate-Operators-c51b3869
	/// </summary>
	public class AggregateFixture
	{
		string _db;
		
		[SetUp]
		public void Setup()
		{
			_db = string.Format("Data Source={0}.db3", Guid.NewGuid());
            //using (var conn = new SQLiteConnection(_db))
            //    using (var cmd = conn.CreateCommand())
            //{
            //    conn.Open();
            //    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Dinners (DinnerId INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT, EventDate DATETIME, Address TEXT, DinnerGuid TEXT NOT NULL, dv FLOAT NOT NULL);";
            //    cmd.ExecuteNonQuery();
            //    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Rsvps (RsvpId INTEGER PRIMARY KEY AUTOINCREMENT, DinnerId INTEGER NOT NULL REFERENCES Dinners(DinnerId), Email TEXT);";
            //    cmd.ExecuteNonQuery();
            //}
            //Database.SetInitializer<NerdDinners>(null);
		}
		
		[TearDown]
		public void TearDown()
		{
			System.IO.File.Delete(_db.Split('=')[1]);
		}
		[Test()]
		public void SimpleCountTest()
		{
            using (var ctx = new NerdDinners(new SQLiteConnection(_db)))
            {
                var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
                ctx.Dinners.Add(dinner);
                ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
                ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
                ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
                ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
                ctx.SaveChanges();
            }
            using (var ctx = new NerdDinners(new SQLiteConnection(_db)))
            {
                var res = ctx.Dinners.Count();
                Assert.That(res, Is.EqualTo(5));
            }
		}
		[Test()]
		public void CountContidionalTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			ctx.SaveChanges();
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var res = ctx.Dinners.Count(x => x.DoubleValue > 1.2);
			Assert.That(res, Is.EqualTo(3));
		}
		[Test()]
		public void CountNestedTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			ctx.Rsvps.Add(new Rsvp { Dinner = dinner, Email = "testemail1" });
			ctx.Rsvps.Add(new Rsvp { Dinner = dinner, Email = "testemail2" });
			ctx.Rsvps.Add(new Rsvp { Dinner = dinner, Email = "testemail3" });
			ctx.Rsvps.Add(new Rsvp { Dinner = dinner, Email = "testemail4" });
			ctx.SaveChanges();
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var res = (from d in ctx.Dinners select new { Title = d.Title, Ct = d.Rsvps.Count() }).ToList();
			Assert.That(res.Count, Is.EqualTo(5));
			Assert.That(res.Count(r => r.Ct > 0), Is.EqualTo(1));
		}
		[Test()]
		public void SimpleSumTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			ctx.SaveChanges();
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var res = ctx.Dinners.Select(d => d.DoubleValue).Sum();
			Assert.That(res, Is.GreaterThanOrEqualTo(6.5));
		}
		[Test()]
		public void GroupedSumTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			ctx.SaveChanges();
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var res = (from d in ctx.Dinners group d by d.Title into g select new { g.Key, Value = g.Sum(x => x.DoubleValue) }).ToList();
			Assert.That(res.First().Value, Is.GreaterThanOrEqualTo(6.5));
		}
		[Test()]
		public void SimpleMinTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			ctx.SaveChanges();
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var res = ctx.Dinners.Select(d => d.DoubleValue).Min();
			Assert.That(res, Is.EqualTo(1.1));
		}
		[Test()]
		public void GroupedMinTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			ctx.SaveChanges();
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var res = (from d in ctx.Dinners group d by d.Title into g select new { g.Key, Value = g.Min(x => x.DoubleValue) }).ToList();
			Assert.That(res.First().Value, Is.EqualTo(1.1));
		}
		[Test()]
		public void SimpleMaxTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			ctx.SaveChanges();
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var res = ctx.Dinners.Select(d => d.DoubleValue).Max();
			Assert.That(res, Is.EqualTo(1.5));
		}
		[Test()]
		public void GroupedMaxTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			ctx.SaveChanges();
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var res = (from d in ctx.Dinners group d by d.Title into g select new { g.Key, Value = g.Max(x => x.DoubleValue) }).ToList();
			Assert.That(res.First().Value, Is.EqualTo(1.5));
		}
		[Test()]
		public void SimpleAvgTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			ctx.SaveChanges();
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var res = ctx.Dinners.Select(d => d.DoubleValue).Average();
			Assert.That(res, Is.EqualTo(1.3));
		}
		[Test()]
		public void GroupedAvgTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			ctx.SaveChanges();
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var res = (from d in ctx.Dinners group d by d.Title into g select new { g.Key, Value = g.Average(x => x.DoubleValue) }).ToList();
			Assert.That(res.First().Value, Is.EqualTo(1.3));
		}
	}
}

