#region

using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Java.Lang;

#endregion

namespace TwoWayView.Core
{
	public abstract class ClickItemTouchListener : Object, RecyclerView.IOnItemTouchListener
	{
		private static string LOGTAG = "ClickItemTouchListener";

		private readonly GestureDetector mGestureDetector;


		public ClickItemTouchListener(RecyclerView hostView)
		{
			mGestureDetector = new ItemClickGestureDetector(hostView.Context, new ItemClickGestureListener(hostView, this));
		}

		public bool OnInterceptTouchEvent(RecyclerView recyclerView, MotionEvent @event)
		{
			if (!isAttachedToWindow(recyclerView) || !hasAdapter(recyclerView))
				return false;

			mGestureDetector.OnTouchEvent(@event);
			return false;
		}

		public void OnTouchEvent(RecyclerView recyclerView, MotionEvent @event)
		{
			// We can silently track tap and and long presses by silently
			// intercepting touch @events in the host RecyclerView.
		}

		public void OnRequestDisallowInterceptTouchEvent(bool disallow)
		{
		}

		private bool isAttachedToWindow(RecyclerView hostView)
		{
			if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
				return hostView.IsAttachedToWindow;
			return hostView.Handler != null;
		}

		private bool hasAdapter(RecyclerView hostView)
		{
			return hostView.GetAdapter() != null;
		}

		public abstract bool PerformItemClick(RecyclerView parent, View view, int position, long id);
		public abstract bool PerformItemLongClick(RecyclerView parent, View view, int position, long id);

		private class ItemClickGestureListener : GestureDetector.SimpleOnGestureListener
		{
			private readonly ClickItemTouchListener _owner;
			private readonly RecyclerView mHostView;
			private View mTargetChild;

			public ItemClickGestureListener(RecyclerView hostView, ClickItemTouchListener owner)
			{
				mHostView = hostView;
				_owner = owner;
			}

			public void dispatchSingleTapUpIfNeeded(MotionEvent @event)
			{
				// When the long press hook is called but the long press listener
				// returns false, the target child will be left around to be
				// handled later. In this case, we should still treat the gesture
				// as potential item click.
				if (mTargetChild != null)
					OnSingleTapUp(@event);
			}

			public override bool OnDown(MotionEvent @event)
			{
				var x = (int) @event.GetX();
				var y = (int) @event.GetY();

				mTargetChild = mHostView.FindChildViewUnder(x, y);
				return mTargetChild != null;
			}

			public override void OnShowPress(MotionEvent @event)
			{
				if (mTargetChild != null)
					mTargetChild.Pressed = true;
			}

			public override bool OnSingleTapUp(MotionEvent @event)
			{
				var handled = false;

				if (mTargetChild != null)
				{
					mTargetChild.Pressed = false;

					var position = mHostView.GetChildPosition(mTargetChild);
					var id = mHostView.GetAdapter().GetItemId(position);
					handled = _owner.PerformItemClick(mHostView, mTargetChild, position, id);

					mTargetChild = null;
				}

				return handled;
			}

			public override bool OnScroll(MotionEvent @event, MotionEvent event2, float v, float v2)
			{
				if (mTargetChild != null)
				{
					mTargetChild.Pressed = false;
					mTargetChild = null;

					return true;
				}

				return false;
			}

			public override void OnLongPress(MotionEvent @event)
			{
				if (mTargetChild == null)
					return;

				var position = mHostView.GetChildPosition(mTargetChild);
				var id = mHostView.GetAdapter().GetItemId(position);
				var handled = _owner.PerformItemLongClick(mHostView, mTargetChild, position, id);

				if (handled)
				{
					mTargetChild.Pressed = false;
					mTargetChild = null;
				}
			}
		}

		private class ItemClickGestureDetector : GestureDetector
		{
			private readonly ItemClickGestureListener mGestureListener;

			public ItemClickGestureDetector(Context context, ItemClickGestureListener listener) : base(context, listener)
			{
				mGestureListener = listener;
			}

			public override bool OnTouchEvent(MotionEvent @event)
			{
				var handled = base.OnTouchEvent(@event);

				var action = @event.Action & MotionEventActions.Mask;
				if (action == MotionEventActions.Up)
					mGestureListener.dispatchSingleTapUpIfNeeded(@event);

				return handled;
			}
		}
	}
}