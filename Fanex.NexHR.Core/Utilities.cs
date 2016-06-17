namespace Fanex.NexHR.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    public static class Utilities
    {
        public static IList<SelectListItem> CheckInOutTypes
        {
            get
            {
                return new List<SelectListItem>
                    {
                        new SelectListItem() { Text = Labels.CheckIn, Value = ((short)RequestType.CheckIn).ToString() },
                        new SelectListItem() { Text = Labels.CheckOut, Value = ((short)RequestType.CheckOut).ToString() }
                    };
            }
        }

        public static IList<SelectListItem> LateInEarlyOutTypes
        {
            get
            {
                return new List<SelectListItem>
                    {
                        new SelectListItem() { Text = Labels.LateIn, Value = ((short)RequestType.LateIn).ToString() },
                        new SelectListItem() { Text = Labels.EarlyOut, Value = ((short)RequestType.EarlyOut).ToString() }
                    };
            }
        }

        public static IList<SelectListItem> LocalLeaveTypes
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem() { Text = Labels.StandardLeave, Value = ((short)LocalLeave.StandardLeave).ToString() },
                    new SelectListItem() { Text = Labels.AccumulationLeave, Value = ((short)LocalLeave.AccumulationLeave).ToString() },
                    new SelectListItem() { Text = Labels.KeepInViewLeave, Value = ((short)LocalLeave.KIVLeave).ToString() }
                };
            }
        }

        public static IList<SelectListItem> Marriages
        {
            get
            {
                return new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Text = "--Select Marriage Status--",
                            Value = ((byte)MarriageStatus.Default).ToString()
                        },
                        new SelectListItem
                        {
                            Text = MarriageStatus.Married.ToString(),
                            Value = ((byte)MarriageStatus.Married).ToString()
                        },
                        new SelectListItem
                        {
                            Text = MarriageStatus.Divorced.ToString(),
                            Value = ((byte)MarriageStatus.Divorced).ToString()
                        },
                        new SelectListItem
                        {
                            Text = MarriageStatus.Single.ToString(),
                            Value = ((byte)MarriageStatus.Single).ToString()
                        },
                        new SelectListItem
                        {
                            Text = MarriageStatus.Widow.ToString(),
                            Value = ((byte)MarriageStatus.Widow).ToString()
                        }
                    };
            }
        }

        public static DateTime AddTimeToDate(DateTime dateTime, string time)
        {
            var times = time.Split(new[] { ':' });
            TimeSpan ts = new TimeSpan(int.Parse(times[0]), int.Parse(times[1]), 0);
            return dateTime.Date + ts;
        }

        public static DateTime AddTimeToDate(DateTime dateTime, int hours, int minutes, int seconds)
        {
            return dateTime.Date.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
        }

        public static string GetTime(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }

            var value = (DateTime)dateTime;
            return ZeroPrefix(value.Hour) + ":" + ZeroPrefix(value.Minute);
        }

        public static void CopyTo(this object s, object t)
        {
            foreach (var pS in s.GetType().GetProperties())
            {
                foreach (var pT in t.GetType().GetProperties().Where(pT => pT.Name == pS.Name))
                {
                    pT.GetSetMethod().Invoke(t, new[] { pS.GetGetMethod().Invoke(s, null) });
                }
            }
        }

        public static string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        public static string RandomString(int length)
        {
            // returns random string of length specified with characters including: 0-9, a-z, A-Z
            var ca = new char[length];
            var random = new byte[length];

            // RNGCryptoServiceProvider is an implementation of a random number generator.
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(random); // The array is now filled with cryptographically strong random bytes.

            for (var i = 0; i < length; i++)
            {
                var found = false;
                var rand = (int)random[i];
                while (!found)
                {
                    if (((rand >= 48) && (rand <= 57)) || ((rand >= 65) && (rand <= 90)) || ((rand >= 97) && (rand <= 122)))
                    {
                        found = true;
                    }
                    else
                    {
                        // get a new random int.
                        var single = new byte[1];
                        rng.GetBytes(single);
                        rand = single[0];
                    }
                }

                var ci = (char)rand;
                ca[i] = ci;
            }

            return new string(ca).ToLower();
        }

        public static DateTime FirstDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo = null)
        {
            if (cultureInfo == null)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
            {
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            }

            return firstDayInWeek;
        }

        public static DateTime LastDayOfWeek(DateTime dayInWeek)
        {
            return DateTime.Now.AddDays(6 - Convert.ToDouble(dayInWeek.DayOfWeek));
        }

        public static IList<SelectListItem> MinuteSteps(int step)
        {
            var list = new List<SelectListItem>();
            var index = 60;
            while (index > 0)
            {
                list.Add(new SelectListItem()
                {
                    Text = index + "'",
                    Value = index.ToString()
                });
                index -= step;
            }

            return list;
        }

        public static IList<SelectListItem> TimeSteps(int minuteStep)
        {
            var list = new List<SelectListItem>();
            var hour = 0;
            var minute = 0;

            while (hour < 24)
            {
                var value = ZeroPrefix(hour) + ":" + ZeroPrefix(minute);
                list.Add(new SelectListItem()
                {
                    Text = value,
                    Value = value
                });
                minute += minuteStep;
                if (minute >= 60)
                {
                    minute = 0;
                    hour++;
                }
            }

            return list;
        }

        public static IList<SelectListItem> TimeSteps(int minuteStep, string selectedItem)
        {
            var list = new List<SelectListItem>();
            var hour = 0;
            var minute = 0;

            while (hour < 24)
            {
                var value = ZeroPrefix(hour) + ":" + ZeroPrefix(minute);
                list.Add(new SelectListItem()
                {
                    Text = value,
                    Value = value,
                    Selected = value == selectedItem
                });
                minute += minuteStep;
                if (minute >= 60)
                {
                    minute = 0;
                    hour++;
                }
            }

            return list;
        }

        public static IList<SelectListItem> LateInOutTimeSteps(int minuteStep)
        {
            var list = new List<SelectListItem>();
            var hour = 0;
            var minute = 15;

            while (hour < 1)
            {
                var value = ZeroPrefix(minute);
                list.Add(new SelectListItem()
                {
                    Text = value,
                    Value = value
                });
                minute += minuteStep;
                if (minute > 150)
                {
                    hour++;
                }
            }

            return list;
        }

        public static string ZeroPrefix(int value)
        {
            return value < 10 ? "0" + value : value.ToString();
        }

        public static string ZeroPrefix(string value)
        {
            return int.Parse(value) < 10 ? "0" + value : value;
        }

        public static string StringArrayToString(string[] array)
        {
            if (array == null || array.Length == 0)
            {
                return string.Empty;
            }

            string result = string.Empty;

            foreach (var item in array)
            {
                result += item + ",";
            }

            return result.Substring(0, result.Length - 1);
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(email);
        }

        public static string TotalHoursToTime(float value)
        {
            int hour = (int)value,
                minute = (int)Math.Round((value - hour) * 60);

            return hour > 0
                ? ZeroPrefix(hour) + "h" + ZeroPrefix(minute) + "'"
                : ZeroPrefix(minute) + "'";
        }

        public static string MinuteToTime(int value)
        {
            if (value == 0)
            {
                return string.Empty;
            }

            int hour = value / 60,
                minute = value - (hour * 60);

            return hour > 0
                ? ZeroPrefix(hour) + "h" + ZeroPrefix(minute) + "'"
                : ZeroPrefix(minute) + "'";
        }

        public static string FormatString(string input)
        {
            return string.IsNullOrEmpty(input) ? string.Empty : input;
        }

        public static bool CheckUserCanEditRequest(int currentUserId, int employeeId)
        {
            return currentUserId == employeeId;
        }

        public static string GetCurrentStatusName(int statusId)
        {
            var statusName = Labels.NA;
            switch (statusId) 
            { 
                case 1:
                    statusName = Labels.Pending;
                    break;
                case 2:
                    statusName = Labels.Verified;
                    break;
                case 3:
                    statusName = Labels.NotVerified;
                    break;
                case 4:
                    statusName = Labels.Approved;
                    break;
                case 5:
                    statusName = Labels.Rejected;
                    break;
                case 6:
                    statusName = Labels.Processed;
                    break;
                case 7:
                    statusName = Labels.Void;
                    break;
            }

            return statusName;
        }
    }
}