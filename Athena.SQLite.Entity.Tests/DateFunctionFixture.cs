//
// DateFunctionFixture.cs
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
	public class DateFunctionFixture
	{
		string _db;
		
		[SetUp]
		public void Setup()
		{
			_db = string.Format("Data Source={0}.db3", Guid.NewGuid());
			using (var conn = new SQLiteConnection(_db))
				using (var cmd = conn.CreateCommand())
			{
				conn.Open();
				cmd.CommandText = "CREATE TABLE IF NOT EXISTS Dinners (DinnerId INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT, EventDate DATETIME, Address TEXT, DinnerGuid TEXT NOT NULL, dv FLOAT NOT NULL);";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "CREATE TABLE IF NOT EXISTS Rsvps (RsvpId INTEGER PRIMARY KEY AUTOINCREMENT, DinnerId INTEGER NOT NULL REFERENCES Dinners(DinnerId), Email TEXT);";
				cmd.ExecuteNonQuery();
			}
			Database.SetInitializer<NerdDinners>(null);
		}
		
		[TearDown]
		public void TearDown()
		{
			System.IO.File.Delete(_db.Split('=')[1]);
		}

		[Test(Description = "DateTime.Year. This sample uses the DateTime's Year property to find Orders placed in 1997.")]
		public void DateTimeYearTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(0), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(0), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(0), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(0), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var q = from d in ctx.Dinners
					where d.EventDate.Year == DateTime.Today.Year
					select d;
			
			var list = q.ToList();
			Assert.That(list.Count, Is.EqualTo(5));
		}

		[Test(Description = "DateTime.Month. This sample uses the DateTime's Month property to find Orders placed in December.")]
		public void DateTimeMonthTest()
		{
			var ctx = new NerdDinners(new SQLiteConnection(_db));
			var dinner = new Dinner { Address = "test1", EventDate = DateTime.Today, Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 };
			ctx.Dinners.Add(dinner);
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(0), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(0), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test2", EventDate = DateTime.Today.AddDays(0), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			ctx.Dinners.Add(new Dinner { Address = "test1", EventDate = DateTime.Today.AddDays(0), Title = "John's dinner", Identifier = Guid.NewGuid(), DoubleValue = 1 });
			
			ctx.SaveChanges();
			var i = dinner.Identifier;
			ctx.Dispose();
			ctx = new NerdDinners(new SQLiteConnection(_db));
			var q = from d in ctx.Dinners
					where d.EventDate.Month == DateTime.Today.Month
					select d;
			Console.WriteLine (q.ToString());
			var list = q.ToList();
			Assert.That(list.Count, Is.EqualTo(5));
		}

		[Test(Description = "DateTime.Day. This sample uses the DateTime's Day property to find Orders placed on the 31st day of the month.")]
		public void DateTimeDayTest()
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
			var q = from d in ctx.Dinners
					where d.EventDate.Day == DateTime.Today.Day
					select d;
			
			var list = q.ToList();
			Assert.That(list.Count, Is.EqualTo(1));
		}

		[Test(Description = "DateTime.Hour. This sample uses the DateTime's Day property to find Orders placed on the 31st day of the month.")]
		public void DateTimeHourTest()
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
			var q = from d in ctx.Dinners
					where d.EventDate.Hour == DateTime.Today.Hour
					select d;
			
			var list = q.ToList();
			Assert.That(list.Count, Is.EqualTo(5));
		}
	}
}

