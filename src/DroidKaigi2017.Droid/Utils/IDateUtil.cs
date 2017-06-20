#region

using System;
using Java.Util;

#endregion

namespace DroidKaigi2017.Droid.Utils
{
	public interface IDateUtil
	{
		string GetHourMinute(Date date);
		string GetHourMinute(DateTime date);
		string GetLongFormatDate(Date date);
		string GetLongFormatDate(DateTime date);
		string GetMonthDate(Date date);
		string GetMonthDate(DateTime date);
	}
}