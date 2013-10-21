//
// WhereFixture.cs
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
	public class WhereFixture
	{
		string _db;
		
		[SetUp]
		public void Setup()
		{
			_db = string.Format("Data Source={0}.db3", Guid.NewGuid());
		}
		
		[TearDown]
		public void TearDown()
		{
			System.IO.File.Delete(_db.Split('=')[1]);
		}
		
		[Test()]
		public void SimpleWhereTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			
			var g = from d in ctx.Dinners
				where d.Address == "test1"
					select d;
			Assert.That(g.Count(), Is.EqualTo(3));
		}
		
		[Test()]
		public void SimpleWhereValiableTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var t = "test1";

			var g = from d in ctx.Dinners
				where d.Address == t
					select d;
			Assert.That(g.Count(), Is.EqualTo(3));
		}
		
		[Test()]
		public void SimpleWhereAndTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			
			var g = from d in ctx.Dinners
					where d.Address == "test1" && d.Identifier == i
					select d;
			Assert.That(g.Count(), Is.EqualTo(1));
		}
		
		[Test()]
		public void SimpleWhereOrTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			
			var g = from d in ctx.Dinners
					where d.Address == "test2" || d.Identifier == i
					select d;
			Assert.That(g.Count(), Is.EqualTo(3));
		}
		
		[Test()]
		public void SimpleWhereContainsTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			
			var g = from d in ctx.Dinners
					where new[] {"test1", "test2"}.Contains(d.Address)
					select d;
			Assert.That(g.Count(), Is.EqualTo(5));
		}
		
		[Test()]
		public void SimpleWhereGreaterThanTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			
			var g = from d in ctx.Dinners
				where d.DoubleValue > 1.3
					select d;
			Assert.That(g.Count(), Is.EqualTo(2));
		}
		
		[Test()]
		public void SimpleWhereLessThanTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			
			var g = from d in ctx.Dinners
					where d.DoubleValue < 1.3
					select d;
			Assert.That(g.Count(), Is.EqualTo(2));
		}
		
		[Test()]
		public void SimpleWhereGreaterThanEqualToTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			
			var g = from d in ctx.Dinners
					where d.DoubleValue >= 1.3
					select d;
			Assert.That(g.Count(), Is.EqualTo(3));
		}
		
		[Test()]
		public void SimpleWhereLessThanEqualToTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			
			var g = from d in ctx.Dinners
					where d.DoubleValue <= 1.3
					select d;
			Assert.That(g.Count(), Is.EqualTo(3));
		}
		
		[Test()]
		public void SimpleWhereNotEqualsTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.2 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.3 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.4 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1.5 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			
			var g = from d in ctx.Dinners
					where d.Address != "test1"
					select d;
			Assert.That(g.Count(), Is.EqualTo(2));
		}
	}
}

