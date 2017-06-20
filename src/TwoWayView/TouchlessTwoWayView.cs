#region

using Android.Content;
using Android.Util;
using Android.Views;

#endregion

namespace TwoWayView.Layout
{
	public class TouchlessTwoWayView : TwoWayView
	{
		public TouchlessTwoWayView(Context context) : base(context)
		{
		}

		public TouchlessTwoWayView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public TouchlessTwoWayView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
		}

		public override bool DispatchTouchEvent(MotionEvent e)
		{
			return base.DispatchTouchEvent(e);
		}

		public bool ForceToDispatchTouchEvent(MotionEvent ev)
		{
			return base.DispatchTouchEvent(ev);
		}
	}
}