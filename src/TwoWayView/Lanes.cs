#region

using System;
using Android.Graphics;
using Android.Widget;
using TwoWayView.Core;
using TwoWayView.Layout;

#endregion

namespace TwoWayview.Layout
{
	public class Lanes
	{
		public static int NO_LANE = -1;
		private int? mInnerEnd;

		private int? mInnerStart;
		private readonly bool mIsVertical;
		private readonly Rect[] mLanes;
		private readonly int mLaneSize;

		private BaseLayoutManager mLayout;
		private readonly Rect[] mSavedLanes;
		private readonly LaneInfo mTempLaneInfo = new LaneInfo();

		private readonly Rect mTempRect = new Rect();

		public Lanes(BaseLayoutManager layout, Orientation orientation, Rect[] lanes, int laneSize)
		{
			mLayout = layout;
			mIsVertical = orientation == Orientation.Vertical;
			mLanes = lanes;
			mLaneSize = laneSize;

			mSavedLanes = new Rect[mLanes.Length];
			for (var i = 0; i < mLanes.Length; i++)
				mSavedLanes[i] = new Rect();
		}

		public Lanes(BaseLayoutManager layout, int laneCount)
		{
			mLayout = layout;
			mIsVertical = layout.isVertical();

			mLanes = new Rect[laneCount];
			mSavedLanes = new Rect[laneCount];
			for (var i = 0; i < laneCount; i++)
			{
				mLanes[i] = new Rect();
				mSavedLanes[i] = new Rect();
			}

			mLaneSize = calculateLaneSize(layout, laneCount);

			var paddingLeft = layout.PaddingLeft;
			var paddingTop = layout.PaddingTop;

			for (var i = 0; i < laneCount; i++)
			{
				var laneStart = i * mLaneSize;

				var l = paddingLeft + (mIsVertical ? laneStart : 0);
				var t = paddingTop + (mIsVertical ? 0 : laneStart);
				var r = mIsVertical ? l + mLaneSize : l;
				var b = mIsVertical ? t : t + mLaneSize;

				mLanes[i].Set(l, t, r, b);
			}
		}

		public int Count => mLanes.Length;

		public static int calculateLaneSize(BaseLayoutManager layout, int laneCount)
		{
			if (layout.isVertical())
			{
				var paddingLeft = layout.PaddingLeft;
				var paddingRight = layout.PaddingRight;
				var width = layout.Width - paddingLeft - paddingRight;
				return width / laneCount;
			}
			var paddingTop = layout.PaddingTop;
			var paddingBottom = layout.PaddingBottom;
			var height = layout.Height - paddingTop - paddingBottom;
			return height / laneCount;
		}

		private void invalidateEdges()
		{
			mInnerStart = null;
			mInnerEnd = null;
		}

		public Orientation getOrientation()
		{
			return mIsVertical ? Orientation.Vertical : Orientation.Horizontal;
		}

		public void save()
		{
			for (var i = 0; i < mLanes.Length; i++)
				mSavedLanes[i].Set(mLanes[i]);
		}

		public void restore()
		{
			for (var i = 0; i < mLanes.Length; i++)
				mLanes[i].Set(mSavedLanes[i]);
		}

		public int getLaneSize()
		{
			return mLaneSize;
		}

		public int getCount()
		{
			return mLanes.Length;
		}

		private void offsetLane(int lane, int offset)
		{
			try
			{
				mLanes[lane]
					.Offset(mIsVertical ? 0 : offset,
						mIsVertical ? offset : 0);
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.TraceWarning("offsetLane" , e);
			}
		}

		public void offset(int offset)
		{
			for (var i = 0; i < mLanes.Length; i++)
				this.offset(i, offset);

			invalidateEdges();
		}

		public void offset(int lane, int offset)
		{
			offsetLane(lane, offset);
			invalidateEdges();
		}

		public void getLane(int lane, Rect laneRect)
		{
			try
			{
				laneRect.Set(mLanes[lane]);
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.TraceWarning(e.ToString());
			}
			
		}

		public int pushChildFrame(Rect outRect, int lane, int margin, Direction direction)
		{
			int delta;

			var laneRect = mLanes[lane];
			if (mIsVertical)
			{
				if (direction == Direction.END)
				{
					delta = outRect.Top - laneRect.Bottom;
					laneRect.Bottom = outRect.Bottom + margin;
				}
				else
				{
					delta = outRect.Bottom - laneRect.Top;
					laneRect.Top = outRect.Top - margin;
				}
			}
			else
			{
				if (direction == Direction.END)
				{
					delta = outRect.Left - laneRect.Right;
					laneRect.Right = outRect.Right + margin;
				}
				else
				{
					delta = outRect.Right - laneRect.Left;
					laneRect.Left = outRect.Left - margin;
				}
			}

			invalidateEdges();

			return delta;
		}

		public void popChildFrame(Rect outRect, int lane, int margin, Direction direction)
		{
			var laneRect = mLanes[lane];
			if (mIsVertical)
			{
				if (direction == Direction.END)
					laneRect.Top = outRect.Bottom - margin;
				else
					laneRect.Bottom = outRect.Top + margin;
			}
			else
			{
				if (direction == Direction.END)
					laneRect.Left = outRect.Right - margin;
				else
					laneRect.Right = outRect.Left + margin;
			}

			invalidateEdges();
		}

		public void getChildFrame(Rect outRect, int childWidth, int childHeight, LaneInfo laneInfo,
			Direction direction)
		{
			var startRect = mLanes[laneInfo.startLane];

			// The anchor lane only applies when we're get child frame in the direction
			// of the forward scroll. We'll need to rethink this once we start working on
			// RTL support.
			var anchorLane =
				direction == Direction.END ? laneInfo.anchorLane : laneInfo.startLane;
			var anchorRect = mLanes[anchorLane];

			if (mIsVertical)
			{
				outRect.Left = startRect.Left;
				outRect.Top =
					direction == Direction.END ? anchorRect.Bottom : anchorRect.Top - childHeight;
			}
			else
			{
				outRect.Top = startRect.Top;
				outRect.Left =
					direction == Direction.END ? anchorRect.Right : anchorRect.Left - childWidth;
			}

			outRect.Right = outRect.Left + childWidth;
			outRect.Bottom = outRect.Top + childHeight;
		}

		private bool intersects(int start, int count, Rect r)
		{
			for (var l = start; l < start + count; l++)
				if (Rect.Intersects(mLanes[l], r))
					return true;

			return false;
		}

		private int findLaneThatFitsSpan(int anchorLane, int laneSpan, Direction direction)
		{
			var findStart = Math.Max(0, anchorLane - laneSpan + 1);
			var findEnd = Math.Min(findStart + laneSpan, mLanes.Length - laneSpan + 1);
			for (var l = findStart; l < findEnd; l++)
			{
				mTempLaneInfo.set(l, anchorLane);

				getChildFrame(mTempRect, mIsVertical ? laneSpan * mLaneSize : 1,
					mIsVertical ? 1 : laneSpan * mLaneSize, mTempLaneInfo, direction);

				if (!intersects(l, laneSpan, mTempRect))
					return l;
			}

			return NO_LANE;
		}

		public void findLane(LaneInfo outInfo, int laneSpan, Direction direction)
		{
			outInfo.setUndefined();

			var targetEdge = direction == Direction.END ? int.MaxValue : int.MinValue;
			for (var l = 0; l < mLanes.Length; l++)
			{
				int laneEdge;
				if (mIsVertical)
					laneEdge = direction == Direction.END ? mLanes[l].Bottom : mLanes[l].Top;
				else
					laneEdge = direction == Direction.END ? mLanes[l].Right : mLanes[l].Left;

				if (direction == Direction.END && laneEdge < targetEdge ||
				    direction == Direction.START && laneEdge > targetEdge)
				{
					var targetLane = findLaneThatFitsSpan(l, laneSpan, direction);
					if (targetLane != NO_LANE)
					{
						targetEdge = laneEdge;
						outInfo.set(targetLane, l);
					}
				}
			}
		}

		public void reset(Direction direction)
		{
			for (var i = 0; i < mLanes.Length; i++)
			{
				var laneRect = mLanes[i];
				if (mIsVertical)
				{
					if (direction == Direction.START)
						laneRect.Bottom = laneRect.Top;
					else
						laneRect.Top = laneRect.Bottom;
				}
				else
				{
					if (direction == Direction.START)
						laneRect.Right = laneRect.Left;
					else
						laneRect.Left = laneRect.Right;
				}
			}

			invalidateEdges();
		}

		public void reset(int offset)
		{
			for (var i = 0; i < mLanes.Length; i++)
			{
				var laneRect = mLanes[i];

				laneRect.OffsetTo(mIsVertical ? laneRect.Left : offset,
					mIsVertical ? offset : laneRect.Top);

				if (mIsVertical)
					laneRect.Bottom = laneRect.Top;
				else
					laneRect.Right = laneRect.Left;
			}

			invalidateEdges();
		}

		public int getInnerStart()
		{
			if (mInnerStart != null)
				return mInnerStart.Value;

			mInnerStart = int.MinValue;
			for (var i = 0; i < mLanes.Length; i++)
			{
				var laneRect = mLanes[i];
				mInnerStart = Math.Max(mInnerStart.Value, mIsVertical ? laneRect.Top : laneRect.Left);
			}

			return mInnerStart.Value;
		}

		public int getInnerEnd()
		{
			if (mInnerEnd != null)
				return mInnerEnd.Value;

			mInnerEnd = int.MaxValue;
			for (var i = 0; i < mLanes.Length; i++)
			{
				var laneRect = mLanes[i];
				mInnerEnd = Math.Min(mInnerEnd.Value, mIsVertical ? laneRect.Bottom : laneRect.Right);
			}

			return mInnerEnd.Value;
		}

		public class LaneInfo
		{
			public int anchorLane;
			public int startLane;

			public bool isUndefined()
			{
				return startLane == NO_LANE || anchorLane == NO_LANE;
			}

			public void set(int startLane, int anchorLane)
			{
				this.startLane = startLane;
				this.anchorLane = anchorLane;
			}

			public void setUndefined()
			{
				startLane = NO_LANE;
				anchorLane = NO_LANE;
			}
		}
	}
}