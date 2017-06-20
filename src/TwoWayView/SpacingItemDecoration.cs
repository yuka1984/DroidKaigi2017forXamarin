#region

using System;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;

#endregion

namespace TwoWayView.Layout
{
	public class SpacingItemDecoration : RecyclerView.ItemDecoration
	{
		private readonly ItemSpacingOffsets mItemSpacing;

		public SpacingItemDecoration(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
			;
		}

		public SpacingItemDecoration(Context context, IAttributeSet attrs, int defStyle)
		{
			var a =
				context.ObtainStyledAttributes(attrs, Resource.Styleable.twowayview_SpacingItemDecoration, defStyle, 0);

			var verticalSpacing =
				Math.Max(0, a.GetInt(Resource.Styleable.twowayview_SpacingItemDecoration_android_verticalSpacing, 0));
			var horizontalSpacing =
				Math.Max(0, a.GetInt(Resource.Styleable.twowayview_SpacingItemDecoration_android_horizontalSpacing, 0));

			a.Recycle();

			mItemSpacing = new ItemSpacingOffsets(verticalSpacing, horizontalSpacing);
		}

		public SpacingItemDecoration(int verticalSpacing, int horizontalSpacing)
		{
			mItemSpacing = new ItemSpacingOffsets(verticalSpacing, horizontalSpacing);
		}

		public override void GetItemOffsets(Rect outRect, int itemPosition, RecyclerView parent)
		{
			mItemSpacing.getItemOffsets(outRect, itemPosition, parent);
		}
	}
}