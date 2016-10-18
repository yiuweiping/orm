using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Zhengdi.Framework
{
    public static class YuDateTime
    {
        /// <summary>
        /// unix时间转换为datetime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeToTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// datetime转换为unixtime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        public static string GetWeekName(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Friday:
                    return "星期五";
                case DayOfWeek.Monday:
                    return "星期一";
                case DayOfWeek.Saturday:
                    return "星期六";
                case DayOfWeek.Sunday:
                    return "星期天";
                case DayOfWeek.Thursday:
                    return "星期四";
                case DayOfWeek.Tuesday:
                    return "星期二";
                case DayOfWeek.Wednesday:
                    return "星三";
                default:
                    return "未知";
            }
        }
        public static string GetWeekNum(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Friday:
                    return "五";
                case DayOfWeek.Monday:
                    return "一";
                case DayOfWeek.Saturday:
                    return "六";
                case DayOfWeek.Sunday:
                    return "日";
                case DayOfWeek.Thursday:
                    return "四";
                case DayOfWeek.Tuesday:
                    return "二";
                case DayOfWeek.Wednesday:
                    return "三";
                default:
                    return "未知";
            }
        }
    }
}
