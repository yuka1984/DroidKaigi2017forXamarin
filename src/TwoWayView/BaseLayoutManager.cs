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

#endregion

namespace TwoWayView.Layout
{
	public abstract class BaseLayoutManager : TwoWayLayoutManager
	{
		private static string LOGTAG = "BaseLayoutManager";

		protected Rect mChildFrame = new Rect();

		private ItemEntries mItemEntries;
		private ItemEntries mItemEntriesToRestore;

		private Lanes mLanesToRestore;
		protected Lanes.LaneInfo mTempLaneInfo = new Lanes.LaneInfo();
		protected Rect mTempRect = new Rect();

		public BaseLayoutManager(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
			;
		}

		public BaseLayoutManager(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
		{
			;
		}

		public BaseLayoutManager(Orientation orientation) : base(orientation)
		{
			;
		}

		public bool IsVertical => getOrientation() == Orientation.Vertical;

		public Lanes Lanes { get; private set; }

		protected void pushChildFrame(ItemEntry entry, Rect childFrame, int lane, int laneSpan,
			Direction direction)
		{
			var shouldSetMargins = direction == Direction.END &&
			                       entry != null && !entry.hasSpanMargins();

			for (var i = lane; i < lane + laneSpan; i++)
			{
				int spanMargin;
				if (entry != null && direction != Direction.END)
					spanMargin = entry.getSpanMargin(i - lane);
				else
					spanMargin = 0;

				var margin = Lanes.pushChildFrame(childFrame, i, spanMargin, direction);
				if (laneSpan > 1 && shouldSetMargins)
					entry.setSpanMargin(i - lane, margin, laneSpan);
			}
		}

		private void popChildFrame(ItemEntry entry, Rect childFrame, int lane, int laneSpan,
			Direction direction)
		{
			for (var i = lane; i < lane + laneSpan; i++)
			{
				int spanMargin;
				if (entry != null && direction != Direction.END)
					spanMargin = entry.getSpanMargin(i - lane);
				else
					spanMargin = 0;

				Lanes.popChildFrame(childFrame, i, spanMargin, direction);
			}
		}

		private void getDecoratedChildFrame(View child, Rect childFrame)
		{
			childFrame.Left = GetDecoratedLeft(child);
			childFrame.Top = GetDecoratedTop(child);
			childFrame.Right = GetDecoratedRight(child);
			childFrame.Bottom = GetDecoratedBottom(child);
		}

		public bool isVertical()
		{
			return getOrientation() == Orientation.Vertical;
		}

		public Lanes getLanes()
		{
			return Lanes;
		}

		protected void setItemEntryForPosition(int position, ItemEntry entry)
		{
			if (mItemEntries != null)
				mItemEntries.putItemEntry(position, entry);
		}

		protected ItemEntry getItemEntryForPosition(int position)
		{
			return mItemEntries != null ? mItemEntries.getItemEntry(position) : null;
		}

		private void clearItemEntries()
		{
			if (mItemEntries != null)
				mItemEntries.clear();
		}

		private void invalidateItemLanesAfter(int position)
		{
			if (mItemEntries != null)
				mItemEntries.invalidateItemLanesAfter(position);
		}

		private void offsetForAddition(int positionStart, int itemCount)
		{
			if (mItemEntries != null)
				mItemEntries.offsetForAddition(positionStart, itemCount);
		}

		private void offsetForRemoval(int positionStart, int itemCount)
		{
			if (mItemEntries != null)
				mItemEntries.offsetForRemoval(positionStart, itemCount);
		}

		private void requestMoveLayout()
		{
			if (getPendingScrollPosition() != RecyclerView.NoPosition)
				return;

			var position = getFirstVisiblePosition();
			var firstChild = FindViewByPosition(position);
			var offset = firstChild != null ? getChildStart(firstChild) : 0;

			setPendingScrollPositionWithOffset(position, offset);
		}

		private bool canUseLanes(Lanes lanes)
		{
			if (lanes == null)
				return false;

			var laneCount = getLaneCount();
			var laneSize = Lanes.calculateLaneSize(this, laneCount);

			return lanes.getOrientation() == getOrientation() &&
			       lanes.getCount() == laneCount &&
			       lanes.getLaneSize() == laneSize;
		}

		private bool ensureLayoutState()
		{
			var laneCount = getLaneCount();
			if (laneCount == 0 || Width == 0 || Height == 0 || canUseLanes(Lanes))
				return false;

			var oldLanes = Lanes;
			Lanes = new Lanes(this, laneCount);

			requestMoveLayout();

			if (mItemEntries == null)
				mItemEntries = new ItemEntries();

			if (oldLanes != null && oldLanes.getOrientation() == Lanes.getOrientation() &&
			    oldLanes.getLaneSize() == Lanes.getLaneSize())
				invalidateItemLanesAfter(0);
			else
				mItemEntries.clear();

			return true;
		}

		private void handleUpdate(int positionStart, int itemCountOrToPosition, UpdateOp cmd)
		{
			invalidateItemLanesAfter(positionStart);

			switch (cmd)
			{
				case UpdateOp.ADD:
					offsetForAddition(positionStart, itemCountOrToPosition);
					break;

				case UpdateOp.REMOVE:
					offsetForRemoval(positionStart, itemCountOrToPosition);
					break;

				case UpdateOp.MOVE:
					offsetForRemoval(positionStart, 1);
					offsetForAddition(itemCountOrToPosition, 1);
					break;
			}

			if (positionStart + itemCountOrToPosition <= getFirstVisiblePosition())
				return;

			if (positionStart <= getLastVisiblePosition())
				RequestLayout();
		}

		public override void OffsetChildrenHorizontal(int offset)
		{
			if (!isVertical())
				Lanes.offset(offset);

			base.OffsetChildrenHorizontal(offset);
		}

		public override void OffsetChildrenVertical(int offset)
		{
			base.OffsetChildrenVertical(offset);

			if (isVertical())
				Lanes.offset(offset);
		}

		public override void OnLayoutChildren(RecyclerView.Recycler recycler, RecyclerView.State state)
		{
			var restoringLanes = mLanesToRestore != null;
			if (restoringLanes)
			{
				Lanes = mLanesToRestore;
				mItemEntries = mItemEntriesToRestore;

				mLanesToRestore = null;
				mItemEntriesToRestore = null;
			}

			var refreshingLanes = ensureLayoutState();

			// Still not able to create lanes, nothing we can do here,
			// just bail for now.
			if (Lanes == null)
				return;

			var itemCount = state.ItemCount;

			if (mItemEntries != null)
				mItemEntries.setAdapterSize(itemCount);

			var anchorItemPosition = getAnchorItemPosition(state);

			// Only move layout if we're not restoring a layout state.
			if (anchorItemPosition > 0 && (refreshingLanes || !restoringLanes))
				moveLayoutToPosition(anchorItemPosition, getPendingScrollOffset(), recycler, state);

			Lanes.reset(Direction.START);

			base.OnLayoutChildren(recycler, state);
		}

		protected override void onLayoutScrapList(RecyclerView.Recycler recycler, RecyclerView.State state)
		{
			Lanes.save();
			base.onLayoutScrapList(recycler, state);
			Lanes.restore();
		}

		public override void OnItemsAdded(RecyclerView recyclerView, int positionStart, int itemCount)
		{
			handleUpdate(positionStart, itemCount, UpdateOp.ADD);
			base.OnItemsAdded(recyclerView, positionStart, itemCount);
		}

		public override void OnItemsRemoved(RecyclerView recyclerView, int positionStart, int itemCount)
		{
			handleUpdate(positionStart, itemCount, UpdateOp.REMOVE);
			base.OnItemsRemoved(recyclerView, positionStart, itemCount);
		}

		public override void OnItemsUpdated(RecyclerView recyclerView, int positionStart, int itemCount)
		{
			handleUpdate(positionStart, itemCount, UpdateOp.UPDATE);
			base.OnItemsUpdated(recyclerView, positionStart, itemCount);
		}

		public override void OnItemsMoved(RecyclerView recyclerView, int from, int to, int itemCount)
		{
			handleUpdate(from, to, UpdateOp.MOVE);
			base.OnItemsMoved(recyclerView, from, to, itemCount);
		}

		public override void OnItemsChanged(RecyclerView recyclerView)
		{
			clearItemEntries();
			base.OnItemsChanged(recyclerView);
		}

		public override IParcelable OnSaveInstanceState()
		{
			var superState = base.OnSaveInstanceState();
			var state = new LanedSavedState(superState);

			var laneCount = Lanes != null ? Lanes.getCount() : 0;
			state.lanes = new Rect[laneCount];
			for (var i = 0; i < laneCount; i++)
			{
				var laneRect = new Rect();
				Lanes.getLane(i, laneRect);
				state.lanes[i] = laneRect;
			}

			state.orientation = getOrientation();
			state.laneSize = Lanes != null ? Lanes.getLaneSize() : 0;
			state.itemEntries = mItemEntries;

			return state;
		}


		public override void OnRestoreInstanceState(IParcelable state)
		{
			var ss = (LanedSavedState) state;

			if (ss.lanes != null && ss.laneSize > 0)
			{
				mLanesToRestore = new Lanes(this, ss.orientation, ss.lanes, ss.laneSize);
				mItemEntriesToRestore = ss.itemEntries;
			}

			base.OnRestoreInstanceState(ss.getSuperState());
		}


		protected override bool canAddMoreViews(Direction direction, int limit)
		{
			if (direction == Direction.START)
				return Lanes.getInnerStart() > limit;
			return Lanes.getInnerEnd() < limit;
		}

		private int getWidthUsed(View child)
		{
			if (!isVertical())
				return 0;

			var size = getLanes().getLaneSize() * getLaneSpanForChild(child);
			return Width - PaddingLeft - PaddingRight - size;
		}

		private int getHeightUsed(View child)
		{
			if (isVertical())
				return 0;

			var size = getLanes().getLaneSize() * getLaneSpanForChild(child);
			return Height - PaddingTop - PaddingBottom - size;
		}

		protected virtual void measureChildWithMargins(View child)
		{
			MeasureChildWithMargins(child, getWidthUsed(child), getHeightUsed(child));
		}

		protected override void MeasureChild(View child, Direction direction)
		{
			cacheChildLaneAndSpan(child, direction);
			measureChildWithMargins(child);
		}

		protected override void layoutChild(View child, Direction direction)
		{
			getLaneForChild(mTempLaneInfo, child, direction);

			Lanes.getChildFrame(mChildFrame, GetDecoratedMeasuredWidth(child),
				GetDecoratedMeasuredHeight(child), mTempLaneInfo, direction);
			var entry = cacheChildFrame(child, mChildFrame);

			LayoutDecorated(child, mChildFrame.Left, mChildFrame.Top, mChildFrame.Right,
				mChildFrame.Bottom);

			var lp = (RecyclerView.LayoutParams) child.LayoutParameters;
			if (!lp.IsItemRemoved)
				pushChildFrame(entry, mChildFrame, mTempLaneInfo.startLane,
					getLaneSpanForChild(child), direction);
		}


		protected override void detachChild(View child, Direction direction)
		{
			var position = GetPosition(child);
			getLaneForPosition(mTempLaneInfo, position, direction);
			getDecoratedChildFrame(child, mChildFrame);

			popChildFrame(getItemEntryForPosition(position), mChildFrame, mTempLaneInfo.startLane,
				getLaneSpanForChild(child), direction);
		}

		protected virtual void getLaneForChild(Lanes.LaneInfo outInfo, View child, Direction direction)
		{
			getLaneForPosition(outInfo, GetPosition(child), direction);
		}

		public virtual int getLaneSpanForChild(View child)
		{
			return 1;
		}

		public virtual int getLaneSpanForPosition(int position)
		{
			return 1;
		}

		protected virtual ItemEntry cacheChildLaneAndSpan(View child, Direction direction)
		{
			// Do nothing by default.
			return null;
		}

		protected virtual ItemEntry cacheChildFrame(View child, Rect childFrame)
		{
			// Do nothing by default.
			return null;
		}


		public override bool CheckLayoutParams(RecyclerView.LayoutParams lp)
		{
			if (isVertical())
				return lp.Width == ViewGroup.LayoutParams.MatchParent;
			return lp.Height == ViewGroup.LayoutParams.MatchParent;
		}


		public override RecyclerView.LayoutParams GenerateDefaultLayoutParams()
		{
			if (isVertical())
				return new RecyclerView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			return new RecyclerView.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
		}

		public override RecyclerView.LayoutParams GenerateLayoutParams(ViewGroup.LayoutParams lp)
		{
			var lanedLp = new RecyclerView.LayoutParams((ViewGroup.MarginLayoutParams) lp);
			if (isVertical())
			{
				lanedLp.Width = ViewGroup.LayoutParams.MatchParent;
				lanedLp.Height = lp.Height;
			}
			else
			{
				lanedLp.Width = lp.Width;
				lanedLp.Height = ViewGroup.LayoutParams.MatchParent;
			}

			return lanedLp;
		}

		public override RecyclerView.LayoutParams GenerateLayoutParams(Context c, IAttributeSet attrs)
		{
			return new RecyclerView.LayoutParams(c, attrs);
		}

		public abstract int getLaneCount();
		public abstract void getLaneForPosition(Lanes.LaneInfo outInfo, int position, Direction direction);

		public abstract void moveLayoutToPosition(int position, int offset, RecyclerView.Recycler recycler,
			RecyclerView.State state);

		public class ItemEntry : Object, IParcelable
		{
			public int anchorLane;

			private int[] spanMargins;

			public int startLane;

			public ItemEntry(int startLane, int anchorLane)
			{
				this.startLane = startLane;
				this.anchorLane = anchorLane;
			}

			public ItemEntry(Parcel @in)
			{
				startLane = @in.ReadInt();
				anchorLane = @in.ReadInt();

				var marginCount = @in.ReadInt();
				if (marginCount > 0)
				{
					spanMargins = new int[marginCount];
					for (var i = 0; i < marginCount; i++)
						spanMargins[i] = @in.ReadInt();
				}
			}

			public int DescribeContents()
			{
				return 0;
			}

			public virtual void WriteToParcel(Parcel @out, ParcelableWriteFlags flags)
			{
				@out.WriteInt(startLane);
				@out.WriteInt(anchorLane);

				var marginCount = spanMargins != null ? spanMargins.Length : 0;
				@out.WriteInt(marginCount);

				for (var i = 0; i < marginCount; i++)
					@out.WriteInt(spanMargins[i]);
			}

			public void setLane(Lanes.LaneInfo laneInfo)
			{
				startLane = laneInfo.startLane;
				anchorLane = laneInfo.anchorLane;
			}

			public void invalidateLane()
			{
				startLane = Lanes.NO_LANE;
				anchorLane = Lanes.NO_LANE;
				spanMargins = null;
			}

			public bool hasSpanMargins()
			{
				return spanMargins != null;
			}

			public int getSpanMargin(int index)
			{
				if (spanMargins == null)
					return 0;

				return spanMargins[index];
			}

			public void setSpanMargin(int index, int margin, int span)
			{
				if (spanMargins == null)
					spanMargins = new int[span];

				spanMargins[index] = margin;
			}

/*
	public static Creator<ItemEntry> CREATOR

			= new Creator<ItemEntry>() {
			@Override

			public ItemEntry createFromParcel(Parcel @in)
	{
		return new ItemEntry(in);
	}
	
			public override ItemEntry[] newArray(int size)
			{
				return new ItemEntry[size];
			}
		}
		*/
		}

		private enum UpdateOp
		{
			ADD,
			REMOVE,
			UPDATE,
			MOVE
		}

		private class LanedSavedState : SavedState
		{
			public ItemEntries itemEntries;
			public Rect[] lanes;
			public int laneSize;

			public Orientation orientation;

			public LanedSavedState(IParcelable superState) : base(superState)
			{
				;
			}

			public LanedSavedState(Parcel @in) : base(@in)
			{
				orientation = (Orientation) @in.ReadInt();
				laneSize = @in.ReadInt();

				var laneCount = @in.ReadInt();
				if (laneCount > 0)
				{
					lanes = new Rect[laneCount];
					for (var i = 0; i < laneCount; i++)
					{
						var lane = new Rect();
						lane.ReadFromParcel(@in);
						lanes[i] = lane;
					}
				}

				var itemEntriesCount = @in.ReadInt();
				if (itemEntriesCount > 0)
				{
					itemEntries = new ItemEntries();
					for (var i = 0; i < itemEntriesCount; i++)
					{
						var entry = (ItemEntry) @in.ReadParcelable(Class.ClassLoader);
						itemEntries.restoreItemEntry(i, entry);
					}
				}
			}

			public static IParcelableCreator CREATOR => new Creator();

			public override void WriteToParcel(Parcel @out, ParcelableWriteFlags flags)
			{
				base.WriteToParcel(@out, flags);

				@out.WriteInt((int) orientation);
				@out.WriteInt(laneSize);

				var laneCount = lanes != null ? lanes.Length : 0;
				@out.WriteInt(laneCount);

				for (var i = 0; i < laneCount; i++)
					lanes[i].WriteToParcel(@out, Rect.InterfaceConsts.ParcelableWriteReturnValue);

				var itemEntriesCount = itemEntries != null ? itemEntries.size() : 0;
				@out.WriteInt(itemEntriesCount);

				for (var i = 0; i < itemEntriesCount; i++)
					@out.WriteParcelable(itemEntries.getItemEntry(i), flags);
			}

			private class Creator : Object, IParcelableCreator
			{
				public Object CreateFromParcel(Parcel source)
				{
					return new LanedSavedState(source);
				}

				public Object[] NewArray(int size)
				{
					return new LanedSavedState[size];
				}
			}
		}
	}
}