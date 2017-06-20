#region

using System;
using Android.Views;
using Object = Java.Lang.Object;

#endregion

namespace DroidKaigi2017.Droid.Views.Fragments
{
	public class AnonymousOnGestureListener : Object, GestureDetector.IOnGestureListener
	{
		public Func<MotionEvent, bool> OnDownAction { get; set; }

		public Func<MotionEvent, MotionEvent, float, float, bool> OnFlingAction { get; set; }

		public Action<MotionEvent> OnLongPressAction { get; set; }

		public Func<MotionEvent, MotionEvent, float, float, bool> OnScrollAction { get; set; }

		public Action<MotionEvent> OnShowPressAction { get; set; }

		public Func<MotionEvent, bool> OnSingleTapUpAction { get; set; }

		public bool OnDown(MotionEvent e)
		{
			return OnDownAction?.Invoke(e) ?? false;
		}

		public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			return OnFlingAction?.Invoke(e1, e2, velocityX, velocityY) ?? false;
		}

		public void OnLongPress(MotionEvent e)
		{
			OnLongPressAction?.Invoke(e);
		}

		public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			return OnScrollAction?.Invoke(e1, e2, distanceX, distanceY) ?? false;
		}

		public void OnShowPress(MotionEvent e)
		{
			OnShowPressAction?.Invoke(e);
		}

		public bool OnSingleTapUp(MotionEvent e)
		{
			return OnSingleTapUpAction?.Invoke(e) ?? false;
		}
	}
}