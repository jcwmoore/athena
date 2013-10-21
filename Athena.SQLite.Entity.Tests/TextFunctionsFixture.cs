//
// TextFunctionsFixture.cs
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
	public class TextFunctionsFixture
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
		public void ToUpperTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			
			var r = from d in ctx.Dinners
					select d.Title.ToUpper();
			foreach(var d in r)
			{
				Assert.That(d, Is.EqualTo("JOHN'S DINNER"));
			}
		}
		[Test()]
		public void ToLowerTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			
			var r = from d in ctx.Dinners
				select d.Title.ToLower();
			foreach(var d in r)
			{
				Assert.That(d, Is.EqualTo("john's dinner"));
			}
		}
		[Test()]
		public void StringLengthTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(2), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(1), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			
			var r = from d in ctx.Dinners
				select d.Title.Length;
			foreach(var d in r)
			{
				Assert.That(d, Is.EqualTo("john's dinner".Length));
			}
		}
		[Test()]
		public void StringContainsTest()
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
			var res = ctx.Dinners.Where(d => d.Address.Contains("test"));
			Console.WriteLine (res.ToString());
			Assert.That(res.Any(), Is.True);
		}
		[Test()]
		public void StringComparisonTest()
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
			var res = ctx.Dinners.Where(d => d.Address.CompareTo("t") > 0).ToList();
			Assert.That(res.Any(), Is.True);
		}
	}
}

