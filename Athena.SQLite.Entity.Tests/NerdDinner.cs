//
// NerdDinner.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Data.Entity;

namespace System.Data.SQLite.Tests.Entity
{
	public class Dinner
	{
		internal protected Dinner() { }

		public int DinnerId { get; set; }
		public Guid Identifier { get; set; }
		public string Title { get; set; }
		public DateTime EventDate { get; set; }
		public string Address { get; set; }
		public double DoubleValue { get; set; }

		public virtual ICollection<Rsvp> Rsvps { get; set; }
	}

	public class Rsvp
	{

		public int RsvpId { get; set; }
		public int DinnerId { get; set; }
		public string Email { get; set; }
		public virtual Dinner Dinner { get; set; }
	}
    [SQLite.Entity.SQLiteConfiguration]
	public class NerdDinners : DbContext
	{
		public NerdDinners(System.Data.Common.DbConnection conn) : base(conn, true)
		{
		}
		
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			// Dinners
			modelBuilder.Entity<Dinner>().HasKey(d => d.DinnerId);
			
			modelBuilder.Entity<Dinner>().ToTable("Dinners");
			
			modelBuilder.Entity<Dinner>().Property(d => d.EventDate).HasColumnName("EventDate");
			modelBuilder.Entity<Dinner>().Property(d => d.Identifier).HasColumnName("DinnerGuid").IsRequired();
			modelBuilder.Entity<Dinner>().Property(d => d.Address).HasColumnName("Address");
			modelBuilder.Entity<Dinner>().Property(d => d.Title).HasColumnName("Title");
			modelBuilder.Entity<Dinner>().Property(d => d.DoubleValue).HasColumnName("dv");

			// RSVPs
			modelBuilder.Entity<Rsvp>().HasKey(r => r.RsvpId);

			modelBuilder.Entity<Rsvp>().ToTable("Rsvps");

			modelBuilder.Entity<Rsvp>().Property(r => r.Email).HasColumnName("Email");
			modelBuilder.Entity<Rsvp>().Property(r => r.DinnerId).HasColumnName("DinnerId");

			modelBuilder.Entity<Rsvp>().HasRequired(r => r.Dinner).WithMany(d => d.Rsvps).HasForeignKey(r => r.DinnerId);
			
			base.OnModelCreating(modelBuilder);
		}

		public virtual DbSet<Dinner> Dinners { get; set; }
		public virtual DbSet<Rsvp> Rsvps { get; set; }

	}
}

