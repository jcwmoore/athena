using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Athena.Extensions.Tests
{
    public class CronStringParserFixture
    {
        /// <summary>
        ///A test for NextTimeMinute
        ///</summary>
        [Test()]
        public void NextTimeMinuteTest()
        {
            string timeString = "* * * * *"; // Every Minute of every hour of everyday
            var time = new DateTime(2009, 1, 1, 6, 1, 34, DateTimeKind.Utc); // 2009 - Jan - 1 06:01:34 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 6, 2, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Jan - 1 06:02:00 UTC
            time = time.NextTime(timeString);
            Assert.AreEqual(new DateTime(2009, 1, 1, 6, 3, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Jan - 1 06:03:00 UTC
        }

        /// <summary>
        ///A test for NextTimeHourly
        ///</summary>
        [Test()]
        public void NextTimeHourlyTest()
        {
            string timeString = "0 * * * *"; // the top every hour of everyday
            var time = new DateTime(2009, 1, 1, 6, 1, 24, DateTimeKind.Utc); // 2009 - Jan - 1 06:01:24 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 7, 0, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Jan - 1 07:00:00 UTC
            timeString = "0,30 * * * *"; // every half hour
            Assert.AreEqual(new DateTime(2009, 1, 1, 6, 30, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Jan - 1 07:00:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 7, 0, 0, DateTimeKind.Utc), time.AddMinutes(30).NextTime(timeString)); // 2009 - Jan - 1 07:00:00 UTC
            timeString = "0,15,30,45 * * * *"; // every quarter hour
            Assert.AreEqual(new DateTime(2009, 1, 1, 6, 15, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Jan - 1 07:00:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 6, 30, 0, DateTimeKind.Utc), time.AddMinutes(15).NextTime(timeString)); // 2009 - Jan - 1 06:30:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 6, 45, 0, DateTimeKind.Utc), time.AddMinutes(30).NextTime(timeString)); // 2009 - Jan - 1 06:45:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 7, 00, 0, DateTimeKind.Utc), time.AddMinutes(45).NextTime(timeString)); // 2009 - Jan - 1 07:00:00 UTC
            timeString = "3,18,33,48 * * * *"; // every quarter hour + three minutes
            Assert.AreEqual(new DateTime(2009, 1, 1, 6, 03, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Jan - 1 06:18:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 6, 18, 0, DateTimeKind.Utc), time.AddMinutes(15).NextTime(timeString)); // 2009 - Jan - 1 06:18:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 6, 33, 0, DateTimeKind.Utc), time.AddMinutes(30).NextTime(timeString)); // 2009 - Jan - 1 06:33:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 6, 48, 0, DateTimeKind.Utc), time.AddMinutes(45).NextTime(timeString)); // 2009 - Jan - 1 06:48:00 UTC 
        }

        /// <summary>
        ///A test for NextTimeDaily
        ///</summary>
        [Test()]
        public void NextTimeDailyTest()
        {
            string timeString = "0 0 * * *"; // Daily at midnight
            var time = new DateTime(2009, 1, 1, 6, 1, 24, DateTimeKind.Utc); // 2009 - Jan - 1 06:01:24 UTC
            Assert.AreEqual(new DateTime(2009, 1, 2, 0, 0, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Jan - 2 00:00:00 UTC
            timeString = "0 7,19 * * *"; // an hour after each shift
            Assert.AreEqual(new DateTime(2009, 1, 1, 7, 00, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Jan - 1 07:00:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 19, 00, 0, DateTimeKind.Utc), time.AddHours(1).NextTime(timeString)); // 2009 - Jan - 1 19:00:00 UTC
            timeString = "20 6,18 * * *"; // an 20 minutes after each shift
            Assert.AreEqual(new DateTime(2009, 1, 1, 6, 20, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Jan - 1 06:20:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 18, 20, 0, DateTimeKind.Utc), time.AddHours(1).NextTime(timeString)); // 2009 - Jan - 1 18:20:00 UTC
            timeString = "15 0,3,6,9,12,15,18,21 * * *"; // every three hours
            Assert.AreEqual(new DateTime(2009, 1, 1, 6, 15, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Jan - 1 06:15:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 9, 15, 0, DateTimeKind.Utc), time.AddHours(3).NextTime(timeString)); // 2009 - Jan - 1 9:15:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 12, 15, 0, DateTimeKind.Utc), time.AddHours(6).NextTime(timeString)); // 2009 - Jan - 1 12:15:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 15, 15, 0, DateTimeKind.Utc), time.AddHours(9).NextTime(timeString)); // 2009 - Jan - 1 15:15:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 18, 15, 0, DateTimeKind.Utc), time.AddHours(12).NextTime(timeString)); // 2009 - Jan - 1 18:15:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 1, 21, 15, 0, DateTimeKind.Utc), time.AddHours(15).NextTime(timeString)); // 2009 - Jan - 1 21:15:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 2, 0, 15, 0, DateTimeKind.Utc), time.AddHours(18).NextTime(timeString)); // 2009 - Jan - 2 00:15:00 UTC
            Assert.AreEqual(new DateTime(2009, 1, 2, 3, 15, 0, DateTimeKind.Utc), time.AddHours(21).NextTime(timeString)); // 2009 - Jan - 2 00:03:00 UTC
        }

        /// <summary>
        ///A test for NextTimeMonthly
        ///</summary>
        [Test()]
        public void NextTimeMonthlyTest()
        {
            string timeString = "0 0 1 * *"; // first of the month at midnight
            var time = new DateTime(2009, 1, 1, 6, 1, 24, DateTimeKind.Utc); // 2009 - Jan - 1 06:01:24 UTC
            Assert.AreEqual(new DateTime(2009, 2, 1, 0, 0, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Feb - 1 00:00:00 UTC
            timeString = "15 1 1,15 * *"; // 1:15 AM on the 1st and 15th of the month
            Assert.AreEqual(new DateTime(2009, 1, 15, 1, 15, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Jan - 15 01:15:00 UTC
            Assert.AreEqual(new DateTime(2009, 2, 1, 1, 15, 0, DateTimeKind.Utc), time.AddDays(15).NextTime(timeString)); // 2009 - Feb - 1 01:15:00 UTC
        }

        /// <summary>
        ///A test for NextTimeYearly
        ///</summary>
        [Test()]
        public void NextTimeYearlyTest()
        {
            string timeString = "0 0 1 3 *"; // first of March at midnight
            var time = new DateTime(2009, 1, 1, 6, 1, 24, DateTimeKind.Utc); // 2009 - Jan - 1 06:01:24 UTC
            Assert.AreEqual(new DateTime(2009, 3, 1, 0, 0, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Mar - 1 00:00:00 UTC
            Assert.AreEqual(new DateTime(2010, 3, 1, 0, 0, 0, DateTimeKind.Utc), time.AddMonths(3).NextTime(timeString)); // 2010 - Mar - 1 00:00:00 UTC
            timeString = "0 0 1 3,4 *";
            Assert.AreEqual(new DateTime(2009, 3, 1, 0, 0, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Mar - 1 00:00:00 UTC
            time = time.NextTime(timeString);  // 2009 - Mar - 1 00:00:00 UTC
            Assert.AreEqual(new DateTime(2009, 4, 1, 0, 0, 0, DateTimeKind.Utc), time.NextTime(timeString)); // 2009 - Apr - 1 00:00:00 UTC

        }

        /// <summary>
        ///A test for NextTimeWeekly
        ///</summary>
        [Test()]
        public void NextTimeWeeklyTest()
        {
            string timeString = "0 0 * * 2,4,6"; //midnight on monday, wendesday and friday
            var time = new DateTime(2009, 10, 30, 5, 4, 12, DateTimeKind.Utc); // it is friday morning
            Assert.AreEqual(new DateTime(2009, 11, 2, 0, 0, 0, DateTimeKind.Utc), time.NextTime(timeString)); // Monday morning...
            Assert.AreEqual(new DateTime(2009, 11, 4, 0, 0, 0, DateTimeKind.Utc), time.AddDays(3).NextTime(timeString)); // Wednesday morning...
            Assert.AreEqual(new DateTime(2009, 11, 6, 0, 0, 0, DateTimeKind.Utc), time.AddDays(5).NextTime(timeString)); // Friday morning...
        }

        /// <summary>
        ///A test for NextTimeWeekly
        ///</summary>
        [Test()]
        public void NextTimeSpecificYearTest()
        {
            string timeString = "0 0 11 11 * 2011"; //veterans day 2011
            var time = new DateTime(2009, 10, 30, 5, 4, 12, DateTimeKind.Utc); // it is friday morning
            Assert.AreEqual(new DateTime(2011, 11, 11, 0, 0, 0, DateTimeKind.Utc), time.NextTime(timeString)); // veterans day
        }

        /// <summary>
        ///A test for NextTimeWeekly
        ///</summary>
        [Test()]
        public void NextTimeSpecificAnyYearTest()
        {
            string timeString = "0 0 11 11 * *"; //veterans day 2011
            var time = new DateTime(2009, 10, 30, 5, 4, 12, DateTimeKind.Utc); // it is friday morning
            Assert.AreEqual(new DateTime(2009, 11, 11, 0, 0, 0, DateTimeKind.Utc), time.NextTime(timeString)); // veterans day
        }

        /// <summary>
        ///A test for NextTimeWeekly
        ///</summary>
        [Test()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextTimePastYearTest()
        {
            string timeString = "0 0 11 11 * 2011"; //veterans day 2011
            var time = new DateTime(2013, 10, 30, 5, 4, 12, DateTimeKind.Utc);
            time.NextTime(timeString);
            Assert.Fail();
        }

        /// <summary>
        ///A test for NextTimeWeekly
        ///</summary>
        [Test()]
        public void NextTimeYearRangeTest()
        {
            string timeString = "0 0 11 11 * 2011-2015"; //veterans day 2011
            var time = new DateTime(2009, 10, 30, 5, 4, 12, DateTimeKind.Utc); // it is friday morning
            Assert.AreEqual(new DateTime(2011, 11, 11, 0, 0, 0, DateTimeKind.Utc), time.NextTime(timeString)); // veterans day
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NextTimeNullTest()
        {
            string timestring = null;
            DateTime.Now.NextTime(timestring);
            Assert.Fail("Time string accepted but is null");
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void NextTimeStringTest()
        {
            string timestring = "INVALID";
            DateTime.Now.NextTime(timestring);
            Assert.Fail("Time string accepted but is not valid cron string");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextTimeInvalidMinuteTest()
        {
            string timestring = "60 * * * *"; // minutes are 0 to 59
            DateTime.Now.NextTime(timestring);
            Assert.Fail("Time string accepted but is not valid cron string");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextTimeInvalidHourTest()
        {
            string timestring = "* 25 * * *"; // hours are 0 to 23
            DateTime.Now.NextTime(timestring);
            Assert.Fail("Time string accepted but is not valid cron string");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextTimeInvalidDayTest()
        {
            string timestring = "* * 31 * *"; //days are 1 to 28 
            DateTime.Now.NextTime(timestring);
            Assert.Fail("Time string accepted but is not valid cron string");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextTimeInvalidMonthTest()
        {
            string timestring = "* * * 13 *"; // months are 1 to 12
            DateTime.Now.NextTime(timestring);
            Assert.Fail("Time string accepted but is not valid cron string");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextTimeInvalidDayOfWeekTest()
        {
            string timestring = "* * * * 7,8"; // Day of week runs 0 to 6
            DateTime.Now.NextTime(timestring);
            Assert.Fail("Time string accepted but is not valid cron string");
        }

        /// <summary>
        /// A test for finding Daylight Savings Time Start 2007 - present
        /// </summary>
        [Test()]
        public void FindDaylightSavingTime2007Start()
        {
            string timeString = "0 2 * 3 1";
            var time = new DateTime(2009, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(new DateTime(2009, 3, 8, 2, 0, 0, DateTimeKind.Utc), time.NextTime(timeString).NextTime(timeString));
            time = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(new DateTime(2010, 3, 14, 2, 0, 0, DateTimeKind.Utc), time.NextTime(timeString).NextTime(timeString));
        }

        /// <summary>
        /// A test for finding Daylight Savings Time End 2007 - present
        /// </summary>
        [Test()]
        public void FindDaylightSavingTime2007End()
        {
            string timeString = "0 2 * 11 1";
            var time = new DateTime(2009, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(new DateTime(2009, 11, 1, 2, 0, 0, DateTimeKind.Utc), time.NextTime(timeString));
            time = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(new DateTime(2010, 11, 7, 2, 0, 0, DateTimeKind.Utc), time.NextTime(timeString));
        }

        /// <summary>
        /// A test for finding Daylight Savings Time Start 1987 - 2006
        /// </summary>
        [Test()]
        public void FindDaylightSavingTime1987Start()
        {
            string timeString = "0 2 * 4 1";
            var time = new DateTime(2006, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(new DateTime(2006, 4, 2, 2, 0, 0, DateTimeKind.Utc), time.NextTime(timeString));
        }

        /// <summary>
        /// A test for finding Daylight Savings Time Start 1966 - 1986
        /// </summary>
        [Test()]
        public void FindDaylightSavingTime1966Start()
        {
            string timeString = "0 2 * 4 1L";
            var time = new DateTime(1986, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(new DateTime(1986, 4, 27, 2, 0, 0, DateTimeKind.Utc), time.NextTime(timeString));
        }

        /// <summary>
        /// A test for finding Daylight Savings Time End 1966 - 2006
        /// </summary>
        [Test()]
        public void FindDaylightSavingTime1966End()
        {
            string timeString = "0 2 * 10 1L";
            var time = new DateTime(1986, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(new DateTime(1986, 10, 26, 2, 0, 0, DateTimeKind.Utc), time.NextTime(timeString));

        }

        /// <summary>
        /// A test for finding the last DayOfTheWeek in a specific month
        /// </summary>
        [Test()]
        public void FindLastSundayOfApril()
        {
            string timeString = "0 0 * 4,6 1L";
            var time = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(new DateTime(2010, 4, 25, 0, 0, 0, DateTimeKind.Utc), time.NextTime(timeString));
            Assert.AreEqual(new DateTime(2010, 6, 27, 0, 0, 0, DateTimeKind.Utc), time.NextTime(timeString).NextTime(timeString));
        }


    }
}
