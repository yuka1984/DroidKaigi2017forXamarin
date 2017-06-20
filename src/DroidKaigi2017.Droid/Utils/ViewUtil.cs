#region

using System;
using Android.Views;

#endregion

namespace DroidKaigi2017.Droid.Utils
{
	public class ViewUtil
	{
		public static readonly string Tag = typeof(ViewUtil).Name;

		public void AddOneTimeOnGlobalLayoutListener(View view, Func<bool> onGlobalLayout)
		{
			EventHandler hander = null;
			hander = (sender, args) =>
			{
				if (onGlobalLayout())
					view.ViewTreeObserver.GlobalLayout -= hander;
			};
			view.ViewTreeObserver.GlobalLayout += hander;
		}
	}
}