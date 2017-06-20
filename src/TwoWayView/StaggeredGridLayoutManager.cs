#region

using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using TwoWayview.Layout;
using TwoWayView.Core;
using Math = System.Math;

#endregion

namespace TwoWayView.Layout
{
	public class StaggeredGridLayoutManager : GridLayoutManager
	{
		private static string LOGTAG = "StaggeredGridLayoutManager";

		private static readonly int DEFAULT_NUM_COLS = 2;
		private static readonly int DEFAULT_NUM_ROWS = 2;


		public StaggeredGridLayoutManager(Context context) : this(context, null)
		{
			;
		}

		public StaggeredGridLayoutManager(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
			;
		}

		public StaggeredGridLayoutManager(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle,
			DEFAULT_NUM_COLS, DEFAULT_NUM_ROWS)
		{
			;
		}

		public StaggeredGridLayoutManager(Orientation orientation, int numColumns, int numRows) : base(orientation,
			numColumns, numRows)
		{
			;
		}

		public override int getLaneSpanForChild(View child)
		{
			var lp = (LayoutParams) child.LayoutParameters;
			return lp.span;
		}


		public override int getLaneSpanForPosition(int position)
		{
			var entry = (StaggeredItemEntry) getItemEntryForPosition(position);
			if (entry == null)
				throw new IllegalStateException("Could not find span for position " + position);

			return entry.span;
		}

		public override void getLaneForPosition(Lanes.LaneInfo outInfo, int position, Direction direction)
		{
			var entry = (StaggeredItemEntry) getItemEntryForPosition(position);
			if (entry != null)
			{
				outInfo.set(entry.startLane, entry.anchorLane);
				return;
			}

			outInfo.setUndefined();
		}


		protected override void getLaneForChild(Lanes.LaneInfo outInfo, View child, Direction direction)
		{
			base.getLaneForChild(outInfo, child, direction);
			if (outInfo.isUndefined())
				getLanes().findLane(outInfo, getLaneSpanForChild(child), direction);
		}

		public override void moveLayoutToPosition(int position, int offset, RecyclerView.Recycler recycler,
			RecyclerView.State state)
		{
			var isVertical = IsVertical;
			var lanes = getLanes();

			lanes.reset(0);

			for (var i = 0; i <= position; i++)
			{
				var entry = (StaggeredItemEntry) getItemEntryForPosition(i);

				if (entry != null)
				{
					mTempLaneInfo.set(entry.startLane, entry.anchorLane);

// The lanes might have been invalidated because an added or
// removed item. See BaseLayoutManager.invalidateItemLanes().
					if (mTempLaneInfo.isUndefined())
					{
						lanes.findLane(mTempLaneInfo, getLaneSpanForPosition(i), Direction.END);
						entry.setLane(mTempLaneInfo);
					}

					lanes.getChildFrame(mTempRect, entry.width, entry.height, mTempLaneInfo,
						Direction.END);
				}
				else
				{
					var child = recycler.GetViewForPosition(i);

// XXX: This might potentially cause stalls in the main
// thread if the layout ends up having to measure tons of
// child views. We might need to add different policies based
// on known assumptions regarding certain layouts e.g. child
// views have stable aspect ratio, lane size is fixed, etc.
					MeasureChild(child, Direction.END);

// The measureChild() call ensures an entry is created for
// this position.
					entry = (StaggeredItemEntry) getItemEntryForPosition(i);

					mTempLaneInfo.set(entry.startLane, entry.anchorLane);
					lanes.getChildFrame(mTempRect, GetDecoratedMeasuredWidth(child),
						GetDecoratedMeasuredHeight(child), mTempLaneInfo, Direction.END);

					cacheItemFrame(entry, mTempRect);
				}

				if (i != position)
					pushChildFrame(entry, mTempRect, entry.startLane, entry.span, Direction.END);
			}

			lanes.getLane(mTempLaneInfo.startLane, mTempRect);
			lanes.reset(Direction.END);
			lanes.offset(offset - (isVertical ? mTempRect.Bottom : mTempRect.Right));
		}

		protected override ItemEntry cacheChildLaneAndSpan(View child, Direction direction)
		{
			var position = GetPosition(child);

			mTempLaneInfo.setUndefined();

			var entry = (StaggeredItemEntry) getItemEntryForPosition(position);
			if (entry != null)
				mTempLaneInfo.set(entry.startLane, entry.anchorLane);

			if (mTempLaneInfo.isUndefined())
				getLaneForChild(mTempLaneInfo, child, direction);

			if (entry == null)
			{
				entry = new StaggeredItemEntry(mTempLaneInfo.startLane, mTempLaneInfo.anchorLane,
					getLaneSpanForChild(child));
				setItemEntryForPosition(position, entry);
			}
			else
			{
				entry.setLane(mTempLaneInfo);
			}

			return entry;
		}

		private void cacheItemFrame(StaggeredItemEntry entry, Rect childFrame)
		{
			entry.width = childFrame.Right - childFrame.Left;
			entry.height = childFrame.Bottom - childFrame.Top;
		}

		protected override ItemEntry cacheChildFrame(View child, Rect childFrame)
		{
			var entry = (StaggeredItemEntry) getItemEntryForPosition(GetPosition(child));
			if (entry == null)
				throw new IllegalStateException("Tried to cache frame on undefined item");

			cacheItemFrame(entry, childFrame);
			return entry;
		}

		public override bool CheckLayoutParams(RecyclerView.LayoutParams lp)
		{
			var result = base.CheckLayoutParams(lp);
			if (lp is LayoutParams)
			{
				var staggeredLp = (LayoutParams) lp;
				result &= staggeredLp.span >= 1 && staggeredLp.span <= getLaneCount();
			}

			return result;
		}

		public override RecyclerView.LayoutParams GenerateDefaultLayoutParams()
		{
			if (isVertical())
				return new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			return new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
		}


		public override RecyclerView.LayoutParams GenerateLayoutParams(ViewGroup.LayoutParams lp)
		{
			var staggeredLp = new LayoutParams((ViewGroup.MarginLayoutParams) lp);
			if (isVertical())
			{
				staggeredLp.Width = ViewGroup.LayoutParams.MatchParent;
				staggeredLp.Height = lp.Height;
			}
			else
			{
				staggeredLp.Width = lp.Width;
				staggeredLp.Height = ViewGroup.LayoutParams.MatchParent;
			}

			if (lp is LayoutParams)
			{
				var other = (LayoutParams) lp;
				staggeredLp.span = Math.Max(1, Math.Min(other.span, getLaneCount()));
			}

			return staggeredLp;
		}


		public override RecyclerView.LayoutParams GenerateLayoutParams(Context c, IAttributeSet attrs)
		{
			return new LayoutParams(c, attrs);
		}

		protected class StaggeredItemEntry : ItemEntry
		{
			internal int height;

			internal int span;
			internal int width;

			public StaggeredItemEntry(int startLane, int anchorLane, int span) : base(startLane, anchorLane)
			{
				;
				this.span = span;
			}

			public StaggeredItemEntry(Parcel @in) : base(@in)
			{
				;
				span = @in.ReadInt();
				width = @in.ReadInt();
				height = @in.ReadInt();
			}

			public override void WriteToParcel(Parcel @out, ParcelableWriteFlags flags)
			{
				base.WriteToParcel(@out, flags);
				@out.WriteInt(span);
				@out.WriteInt(width);
				@out.WriteInt(height);
			}

			//	TODO: Creator
			/*
				public static Parcelable.Creator<StaggeredItemEntry> CREATOR

				= new Parcelable.Creator<StaggeredItemEntry>() {
					@Override

					public StaggeredItemEntry createFromParcel(Parcel @in)
					{
						return new StaggeredItemEntry(in);
					}

					@Override
					public StaggeredItemEntry[] newArray(int size)
					{
						return new StaggeredItemEntry[size];
					}
				};
				*/
		}

		public class LayoutParams : RecyclerView.LayoutParams
		{
			private static readonly int DEFAULT_SPAN = 1;

			public int span;

			public LayoutParams(int width, int height) : base(width, height)
			{
				;
				span = DEFAULT_SPAN;
			}

			public LayoutParams(Context c, IAttributeSet attrs) : base(c, attrs)
			{
				;

				var a = c.ObtainStyledAttributes(attrs, Resource.Styleable.twowayview_StaggeredGridViewChild);
				span = Math.Max(DEFAULT_SPAN, a.GetInt(Resource.Styleable.twowayview_StaggeredGridViewChild_twowayview_span, -1));
				a.Recycle();
			}

			public LayoutParams(ViewGroup.LayoutParams other) : base(other)
			{
				;
				init(other);
			}

			public LayoutParams(ViewGroup.MarginLayoutParams other) : base(other)
			{
				;
				init(other);
			}

			private void init(ViewGroup.LayoutParams other)
			{
				if (other is LayoutParams)
				{
					var lp = (LayoutParams) other;
					span = lp.span;
				}
				else
				{
					span = DEFAULT_SPAN;
				}
			}
		}
	}
}