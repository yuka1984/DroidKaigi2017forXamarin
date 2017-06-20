#region

using System.Diagnostics;
using Android.Support.Design.Internal;
using Android.Support.Design.Widget;
using Java.Lang;

#endregion

namespace DroidKaigi2017.Droid.Views.helpers
{
	public static class BottomNavigationViewHelper
	{
		public static void DisableShiftingMode(BottomNavigationView view)
		{
			var menuView = (BottomNavigationMenuView) view.GetChildAt(0);
			try
			{
				var shiftingMode = menuView.Class.GetDeclaredField("mShiftingMode");
				shiftingMode.Accessible = true;
				shiftingMode.SetBoolean(menuView, false);
				shiftingMode.Accessible = false;
				for (var i = 0; i < menuView.ChildCount; i++)
				{
					var item = (BottomNavigationItemView) menuView.GetChildAt(i);
					item.SetShiftingMode(false);
					// Set once again checked value, so view will be updated
					item.SetChecked(item.ItemData.IsChecked);
				}
			}
			catch (NoSuchFieldException e)
			{
				Trace.TraceWarning("Unable to get shift mode field" + e);
			}
			catch (IllegalAccessException e)
			{
				Trace.TraceWarning("Unable to change value of shift mode" + e);
			}
		}
	}
}