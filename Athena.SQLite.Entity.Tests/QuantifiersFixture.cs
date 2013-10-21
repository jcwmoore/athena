//
// QuantifiersFixture.cs
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
	/// <summary>
	/// http://code.msdn.microsoft.com/LINQ-Quantifiers-f00e7e3e
	/// </summary>		
	public class QuantifiersFixture
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
		public void SimpleAnyTest()
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
			var res = from d in ctx.Dinners
				where d.DoubleValue == 1.4
					select d;
			Assert.That(res.Any(), Is.True);
		}
		[Test()]
		public void ConditionalAnyTest()
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
			var res = ctx.Dinners.Any(d => d.DoubleValue > 1);
			Assert.That(res, Is.True);
		}
		[Test()]
		public void NestedAnyTest()
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
			var res = from d in ctx.Dinners where d.Rsvps.Any() select d;
			//Console.WriteLine (res.ToString());
			var list = res.ToList();
			Assert.That(list.Count, Is.EqualTo(1));
		}
		[Test()]
		public void NestedNotAnyTest()
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
			var res = from d in ctx.Dinners where !d.Rsvps.Any() select d;
			//Console.WriteLine (res.ToString());
			var list = res.ToList();
			Assert.That(list.Count, Is.EqualTo(4));
		}
		[Test()]
		public void ConditionalAllTest()
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
			var res = ctx.Dinners.All(d => d.DoubleValue > 1);
			Assert.That(res, Is.True);
		}
		[Test()]
		public void NestedAllTest()
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
			var res = from d in ctx.Dinners where d.Rsvps.All(r => r.Email == null) select d;
			//Console.WriteLine (res.ToString());
			var list = res.ToList();
			Assert.That(list.Count, Is.EqualTo(4));
		}
	}
}

