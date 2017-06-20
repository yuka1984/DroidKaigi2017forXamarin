#region

using Android.Graphics;
using Android.Support.V7.Widget;
using Java.Lang;
using TwoWayview.Layout;
using TwoWayView.Core;

#endregion

namespace TwoWayView.Layout
{
	internal class ItemSpacingOffsets
	{
		private readonly int mHorizontalSpacing;

		private readonly Lanes.LaneInfo mTempLaneInfo = new Lanes.LaneInfo();
		private readonly int mVerticalSpacing;
		private bool mAddSpacingAtEnd;

		public ItemSpacingOffsets(int verticalSpacing, int horizontalSpacing)
		{
			if (verticalSpacing < 0 || horizontalSpacing < 0)
				throw new IllegalArgumentException("Spacings should be equal or greater than 0");

			mVerticalSpacing = verticalSpacing;
			mHorizontalSpacing = horizontalSpacing;
		}

		/**
		 * Checks whether the given position is placed just after the item in the
		 * first lane of the layout taking items spans into account.
		 */
		private bool isSecondLane(BaseLayoutManager lm, int itemPosition, int lane)
		{
			if (lane == 0 || itemPosition == 0)
				return false;

			var previousLane = Lanes.NO_LANE;
			var previousPosition = itemPosition - 1;
			while (previousPosition >= 0)
			{
				lm.getLaneForPosition(mTempLaneInfo, previousPosition, Direction.END);
				previousLane = mTempLaneInfo.startLane;
				if (previousLane != lane)
					break;

				previousPosition--;
			}

			var previousLaneSpan = lm.getLaneSpanForPosition(previousPosition);
			if (previousLane == 0)
				return lane == previousLane + previousLaneSpan;

			return false;
		}

		/**
		 * Checks whether the given position is placed at the start of a layout lane.
		 */
		private static bool isFirstChildInLane(BaseLayoutManager lm, int itemPosition)
		{
			var laneCount = lm.Lanes.Count;
			if (itemPosition >= laneCount)
				return false;

			var count = 0;
			for (var i = 0; i < itemPosition; i++)
			{
				count += lm.getLaneSpanForPosition(i);
				if (count >= laneCount)
					return false;
			}

			return true;
		}

		/**
		 * Checks whether the given position is placed at the end of a layout lane.
		 */
		private static bool isLastChildInLane(BaseLayoutManager lm, int itemPosition, int itemCount)
		{
			var laneCount = lm.getLanes().getCount();
			if (itemPosition < itemCount - laneCount)
				return false;

			// TODO: Figure out a robust way to compute this for layouts
			// that are dynamically placed and might span multiple lanes.
			if (lm is SpannableGridLayoutManager ||
			    lm is StaggeredGridLayoutManager) return false;

			return true;
		}

		public void setAddSpacingAtEnd(bool spacingAtEnd)
		{
			mAddSpacingAtEnd = spacingAtEnd;
		}

		/**
		 * Computes the offsets based on the vertical and horizontal spacing values.
		 * The spacing computation has to ensure that the lane sizes are the same after
		 * applying the offsets. This means we have to shift the spacing unevenly across
		 * items depending on their position in the layout.
		 */
		public void getItemOffsets(Rect outRect, int itemPosition, RecyclerView parent)
		{
			var lm = (BaseLayoutManager) parent.GetLayoutManager();

			lm.getLaneForPosition(mTempLaneInfo, itemPosition, Direction.END);
			var lane = mTempLaneInfo.startLane;
			var laneSpan = lm.getLaneSpanForPosition(itemPosition);
			var laneCount = lm.getLanes().getCount();
			var itemCount = parent.GetAdapter().ItemCount;

			var isVertical = lm.isVertical();

			var firstLane = lane == 0;
			var secondLane = isSecondLane(lm, itemPosition, lane);

			var lastLane = lane + laneSpan == laneCount;
			var beforeLastLane = lane + laneSpan == laneCount - 1;

			var laneSpacing = isVertical ? mHorizontalSpacing : mVerticalSpacing;

			int laneOffsetStart;
			int laneOffsetEnd;

			if (firstLane)
				laneOffsetStart = 0;
			else if (lastLane && !secondLane)
				laneOffsetStart = (int) (laneSpacing * 0.75);
			else if (secondLane && !lastLane)
				laneOffsetStart = (int) (laneSpacing * 0.25);
			else
				laneOffsetStart = (int) (laneSpacing * 0.5);

			if (lastLane)
				laneOffsetEnd = 0;
			else if (firstLane && !beforeLastLane)
				laneOffsetEnd = (int) (laneSpacing * 0.75);
			else if (beforeLastLane && !firstLane)
				laneOffsetEnd = (int) (laneSpacing * 0.25);
			else
				laneOffsetEnd = (int) (laneSpacing * 0.5);

			var isFirstInLane = isFirstChildInLane(lm, itemPosition);
			var isLastInLane = !mAddSpacingAtEnd &&
			                   isLastChildInLane(lm, itemPosition, itemCount);

			if (isVertical)
			{
				outRect.Left = laneOffsetStart;
				outRect.Top = isFirstInLane ? 0 : mVerticalSpacing / 2;
				outRect.Right = laneOffsetEnd;
				outRect.Bottom = isLastInLane ? 0 : mVerticalSpacing / 2;
			}
			else
			{
				outRect.Left = isFirstInLane ? 0 : mHorizontalSpacing / 2;
				outRect.Top = laneOffsetStart;
				outRect.Right = isLastInLane ? 0 : mHorizontalSpacing / 2;
				outRect.Bottom = laneOffsetEnd;
			}
		}
	}
}