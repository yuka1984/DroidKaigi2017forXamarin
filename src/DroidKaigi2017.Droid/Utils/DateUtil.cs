#region

using System;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Text.Format;
using Java.Util;
using Java.Util.Concurrent;
using DateFormat = Android.Text.Format.DateFormat;
using String = Java.Lang.String;
using TimeZone = Java.Util.TimeZone;

#endregion

namespace DroidKaigi2017.Droid.Utils
{
	public static class DateUtilExtensions
	{
		public static Date ToJavaDate(this DateTime datetime)
		{
			return new Date(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second);
		}
	}

	public class DateUtil : IDateUtil
	{
		private const string FORMAT_MMDD = "MMMd";

		private const string FORMAT_KKMM = "kk:mm";

		private const string FORMAT_YYYYMMDDKKMM = "yyyyMMMdkkmm";

		private const string FORMAT_PROGRAM_START_DATE = "MM/dd(E) kk:mm";

		private readonly Context _context;

		public DateUtil(Context context)
		{
			_context = context;
		}


		public string GetMonthDate(DateTime date)
		{
			return GetMonthDate(date.ToJavaDate());
		}

		public string GetMonthDate(Date date)
		{
			if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2)
			{
				var pattern = DateFormat.GetBestDateTimePattern(Locale.Default, FORMAT_MMDD);
				return new SimpleDateFormat(pattern, Locale.Default).Format(date);
			}
			var flag = FormatStyleFlags.AbbrevAll | FormatStyleFlags.NoYear;
			return DateUtils.FormatDateTime(_context, date.Time, flag);
		}

		public string GetHourMinute(DateTime date)
		{
			return GetHourMinute(date.ToJavaDate());
		}

		public string GetHourMinute(Date date)
		{
			if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2)
			{
				var pattern = DateFormat.GetBestDateTimePattern(Locale.Default, FORMAT_KKMM);
				return new SimpleDateFormat(pattern, Locale.Default).Format(date);
			}
			return String.ValueOf(DateFormat.Format(FORMAT_KKMM, date));
		}

		public string GetLongFormatDate(DateTime date)
		{
			return GetLongFormatDate(date.ToJavaDate());
		}

		public string GetLongFormatDate(Date date)
		{
			if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2)
			{
				var pattern = DateFormat.GetBestDateTimePattern(Locale.Default, FORMAT_YYYYMMDDKKMM);
				return new SimpleDateFormat(pattern, Locale.Default).Format(date);
			}
			var dayOfWeekFormat =
				Java.Text.DateFormat.GetDateInstance(Java.Text.DateFormat.Long);
			var shortTimeFormat =
				Java.Text.DateFormat.GetTimeInstance(Java.Text.DateFormat.Short);
			dayOfWeekFormat.TimeZone = TimeZone.Default;
			shortTimeFormat.TimeZone = TimeZone.Default;
			return dayOfWeekFormat.Format(date) + " " + shortTimeFormat.Format(date);
		}

		public static int GetMinutes(Date stime, Date etime)
		{
			var range = etime.Time - stime.Time;

			if (range > 0)
				return (int) (range / TimeUnit.Minutes.ToMillis(1L));
			return 0;
		}
	}
}