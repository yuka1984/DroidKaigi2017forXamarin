#region

using System;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using TwoWayview.Layout;
using TwoWayView.Core;
using Exception = Java.Lang.Exception;
using Math = System.Math;

#endregion

namespace TwoWayView.Layout
{
	public class SpannableGridLayoutManager : GridLayoutManager
	{
		private static string LOGTAG = "SpannableGridLayoutManager";

		private static readonly int DEFAULT_NUM_COLS = 3;
		private static readonly int DEFAULT_NUM_ROWS = 3;
		private bool mMeasuring;

		public SpannableGridLayoutManager(Context context) : this(context, null)
		{
			;
		}

		public SpannableGridLayoutManager(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
			;
		}

		public SpannableGridLayoutManager(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle,
			DEFAULT_NUM_COLS, DEFAULT_NUM_ROWS)
		{
			;
		}

		public SpannableGridLayoutManager(Orientation orientation, int numColumns, int numRows) : base(orientation,
			numColumns, numRows)
		{
			;
		}

		private int getChildWidth(int colSpan)
		{
			return getLanes().getLaneSize() * colSpan;
		}

		private int getChildHeight(int rowSpan)
		{
			return getLanes().getLaneSize() * rowSpan;
		}

		private static int getLaneSpan(LayoutParams lp, bool isVertical)
		{
			return isVertical ? lp.ColSpan : lp.RowSpan;
		}

		private static int getLaneSpan(SpannableItemEntry entry, bool isVertical)
		{
			return isVertical ? entry.colSpan : entry.rowSpan;
		}


		public override bool CanScrollHorizontally()
		{
			return base.CanScrollHorizontally() && !mMeasuring;
		}

		public override bool CanScrollVertically()
		{
			return base.CanScrollVertically() && !mMeasuring;
		}

		public override int getLaneSpanForChild(View child)
		{
			return getLaneSpan((LayoutParams) child.LayoutParameters, isVertical());
		}

		public override int getLaneSpanForPosition(int position)
		{
			var entry = (SpannableItemEntry) getItemEntryForPosition(position);
			if (entry == null)
				throw new IllegalStateException("Could not find span for position " + position);

			return getLaneSpan(entry, isVertical());
		}


		public override void getLaneForPosition(Lanes.LaneInfo outInfo, int position, Direction direction)
		{
			var entry = (SpannableItemEntry) getItemEntryForPosition(position);
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

		private int getWidthUsed(View child)
		{
			var lp = (LayoutParams) child.LayoutParameters;
			return Width - PaddingLeft - PaddingRight - getChildWidth(lp.ColSpan);
		}

		private int getHeightUsed(View child)
		{
			var lp = (LayoutParams) child.LayoutParameters;
			return Height - PaddingTop - PaddingBottom - getChildHeight(lp.RowSpan);
		}

		protected override void measureChildWithMargins(View child)
		{
// XXX: This will disable scrolling while measuring this child to ensure that
// both width and height can use MATCH_PARENT properly.
			mMeasuring = true;
			MeasureChildWithMargins(child, getWidthUsed(child), getHeightUsed(child));
			mMeasuring = false;
		}


		public override void moveLayoutToPosition(int position, int offset, RecyclerView.Recycler recycler,
			RecyclerView.State state)
		{
			var isVertical = IsVertical;
			var lanes = getLanes();

			lanes.reset(0);

			for (var i = 0; i <= position; i++)
			{
				var entry = (SpannableItemEntry) getItemEntryForPosition(i);
				if (entry == null)
				{
					var child = recycler.GetViewForPosition(i);
					entry = (SpannableItemEntry) cacheChildLaneAndSpan(child, Direction.END);
				}

				mTempLaneInfo.set(entry.startLane, entry.anchorLane);

// The lanes might have been invalidated because an added or
// removed item. See BaseLayoutManager.invalidateItemLanes().
				if (mTempLaneInfo.isUndefined())
				{
					lanes.findLane(mTempLaneInfo, getLaneSpanForPosition(i), Direction.END);
					entry.setLane(mTempLaneInfo);
				}

				lanes.getChildFrame(mTempRect, getChildWidth(entry.colSpan),
					getChildHeight(entry.rowSpan), mTempLaneInfo, Direction.END);

				if (i != position)
					pushChildFrame(entry, mTempRect, entry.startLane, getLaneSpan(entry, isVertical),
						Direction.END);
			}
			try
			{
				lanes.getLane(mTempLaneInfo.startLane, mTempRect);
				lanes.reset(Direction.END);
				lanes.offset(offset - (isVertical ? mTempRect.Bottom : mTempRect.Right));
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.TraceWarning("moveLayoutToPosition", e);
			}
			
		}

		protected override ItemEntry cacheChildLaneAndSpan(View child, Direction direction)
		{
			var position = GetPosition(child);

			mTempLaneInfo.setUndefined();

			var entry = (SpannableItemEntry) getItemEntryForPosition(position);
			if (entry != null)
				mTempLaneInfo.set(entry.startLane, entry.anchorLane);

			if (mTempLaneInfo.isUndefined())
				getLaneForChild(mTempLaneInfo, child, direction);

			if (entry == null)
			{
				var lp = (LayoutParams) child.LayoutParameters;
				entry = new SpannableItemEntry(mTempLaneInfo.startLane, mTempLaneInfo.anchorLane,
					lp.ColSpan, lp.RowSpan);
				setItemEntryForPosition(position, entry);
			}
			else
			{
				entry.setLane(mTempLaneInfo);
			}

			return entry;
		}

		public override bool CheckLayoutParams(RecyclerView.LayoutParams lp)
		{
			if (lp.Width != ViewGroup.LayoutParams.MatchParent ||
			    lp.Height != ViewGroup.LayoutParams.MatchParent)
				return false;

			if (lp is LayoutParams)
			{
				var spannableLp = (LayoutParams) lp;

				if (isVertical())
					return spannableLp.RowSpan >= 1 && spannableLp.ColSpan >= 1 &&
					       spannableLp.ColSpan <= getLaneCount();
				return spannableLp.ColSpan >= 1 && spannableLp.RowSpan >= 1 &&
				       spannableLp.RowSpan <= getLaneCount();
			}

			return false;
		}

		public override RecyclerView.LayoutParams GenerateDefaultLayoutParams()
		{
			return new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
		}

		public override RecyclerView.LayoutParams GenerateLayoutParams(ViewGroup.LayoutParams lp)
		{
			var spannableLp = new LayoutParams((ViewGroup.MarginLayoutParams) lp);
			spannableLp.Width = ViewGroup.LayoutParams.MatchParent;
			spannableLp.Height = ViewGroup.LayoutParams.MatchParent;

			if (lp is LayoutParams)
			{
				var other = (LayoutParams) lp;
				if (isVertical())
				{
					spannableLp.ColSpan = Math.Max(1, Math.Min(other.ColSpan, getLaneCount()));
					spannableLp.RowSpan = Math.Max(1, other.RowSpan);
				}
				else
				{
					spannableLp.ColSpan = Math.Max(1, other.ColSpan);
					spannableLp.RowSpan = Math.Max(1, Math.Min(other.RowSpan, getLaneCount()));
				}
			}

			return spannableLp;
		}


		public override RecyclerView.LayoutParams GenerateLayoutParams(Context c, IAttributeSet attrs)
		{
			return new LayoutParams(c, attrs);
		}

		protected class SpannableItemEntry : ItemEntry
		{
			public static AnonymousIParcelableCreator<SpannableItemEntry> CREATOR
				= new AnonymousIParcelableCreator<SpannableItemEntry>(s => new SpannableItemEntry(s));

			public int colSpan;
			public int rowSpan;

			public SpannableItemEntry(int startLane, int anchorLane, int colSpan, int rowSpan) : base(startLane, anchorLane)
			{
				;
				this.colSpan = colSpan;
				this.rowSpan = rowSpan;
			}

			public SpannableItemEntry(Parcel @in) : base(@in)
			{
				;
				colSpan = @in.ReadInt();
				rowSpan = @in.ReadInt();
			}

			public override void WriteToParcel(Parcel @out, ParcelableWriteFlags flags)
			{
				base.WriteToParcel(@out, flags);
				@out.WriteInt(colSpan);
				@out.WriteInt(rowSpan);
			}
		}

		public class LayoutParams : RecyclerView.LayoutParams
		{
			private static readonly int DEFAULT_SPAN = 1;

			public LayoutParams(int width, int height) : this(width, height, DEFAULT_SPAN, DEFAULT_SPAN)
			{
			}

			public LayoutParams(int width, int height, int rowSpan, int colSpan) : base(width, height)
			{
				RowSpan = rowSpan;
				ColSpan = colSpan;
			}

			public LayoutParams(Context c, IAttributeSet attrs) : base(c, attrs)
			{
				var a = c.ObtainStyledAttributes(attrs, Resource.Styleable.twowayview_SpannableGridViewChild);
				ColSpan = Math.Max(
					DEFAULT_SPAN, a.GetInt(Resource.Styleable.twowayview_SpannableGridViewChild_twowayview_colSpan, -1));
				RowSpan = Math.Max(
					DEFAULT_SPAN, a.GetInt(Resource.Styleable.twowayview_SpannableGridViewChild_twowayview_rowSpan, -1));

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

			public int ColSpan { get; set; }
			public int RowSpan { get; set; }

			private void init(ViewGroup.LayoutParams other)
			{
				if (other is LayoutParams)
				{
					var lp = (LayoutParams) other;
					RowSpan = lp.RowSpan;
					ColSpan = lp.ColSpan;
				}
				else
				{
					RowSpan = DEFAULT_SPAN;
					ColSpan = DEFAULT_SPAN;
				}
			}
		}
	}
}