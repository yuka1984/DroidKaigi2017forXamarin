#region

using System;
using Android.Content;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Views.Animations;

#endregion

namespace DroidKaigi2017.Droid.Views.CustomViews
{
	public class OverScrollLayout : CoordinatorLayout
	{
		private static readonly float OVER_SCROLL_THRESHOLD_RATIO = 0.20f;

		private static readonly int RESTORE_ANIM_DURATION = 100;

		private static readonly float MINIMUM_OVER_SCROLL_SCALE = 0.99f;

		private readonly Rect _originalRect = new Rect();

		private IOnOverScrollListener _overScrollListener;

		private int _overScrollThreshold;

		public OverScrollLayout(Context context) : base(context)
		{
			;
		}

		public OverScrollLayout(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			;
		}

		public OverScrollLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
			;
		}

		protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout(changed, left, top, right, bottom);
			_originalRect.Set(left, top, right, bottom);
			_overScrollThreshold = (int) (_originalRect.Height() * OVER_SCROLL_THRESHOLD_RATIO);
		}

		public override void OnNestedPreScroll(View target, int dx, int dy, int[] consumed)
		{
			if (GetY() != _originalRect.Top)
			{
				var scale =
					Math.Max(MINIMUM_OVER_SCROLL_SCALE,
						1 - Math.Abs(GetY()) / _overScrollThreshold * (1 - MINIMUM_OVER_SCROLL_SCALE));
				ScaleX = scale;
				ScaleY = scale;
				TranslationY(-dy);
				consumed[1] = dy;
			}
			else
			{
				base.OnNestedPreScroll(target, dx, dy, consumed);
			}
		}

		public override void OnNestedScroll(View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed)
		{
			base.OnNestedScroll(target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed);
			AppBarLayout appBarLayout = null;
			var scrollTop = false;
			var scrollEnd = false;
			for (var i = 0; i < ChildCount; i++)
			{
				var view = GetChildAt(i);
				if (view is AppBarLayout)
				{
					appBarLayout = (AppBarLayout) view;
					continue;
				}
				if (view is NestedScrollView)
				{
					scrollTop = !view.CanScrollVertically(-1);
					scrollEnd = !view.CanScrollVertically(1);
				}
			}

			if (appBarLayout == null
			    || scrollTop && dyUnconsumed < 0 && IsAppBarExpanded(appBarLayout)
			    || scrollEnd && dyUnconsumed > 0 && IsAppBarCollapsed(appBarLayout)) TranslationY(-dyUnconsumed);
		}

		public override bool OnNestedPreFling(View target, float velocityX, float velocityY)
		{
			return GetY() != _originalRect.Top || base.OnNestedPreFling(target, velocityX, velocityY);
		}

		public override void OnStopNestedScroll(View target)
		{
			base.OnStopNestedScroll(target);
			if (Math.Abs(GetY()) > _overScrollThreshold)
			{
				float yTranslation;
				yTranslation = _originalRect.Top + GetY() > 0 ? _originalRect.Height() : -_originalRect.Height();
				Animate()
					.SetDuration(Context.Resources.GetInteger(Resource.Integer.activity_transition_mills))
					.SetInterpolator(new AccelerateDecelerateInterpolator())
					.Alpha(0)
					.TranslationY(yTranslation)
					.SetListener(new AnonymousAnimatorListener
					{
						OnAnimationEndAction = animator =>
						{
							if (_overScrollListener != null)
								_overScrollListener.OnOverScroll();
						}
					});
				return;
			}
			if (GetY() != _originalRect.Top)
				Animate()
					.SetDuration(RESTORE_ANIM_DURATION)
					.SetInterpolator(new AccelerateDecelerateInterpolator())
					.TranslationY(_originalRect.Top)
					.ScaleX(1)
					.ScaleY(1);
		}

		private void TranslationY(float dy)
		{
			SetY(GetY() + dy * 0.5f);
		}

		private bool IsAppBarExpanded(AppBarLayout appBarLayout)
		{
			return appBarLayout.Top == 0;
		}

		private bool IsAppBarCollapsed(AppBarLayout appBarLayout)
		{
			return appBarLayout.GetY() == -appBarLayout.TotalScrollRange;
		}

		public void SetOverScrollListener(IOnOverScrollListener listener)
		{
			_overScrollListener = listener;
		}

		public interface IOnOverScrollListener
		{
			void OnOverScroll();
		}
		public class AnonymousOnOverScrillListner : IOnOverScrollListener
		{ 
			public Action OnOverScrollAction { get; set; }
			public void OnOverScroll()
			{
				OnOverScrollAction?.Invoke();
			}
		}
	}
}