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
	[TestFixture()]
	public class SimpleInsertFixture
	{
		string _db = string.Format("Data Source={0}.db3", Guid.NewGuid());

		[TestFixtureSetUp]
		public void Setup()
		{}

		[TestFixtureTearDown]
		public void TearDown()
		{
			System.IO.File.Delete(_db.Split('=')[1]);
		}

		[Test()]
		public void SimpleInsertAutoIncrementPrimaryKeyTest ()
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
			var items = from d in ctx.Dinners where d.DinnerId > 0 select d.DinnerId;
			var set = new HashSet<int>();
			foreach(var item in items) {
				set.Add(item);
			}
			Assert.That(set.Contains(1));
			Assert.That(set.Contains(2));
			Assert.That(set.Contains(3));
			Assert.That(set.Contains(4));
			Assert.That(set.Contains(5));
		}
		[Test()]
		public void SimpleInsertGuidTest ()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var ids = new List<Guid>();
			ids.Add(Guid.NewGuid());
			ids.Add(Guid.NewGuid());
			ids.Add(Guid.NewGuid());
			ids.Add(Guid.NewGuid());
			ids.Add(Guid.NewGuid());
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = ids[0], DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today, Title = "John's dinner", Identifier = ids[1], DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today, Title = "John's dinner", Identifier = ids[2], DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today, Title = "John's dinner", Identifier = ids[3], DoubleValue = 1.4 });	
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today, Title = "John's dinner", Identifier = ids[4], DoubleValue = 1.5 });
			var i = ctx.SaveChanges();
			ctx.Dispose();
			Assert.That(i, Is.EqualTo(5));
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var items = from d in ctx.Dinners where d.DinnerId > 0 select d.Identifier;
			var set = new HashSet<Guid>();
			foreach(var item in items) {
				set.Add(item);
			}
			foreach(var id in ids)
			{
				Assert.That(set.Contains(id));
			}
		}
		[Test()]
		public void SimpleInsertDateTimesTest ()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(-.134), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test3", EventDate = DateTime.Today.AddDays(-.623), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test4", EventDate = DateTime.Today.AddDays(-.234), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });	
			ctx.Dinners.Add(new Dinner { Address = "test5", EventDate = DateTime.Today.AddDays(-.3426), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			var i = ctx.SaveChanges();
			ctx.Dispose();
			Assert.That(i, Is.EqualTo(5));
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var items = from d in ctx.Dinners where d.DinnerId > 0 select d.EventDate;
			var set = new HashSet<DateTime>();
			foreach(var item in items) {
				set.Add(item);
			}
			Assert.That(set.Contains(DateTime.Today));
			Assert.That(set.Contains(DateTime.Today.AddDays(-.134)));
			Assert.That(set.Contains(DateTime.Today.AddDays(-.623)));
			Assert.That(set.Contains(DateTime.Today.AddDays(-.234)));
			Assert.That(set.Contains(DateTime.Today.AddDays(-.3426)));
		}
		[Test()]
		public void SimpleInsertStringsTest ()
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
			var items = from d in ctx.Dinners where d.DinnerId > 0 select d.Address;
			var set = new HashSet<string>();
			foreach(var item in items) {
				set.Add(item);
			}
			Assert.That(set.Contains("test1"));
			Assert.That(set.Contains("test2"));
			Assert.That(set.Contains("test3"));
			Assert.That(set.Contains("test4"));
			Assert.That(set.Contains("test5"));
		}
		[Test()]
		public void SimpleInsertFloatingPointTest ()
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
			var items = from d in ctx.Dinners where d.DinnerId > 0 select d.DoubleValue;
			var set = new HashSet<double>();
			foreach(var item in items) {
				set.Add(item);
			}
			Assert.That(set.Contains(1.1));
			Assert.That(set.Contains(1.2));
			Assert.That(set.Contains(1.3));
			Assert.That(set.Contains(1.4));
			Assert.That(set.Contains(1.5));
		}
	}
}

