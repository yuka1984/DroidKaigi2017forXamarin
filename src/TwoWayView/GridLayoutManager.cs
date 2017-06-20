#region

using Android.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;
using Java.Lang;
using TwoWayview.Layout;
using TwoWayView.Core;
using Math = System.Math;

#endregion

namespace TwoWayView.Layout
{
	public class GridLayoutManager : BaseLayoutManager
	{
		private static string LOGTAG = "GridLayoutManager";

		private static readonly int DEFAULT_NUM_COLS = 2;
		private static readonly int DEFAULT_NUM_ROWS = 2;

		private int mNumColumns;
		private int mNumRows;

		public GridLayoutManager(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
			;
		}

		public GridLayoutManager(Context context, IAttributeSet attrs, int defStyle) : this(context, attrs, defStyle,
			DEFAULT_NUM_COLS, DEFAULT_NUM_ROWS)
		{
			;
		}

		protected GridLayoutManager(Context context, IAttributeSet attrs, int defStyle,
			int defaultNumColumns, int defaultNumRows) : base(context, attrs, defStyle)
		{
			;

			var a =
				context.ObtainStyledAttributes(attrs, Resource.Styleable.twowayview_GridLayoutManager, defStyle, 0);

			mNumColumns =
				Math.Max(1, a.GetInt(Resource.Styleable.twowayview_GridLayoutManager_twowayview_numColumns, defaultNumColumns));
			mNumRows =
				Math.Max(1, a.GetInt(Resource.Styleable.twowayview_GridLayoutManager_twowayview_numRows, defaultNumRows));

			a.Recycle();
		}

		public GridLayoutManager(Orientation orientation, int numColumns, int numRows) : base(orientation)
		{
			mNumColumns = numColumns;
			mNumRows = numRows;

			if (mNumColumns < 1)
				throw new IllegalArgumentException("GridLayoutManager must have at least 1 column");

			if (mNumRows < 1)
				throw new IllegalArgumentException("GridLayoutManager must have at least 1 row");
		}

		public override int getLaneCount()
		{
			return isVertical() ? mNumColumns : mNumRows;
		}

		public override void getLaneForPosition(Lanes.LaneInfo outInfo, int position, Direction direction)
		{
			var lane = position % getLaneCount();
			outInfo.set(lane, lane);
		}

		public override void moveLayoutToPosition(int position, int offset, RecyclerView.Recycler recycler,
			RecyclerView.State state)
		{
			var lanes = getLanes();
			lanes.reset(offset);

			getLaneForPosition(mTempLaneInfo, position, Direction.END);
			var lane = mTempLaneInfo.startLane;
			if (lane == 0)
				return;

			var child = recycler.GetViewForPosition(position);
			MeasureChild(child, Direction.END);

			var dimension =
				isVertical() ? GetDecoratedMeasuredHeight(child) : GetDecoratedMeasuredWidth(child);

			for (var i = lane - 1; i >= 0; i--)
				lanes.offset(i, dimension);
		}

		public int getNumColumns()
		{
			return mNumColumns;
		}

		public void setNumColumns(int numColumns)
		{
			if (mNumColumns == numColumns)
				return;

			mNumColumns = numColumns;
			if (isVertical())
				RequestLayout();
		}

		public int getNumRows()
		{
			return mNumRows;
		}

		public void setNumRows(int numRows)
		{
			if (mNumRows == numRows)
				return;

			mNumRows = numRows;
			if (!isVertical())
				RequestLayout();
		}
	}
}