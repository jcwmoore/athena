using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace System
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class CronStringParser
    {
        /// <summary>
        /// Safely tries to compute the next occurrence of the cron string
        /// </summary>
        /// <param name="baseTime">reference time</param>
        /// <param name="result">Next occurrence</param>
        /// <param name="cronString">CRON string, i.e. 0 0 1 1 * 2011</param>
        /// <returns>true if there is a valid next time, false otherwise</returns>
        public static bool TryNextTime(this DateTime baseTime, out DateTime result, string cronString)
        {
            result = DateTime.MinValue;
            try
            {
                result = NextTime(baseTime, cronString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Computes the next occurring time described by the provided CRON string, see http://en.wikipedia.org/wiki/Cron
        /// </summary>
        /// <param name="baseTime">Starting Time</param>
        /// <param name="cronString">CRON string, i.e. 0 0 1 1 * 2011</param>
        /// <returns>The next valid DateTime after <see cref="baseTime"/> that is described by the <see cref="cronString"/></returns>
        /// <exception cref="System.InvalidOperationException">Throws InvalidOperationException if there is no future time described by the CRON string</exception>
        public static DateTime NextTime(this DateTime baseTime, string cronString)
        {
            if (cronString == null)
            {
                throw new ArgumentNullException("cronString", "Must Specify a value");
            }
            Regex matcher = new Regex(@"((\*)|((\d\d?)(,\d\d?)*))" +
                                      @"(\s((\*)|((\d\d?)(,\d\d?)*)))" +
                                      @"(\s((\*)|((\d\d?)(,\d\d?)*)|(L)))" +
                                      @"(\s((\*)|((\d\d?)(,\d\d?)*)))" +
                                      @"(\s((\*)|((\d)(,\d)*)|(\dL)))" +
                                      @"(\s((\*)|((\d{4})(,\d{4})*)|((\d{4})(-\d{4}))))?");
            if (!matcher.IsMatch(cronString))
            {
                throw new ArgumentException("Provided string does not match the cron string pattern", "cronString");
            }
            var tmp = cronString.Split(' ');
            for (int i = 0; i < 5; i++)
            {
                tmp[i] = tmp[i].Replace("*", "-1");
            }
            bool lastDayOfMonth = false;
            bool lastDayOfWeek = false;
            bool hasYear = false;

            if (tmp.Length == 6)
            {
                hasYear = true;
                if (tmp[5].Contains('-'))
                {
                    var split = tmp[5].Split('-');
                    var sYear = int.Parse(split[0]);
                    var eYear = int.Parse(split[1]);
                    tmp[5] = string.Join(",", Enumerable.Range(sYear, eYear).ToArray());
                }
                tmp[5] = tmp[5].Replace("*", "-1");

            }
            if (tmp[2].Contains("L"))
            {
                lastDayOfMonth = true;
                tmp[2] = "-1";
            }
            if (tmp[4].Contains("L"))
            {
                lastDayOfWeek = true;
                tmp[4] = tmp[4].Replace("L", "");
            }

            DateTime nextTime = baseTime;
            int[] month = tmp[3].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToArray();
            int[] day = tmp[2].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToArray();
            int[] mins = tmp[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToArray();
            int[] hours = tmp[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToArray();
            int[] dayOWeek = tmp[4].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToArray();
            int[] years = new int[0];
            if (hasYear)
            {
                years = tmp[5].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToArray();
            }

            ValidateTimePortionNumbers(mins, 0, 59, "Minute");
            ValidateTimePortionNumbers(hours, 0, 23, "Hour");
            ValidateTimePortionNumbers(day, 1, 28, "Day of Month"); // only allowing days 1 to 28, no 29,30 or 31 day of month runs.
            ValidateTimePortionNumbers(month, 1, 12, "Month of Year");
            ValidateTimePortionNumbers(dayOWeek, 1, 7, "Day of Week");

            nextTime = new DateTime(nextTime.Year,
                                    nextTime.Month,
                                    nextTime.Day,
                                    nextTime.Hour,
                                    nextTime.Minute,
                                    0,
                                    nextTime.Kind).AddMinutes(1);

            while (true)
            {
                bool reset = false;
                nextTime = NextMinute(nextTime, mins);
                nextTime = NextHour(nextTime, hours, out reset);
                if (reset)
                {
                    continue;
                }
                nextTime = NextDay(nextTime, day, lastDayOfMonth, out reset);
                if (reset)
                {
                    continue;
                }
                nextTime = NextMonth(nextTime, month, out reset);
                if (reset)
                {
                    continue;
                }
                nextTime = NextDayOfWeek(nextTime, dayOWeek, lastDayOfWeek, out reset);
                if (reset)
                {
                    continue;
                }
                if (hasYear)
                {
                    nextTime = NextYear(nextTime, years, out reset);
                    if (reset)
                    {
                        continue;
                    }
                }
                break;
            }
            return nextTime;
        }

        # region Next Valid Date Time Part

        private static DateTime NextYear(DateTime baseTime, int[] years, out bool reset)
        {
            reset = true;
            if (years.Contains(-1) || years.Contains(baseTime.Year))
            {
                reset = false;
                return baseTime;
            }
            var nextYear = years.FirstOrDefault(y => y > baseTime.Year);
            if (nextYear > baseTime.Year)
            {
                return new DateTime(nextYear, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }
            throw new InvalidOperationException("Provided cron string will not trigger.");
        }

        private static DateTime NextDayOfWeek(DateTime baseTime, int[] dayOWeek, bool lastDayOfWeek, out bool reset)
        {
            reset = true;
            foreach (int day in dayOWeek.OrderBy(n => n))
            {
                if (day == -1 || day == ((int)baseTime.DayOfWeek) + 1)
                {
                    if (lastDayOfWeek && !((new DateTime(baseTime.Year, baseTime.Month + 1, 1) - baseTime) <= TimeSpan.FromDays(7)))
                    {
                        continue;
                    }
                    reset = false;
                    return baseTime;
                }
            }
            return new DateTime(baseTime.Year, baseTime.Month, baseTime.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1);
        }

        private static DateTime NextMonth(DateTime baseTime, int[] months, out bool reset)
        {
            reset = true;
            foreach (int month in months.OrderBy(n => n))
            {
                if (month == -1 || month == baseTime.Month)
                {
                    reset = false;
                    return baseTime;
                }
                if (month > baseTime.Month)
                {
                    return new DateTime(baseTime.Year, month, 1, 0, 0, 0, DateTimeKind.Utc);
                }
            }
            return new DateTime(baseTime.Year, months.Min(), 1, 0, 0, 0, DateTimeKind.Utc).AddYears(1);
        }

        private static DateTime NextDay(DateTime baseTime, int[] days, bool lastDayOfMonth, out bool reset)
        {
            reset = true;
            if (lastDayOfMonth)
            {
                return new DateTime(baseTime.Year, baseTime.Month + 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(-1);
            }
            foreach (int day in days.OrderBy(n => n))
            {
                if (day == -1 || day == baseTime.Day)
                {
                    reset = false;
                    return baseTime;
                }
                if (day > baseTime.Day)
                {
                    return new DateTime(baseTime.Year, baseTime.Month, day, 0, 0, 0, DateTimeKind.Utc);
                }
            }
            return new DateTime(baseTime.Year, baseTime.Month, days.Min(), 0, 0, 0, DateTimeKind.Utc).AddMonths(1);
        }

        private static DateTime NextHour(DateTime baseTime, int[] hours, out bool reset)
        {
            reset = true;
            foreach (int hour in hours.OrderBy(n => n))
            {
                if (hour == -1 || hour == baseTime.Hour)
                {
                    reset = false;
                    return baseTime;
                }
                if (hour > baseTime.Hour)
                {
                    return new DateTime(baseTime.Year, baseTime.Month, baseTime.Day, hour, 0, 0, DateTimeKind.Utc);
                }
            }
            return new DateTime(baseTime.Year, baseTime.Month, baseTime.Day, hours.Min(), 0, 0, DateTimeKind.Utc).AddDays(1);
        }

        private static DateTime NextMinute(DateTime baseTime, int[] minutes)
        {
            int nextmin = -1;
            foreach (int min in minutes.OrderBy(n => n))
            {
                if (min == -1)
                {
                    return baseTime;
                }
                if (min >= baseTime.Minute)
                {
                    nextmin = min;
                    return baseTime.AddMinutes(nextmin - baseTime.Minute);
                }
            }
            return new DateTime(baseTime.Year, baseTime.Month, baseTime.Day, baseTime.Hour, minutes.Min(), 0, DateTimeKind.Utc).AddHours(1);
        }

        #endregion

        private static void ValidateTimePortionNumbers(int[] values, int minValue, int maxValue, string timePortion)
        {
            foreach (var value in values)
            {
                if (value != -1 && (value > maxValue || value < minValue))
                {
                    throw new InvalidOperationException(string.Format("{0} is not a valid {1} value", value, timePortion));
                }
            }
        }
    }
}
