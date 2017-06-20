#region

using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Math = System.Math;
using Object = Java.Lang.Object;

#endregion

namespace TwoWayView.Core
{
	public enum Direction
	{
		START,
		END
	}

	public abstract class TwoWayLayoutManager : RecyclerView.LayoutManager
	{
		private static string LOGTAG = "TwoWayLayoutManager";
		private readonly Context _context;

		private bool mIsVertical = true;
		private int mLayoutEnd;

		private int mLayoutStart;

		private SavedState mPendingSavedState;
		private int mPendingScrollOffset;

		private int mPendingScrollPosition = RecyclerView.NoPosition;


		private RecyclerView mRecyclerView;

		public TwoWayLayoutManager(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
		}

		public TwoWayLayoutManager(Context context, IAttributeSet attrs, int defStyle)
		{
			_context = context;
			var a =
				context.ObtainStyledAttributes(attrs, Resource.Styleable.twowayview_TwoWayLayoutManager, defStyle, 0);

			var indexCount = a.IndexCount;
			for (var i = 0; i < indexCount; i++)
			{
				var attr = a.GetIndex(i);

				if (attr == Resource.Styleable.twowayview_TwoWayLayoutManager_android_orientation)
				{
					var orientation = a.GetInt(attr, -1);
					if (orientation >= 0)
						setOrientation((Orientation) orientation);
				}
			}

			a.Recycle();
		}

		public TwoWayLayoutManager(Orientation orientation)
		{
			mIsVertical = orientation == Orientation.Vertical;
		}

		private int getTotalSpace()
		{
			if (mIsVertical)
				return Height - PaddingBottom - PaddingTop;
			return Width - PaddingRight - PaddingLeft;
		}

		protected int GetStartWithPadding()
		{
			return mIsVertical ? PaddingTop : PaddingLeft;
		}

		protected int GetEndWithPadding()
		{
			if (mIsVertical)
				return Height - PaddingBottom;
			return Width - PaddingRight;
		}

		protected int getChildStart(View child)
		{
			return mIsVertical ? GetDecoratedTop(child) : GetDecoratedLeft(child);
		}

		protected int getChildEnd(View child)
		{
			return mIsVertical ? GetDecoratedBottom(child) : GetDecoratedRight(child);
		}

		protected RecyclerView.Adapter getAdapter()
		{
			return mRecyclerView != null ? mRecyclerView.GetAdapter() : null;
		}

		private void offsetChildren(int offset)
		{
			if (mIsVertical)
				OffsetChildrenVertical(offset);
			else
				OffsetChildrenHorizontal(offset);

			mLayoutStart += offset;
			mLayoutEnd += offset;
		}

		private void recycleChildrenOutOfBounds(Direction direction, RecyclerView.Recycler recycler)
		{
			if (direction == Direction.END)
				recycleChildrenFromStart(direction, recycler);
			else
				recycleChildrenFromEnd(direction, recycler);
		}

		private void recycleChildrenFromStart(Direction direction, RecyclerView.Recycler recycler)
		{
			var childCount = ChildCount;
			var childrenStart = GetStartWithPadding();

			var detachedCount = 0;
			for (var i = 0; i < childCount; i++)
			{
				var child = GetChildAt(i);
				var childEnd = getChildEnd(child);

				if (childEnd >= childrenStart)
					break;

				detachedCount++;

				detachChild(child, direction);
			}

			while (--detachedCount >= 0)
			{
				var child = GetChildAt(0);
				RemoveAndRecycleView(child, recycler);
				updateLayoutEdgesFromRemovedChild(child, direction);
			}
		}

		private void recycleChildrenFromEnd(Direction direction, RecyclerView.Recycler recycler)
		{
			var childrenEnd = GetEndWithPadding();
			var childCount = ChildCount;

			var firstDetachedPos = 0;
			var detachedCount = 0;
			for (var i = childCount - 1; i >= 0; i--)
			{
				var child = GetChildAt(i);
				var childStart = getChildStart(child);

				if (childStart <= childrenEnd)
					break;

				firstDetachedPos = i;
				detachedCount++;

				detachChild(child, direction);
			}

			while (--detachedCount >= 0)
			{
				var child = GetChildAt(firstDetachedPos);
				RemoveAndRecycleViewAt(firstDetachedPos, recycler);
				updateLayoutEdgesFromRemovedChild(child, direction);
			}
		}

		private int scrollBy(int delta, RecyclerView.Recycler recycler, RecyclerView.State state)
		{
			var childCount = ChildCount;
			if (childCount == 0 || delta == 0)
				return 0;

			var start = GetStartWithPadding();
			var end = GetEndWithPadding();
			var firstPosition = getFirstVisiblePosition();

			var totalSpace = getTotalSpace();
			if (delta < 0)
				delta = Math.Max(-(totalSpace - 1), delta);
			else
				delta = Math.Min(totalSpace - 1, delta);
			var cannotScrollBackward = firstPosition == 0 &&
			                           mLayoutStart >= start && delta <= 0;
			var cannotScrollForward = firstPosition + childCount == state.ItemCount &&
			                          mLayoutEnd <= end && delta >= 0;

			if (cannotScrollForward || cannotScrollBackward)
				return 0;

			offsetChildren(-delta);

			var direction = delta > 0 ? Direction.END : Direction.START;
			recycleChildrenOutOfBounds(direction, recycler);

			var absDelta = Math.Abs(delta);
			if (canAddMoreViews(Direction.START, start - absDelta) ||
			    canAddMoreViews(Direction.END, end + absDelta))
				fillGap(direction, recycler, state);

			return delta;
		}

		private void fillGap(Direction direction, RecyclerView.Recycler recycler, RecyclerView.State state)
		{
			var childCount = ChildCount;
			var extraSpace = getExtraLayoutSpace(state);
			var firstPosition = getFirstVisiblePosition();

			if (direction == Direction.END)
			{
				fillAfter(firstPosition + childCount, recycler, state, extraSpace);
				correctTooHigh(childCount, recycler, state);
			}
			else
			{
				fillBefore(firstPosition - 1, recycler, extraSpace);
				correctTooLow(childCount, recycler, state);
			}
		}

		private void fillBefore(int pos, RecyclerView.Recycler recycler)
		{
			fillBefore(pos, recycler, 0);
		}

		private void fillBefore(int position, RecyclerView.Recycler recycler, int extraSpace)
		{
			var limit = GetStartWithPadding() - extraSpace;

			while (canAddMoreViews(Direction.START, limit) && position >= 0)
			{
				makeAndAddView(position, Direction.START, recycler);
				position--;
			}
		}

		private void fillAfter(int pos, RecyclerView.Recycler recycler, RecyclerView.State state)
		{
			fillAfter(pos, recycler, state, 0);
		}

		private void fillAfter(int position, RecyclerView.Recycler recycler, RecyclerView.State state, int extraSpace)
		{
			var limit = GetEndWithPadding() + extraSpace;

			var itemCount = state.ItemCount;
			while (canAddMoreViews(Direction.END, limit) && position < itemCount)
			{
				makeAndAddView(position, Direction.END, recycler);
				position++;
			}
		}

		private void fillSpecific(int position, RecyclerView.Recycler recycler, RecyclerView.State state)
		{
			if (state.ItemCount == 0)
				return;

			makeAndAddView(position, Direction.END, recycler);

			int extraSpaceBefore;
			int extraSpaceAfter;

			var extraSpace = getExtraLayoutSpace(state);
			if (state.TargetScrollPosition < position)
			{
				extraSpaceAfter = 0;
				extraSpaceBefore = extraSpace;
			}
			else
			{
				extraSpaceAfter = extraSpace;
				extraSpaceBefore = 0;
			}

			fillBefore(position - 1, recycler, extraSpaceBefore);

			// This will correct for the top of the first view not
			// touching the top of the parent.
			adjustViewsStartOrEnd();

			fillAfter(position + 1, recycler, state, extraSpaceAfter);
			correctTooHigh(ChildCount, recycler, state);
		}

		private void correctTooHigh(int childCount, RecyclerView.Recycler recycler, RecyclerView.State state)
		{
			// First see if the last item is visible. If it is not, it is OK for the
			// top of the list to be pushed up.
			var lastPosition = getLastVisiblePosition();
			if (lastPosition != state.ItemCount - 1 || childCount == 0)
				return;

			// This is bottom of our drawable area.
			var start = GetStartWithPadding();
			var end = GetEndWithPadding();
			var firstPosition = getFirstVisiblePosition();

			// This is how far the end edge of the last view is from the end of the
			// drawable area.
			var endOffset = end - mLayoutEnd;

			// Make sure we are 1) Too high, and 2) Either there are more rows above the
			// first row or the first row is scrolled off the top of the drawable area
			if (endOffset > 0 && (firstPosition > 0 || mLayoutStart < start))
			{
				if (firstPosition == 0)
					endOffset = Math.Min(endOffset, start - mLayoutStart);

				// Move everything down
				offsetChildren(endOffset);

				if (firstPosition > 0)
				{
					// Fill the gap that was opened above first position with more
					// children, if possible.
					fillBefore(firstPosition - 1, recycler);

					// Close up the remaining gap.
					adjustViewsStartOrEnd();
				}
			}
		}

		private void correctTooLow(int childCount, RecyclerView.Recycler recycler, RecyclerView.State state)
		{
			// First see if the first item is visible. If it is not, it is OK for the
			// end of the list to be pushed forward.
			var firstPosition = getFirstVisiblePosition();
			if (firstPosition != 0 || childCount == 0)
				return;

			var start = GetStartWithPadding();
			var end = GetEndWithPadding();
			var itemCount = state.ItemCount;
			var lastPosition = getLastVisiblePosition();

			// This is how far the start edge of the first view is from the start of the
			// drawable area.
			var startOffset = mLayoutStart - start;

			// Make sure we are 1) Too low, and 2) Either there are more columns/rows below the
			// last column/row or the last column/row is scrolled off the end of the
			// drawable area.
			if (startOffset > 0)
				if (lastPosition < itemCount - 1 || mLayoutEnd > end)
				{
					if (lastPosition == itemCount - 1)
						startOffset = Math.Min(startOffset, mLayoutEnd - end);

					// Move everything up.
					offsetChildren(-startOffset);

					if (lastPosition < itemCount - 1)
					{
						// Fill the gap that was opened below the last position with more
						// children, if possible.
						fillAfter(lastPosition + 1, recycler, state);

						// Close up the remaining gap.
						adjustViewsStartOrEnd();
					}
				}
				else if (lastPosition == itemCount - 1)
				{
					adjustViewsStartOrEnd();
				}
		}

		private void adjustViewsStartOrEnd()
		{
			if (ChildCount == 0)
				return;

			var delta = mLayoutStart - GetStartWithPadding();
			if (delta < 0)
				delta = 0;

			if (delta != 0)
				offsetChildren(-delta);
		}

		private static View findNextScrapView(IList<RecyclerView.ViewHolder> scrapList, Direction direction,
			int position)
		{
			var scrapCount = scrapList.Count;

			RecyclerView.ViewHolder closest = null;
			var closestDistance = int.MaxValue;

			for (var i = 0; i < scrapCount; i++)
			{
				var holder = scrapList[i];

				var distance = holder.Position - position;
				if (distance < 0 && direction == Direction.END ||
				    distance > 0 && direction == Direction.START)
					continue;

				var absDistance = Math.Abs(distance);
				if (absDistance < closestDistance)
				{
					closest = holder;
					closestDistance = absDistance;

					if (distance == 0)
						break;
				}
			}

			if (closest != null)
				return closest.ItemView;

			return null;
		}

		private void fillFromScrapList(IList<RecyclerView.ViewHolder> scrapList, Direction direction)
		{
			var firstPosition = getFirstVisiblePosition();

			int position;
			if (direction == Direction.END)
				position = firstPosition + ChildCount;
			else
				position = firstPosition - 1;

			View scrapChild;
			while ((scrapChild = findNextScrapView(scrapList, direction, position)) != null)
			{
				setupChild(scrapChild, direction);
				position += direction == Direction.END ? 1 : -1;
			}
		}

		private void setupChild(View child, Direction direction)
		{
			var itemSelection = ItemSelectionSupport.from(mRecyclerView);
			if (itemSelection != null)
			{
				var position = GetPosition(child);
				itemSelection.setViewChecked(child, itemSelection.isItemChecked(position));
			}

			MeasureChild(child, direction);
			layoutChild(child, direction);
		}

		private View makeAndAddView(int position, Direction direction, RecyclerView.Recycler recycler)
		{
			var child = recycler.GetViewForPosition(position);
			var isItemRemoved = ((RecyclerView.LayoutParams) child.LayoutParameters).IsItemRemoved;

			if (!isItemRemoved)
				AddView(child, direction == Direction.END ? -1 : 0);

			setupChild(child, direction);

			if (!isItemRemoved)
				updateLayoutEdgesFromNewChild(child);

			return child;
		}

		private void handleUpdate()
		{
			// Refresh state by requesting layout without changing the
			// first visible position. This will ensure the layout will
			// sync with the adapter changes.
			var firstPosition = getFirstVisiblePosition();
			var firstChild = FindViewByPosition(firstPosition);
			if (firstChild != null)
				setPendingScrollPositionWithOffset(firstPosition, getChildStart(firstChild));
			else
				setPendingScrollPositionWithOffset(RecyclerView.NoPosition, 0);
		}

		private void updateLayoutEdgesFromNewChild(View newChild)
		{
			var childStart = getChildStart(newChild);
			if (childStart < mLayoutStart)
				mLayoutStart = childStart;

			var childEnd = getChildEnd(newChild);
			if (childEnd > mLayoutEnd)
				mLayoutEnd = childEnd;
		}

		private void updateLayoutEdgesFromRemovedChild(View removedChild, Direction direction)
		{
			var childCount = ChildCount;
			if (childCount == 0)
			{
				resetLayoutEdges();
				return;
			}

			var removedChildStart = getChildStart(removedChild);
			var removedChildEnd = getChildEnd(removedChild);

			if (removedChildStart > mLayoutStart && removedChildEnd < mLayoutEnd)
				return;

			int index;
			int limit;
			if (direction == Direction.END)
			{
				// Scrolling towards the end of the layout, child view being
				// removed from the start.
				mLayoutStart = int.MaxValue;
				index = 0;
				limit = removedChildEnd;
			}
			else
			{
				// Scrolling towards the start of the layout, child view being
				// removed from the end.
				mLayoutEnd = int.MaxValue;
				index = childCount - 1;
				limit = removedChildStart;
			}

			while (index >= 0 && index <= childCount - 1)
			{
				var child = GetChildAt(index);

				if (direction == Direction.END)
				{
					var childStart = getChildStart(child);
					if (childStart < mLayoutStart)
						mLayoutStart = childStart;

					// Checked enough child views to update the minimum
					// layout start edge, stop.
					if (childStart >= limit)
						break;

					index++;
				}
				else
				{
					var childEnd = getChildEnd(child);
					if (childEnd > mLayoutEnd)
						mLayoutEnd = childEnd;

					// Checked enough child views to update the minimum
					// layout end edge, stop.
					if (childEnd <= limit)
						break;

					index--;
				}
			}
		}

		private void resetLayoutEdges()
		{
			mLayoutStart = GetStartWithPadding();
			mLayoutEnd = mLayoutStart;
		}

		protected int getExtraLayoutSpace(RecyclerView.State state)
		{
			if (state.HasTargetScrollPosition)
				return getTotalSpace();
			return 0;
		}

		private Bundle getPendingItemSelectionState()
		{
			if (mPendingSavedState != null)
				return mPendingSavedState.itemSelectionState;

			return null;
		}

		protected void setPendingScrollPositionWithOffset(int position, int offset)
		{
			mPendingScrollPosition = position;
			mPendingScrollOffset = offset;
		}

		protected int getPendingScrollPosition()
		{
			if (mPendingSavedState != null)
				return mPendingSavedState.anchorItemPosition;

			return mPendingScrollPosition;
		}

		protected int getPendingScrollOffset()
		{
			if (mPendingSavedState != null)
				return 0;

			return mPendingScrollOffset;
		}

		protected int getAnchorItemPosition(RecyclerView.State state)
		{
			var itemCount = state.ItemCount;

			var pendingPosition = getPendingScrollPosition();
			if (pendingPosition != RecyclerView.NoPosition)
				if (pendingPosition < 0 || pendingPosition >= itemCount)
					pendingPosition = RecyclerView.NoPosition;

			if (pendingPosition != RecyclerView.NoPosition)
				return pendingPosition;
			if (ChildCount > 0)
				return findFirstValidChildPosition(itemCount);
			return 0;
		}

		private int findFirstValidChildPosition(int itemCount)
		{
			var childCount = ChildCount;
			for (var i = 0; i < childCount; i++)
			{
				var view = GetChildAt(i);
				var position = GetPosition(view);
				if (position >= 0 && position < itemCount)
					return position;
			}

			return 0;
		}

		public override int GetDecoratedMeasuredWidth(View child)
		{
			var lp = (ViewGroup.MarginLayoutParams) child.LayoutParameters;
			return base.GetDecoratedMeasuredWidth(child) + lp.LeftMargin + lp.RightMargin;
		}

		public override int GetDecoratedMeasuredHeight(View child)
		{
			var lp = (ViewGroup.MarginLayoutParams) child.LayoutParameters;
			return base.GetDecoratedMeasuredHeight(child) + lp.TopMargin + lp.BottomMargin;
		}

		public override int GetDecoratedLeft(View child)
		{
			var lp = (ViewGroup.MarginLayoutParams) child.LayoutParameters;
			return base.GetDecoratedLeft(child) - lp.LeftMargin;
		}

		public override int GetDecoratedTop(View child)
		{
			var lp = (ViewGroup.MarginLayoutParams) child.LayoutParameters;
			return base.GetDecoratedTop(child) - lp.TopMargin;
		}

		public override int GetDecoratedRight(View child)
		{
			var lp = (ViewGroup.MarginLayoutParams) child.LayoutParameters;
			return base.GetDecoratedRight(child) + lp.RightMargin;
		}

		public override int GetDecoratedBottom(View child)
		{
			var lp = (ViewGroup.MarginLayoutParams) child.LayoutParameters;
			return base.GetDecoratedBottom(child) + lp.BottomMargin;
		}

		public override void LayoutDecorated(View child, int left, int top, int right, int bottom)
		{
			var lp = (ViewGroup.MarginLayoutParams) child.LayoutParameters;
			base.LayoutDecorated(child, left + lp.LeftMargin, top + lp.TopMargin,
				right - lp.RightMargin, bottom - lp.BottomMargin);
		}

		public override void OnAttachedToWindow(RecyclerView view)
		{
			base.OnAttachedToWindow(view);
			mRecyclerView = view;
		}

		public override void OnDetachedFromWindow(RecyclerView view, RecyclerView.Recycler recycler)
		{
			base.OnDetachedFromWindow(view, recycler);
			mRecyclerView = null;
		}

		public override void OnAdapterChanged(RecyclerView.Adapter oldAdapter, RecyclerView.Adapter newAdapter)
		{
			base.OnAdapterChanged(oldAdapter, newAdapter);

			var itemSelectionSupport = ItemSelectionSupport.from(mRecyclerView);
			if (oldAdapter != null && itemSelectionSupport != null)
				itemSelectionSupport.clearChoices();
		}

		public override void OnLayoutChildren(RecyclerView.Recycler recycler, RecyclerView.State state)
		{
			var itemSelection = ItemSelectionSupport.from(mRecyclerView);
			if (itemSelection != null)
			{
				var itemSelectionState = getPendingItemSelectionState();
				if (itemSelectionState != null)
					itemSelection.OnRestoreInstanceState(itemSelectionState);

				if (state.DidStructureChange())
					itemSelection.onAdapterDataChanged();
			}

			var anchorItemPosition = getAnchorItemPosition(state);
			DetachAndScrapAttachedViews(recycler);
			fillSpecific(anchorItemPosition, recycler, state);

			onLayoutScrapList(recycler, state);

			setPendingScrollPositionWithOffset(RecyclerView.NoPosition, 0);
			mPendingSavedState = null;
		}

		protected virtual void onLayoutScrapList(RecyclerView.Recycler recycler, RecyclerView.State state)
		{
			var childCount = ChildCount;
			if (childCount == 0 || state.IsPreLayout || !SupportsPredictiveItemAnimations())
				return;

			var scrapList = recycler.ScrapList;
			fillFromScrapList(scrapList, Direction.START);
			fillFromScrapList(scrapList, Direction.END);
		}

		protected virtual void detachChild(View child, Direction direction)
		{
			// Do nothing by default.
		}


		public override void OnItemsAdded(RecyclerView recyclerView, int positionStart, int itemCount)
		{
			handleUpdate();
		}

		public override void OnItemsRemoved(RecyclerView recyclerView, int positionStart, int itemCount)
		{
			handleUpdate();
		}

		public override void OnItemsUpdated(RecyclerView recyclerView, int positionStart, int itemCount)
		{
			handleUpdate();
		}

		public override void OnItemsMoved(RecyclerView recyclerView, int from, int to, int itemCount)
		{
			handleUpdate();
		}


		public override void OnItemsChanged(RecyclerView recyclerView)
		{
			handleUpdate();
		}

		public override RecyclerView.LayoutParams GenerateDefaultLayoutParams()
		{
			if (mIsVertical)
				return new RecyclerView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			return new RecyclerView.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
		}

		public override bool SupportsPredictiveItemAnimations()
		{
			return true;
		}

		public override int ScrollHorizontallyBy(int dx, RecyclerView.Recycler recycler, RecyclerView.State state)
		{
			if (mIsVertical)
				return 0;

			return scrollBy(dx, recycler, state);
		}

		public override int ScrollVerticallyBy(int dy, RecyclerView.Recycler recycler, RecyclerView.State state)
		{
			if (!mIsVertical)
				return 0;

			return scrollBy(dy, recycler, state);
		}

		public override bool CanScrollHorizontally()
		{
			return !mIsVertical;
		}

		public override bool CanScrollVertically()
		{
			return mIsVertical;
		}

		public override void ScrollToPosition(int position)
		{
			ScrollToPositionWithOffset(position, 0);
		}

		public void ScrollToPositionWithOffset(int position, int offset)
		{
			setPendingScrollPositionWithOffset(position, offset);
			RequestLayout();
		}

		public override void SmoothScrollToPosition(RecyclerView recyclerView, RecyclerView.State state, int position)
		{
			LinearSmoothScroller scroller = new MyLinearSmoothScroller(_context, getFirstVisiblePosition, () => mIsVertical);

			scroller.TargetPosition = position;

			StartSmoothScroll(scroller);
		}

		public override int ComputeHorizontalScrollOffset(RecyclerView.State state)
		{
			if (ChildCount == 0)
				return 0;

			return getFirstVisiblePosition();
		}

		public override int ComputeVerticalScrollOffset(RecyclerView.State state)
		{
			if (ChildCount == 0)
				return 0;

			return getFirstVisiblePosition();
		}

		public override int ComputeHorizontalScrollExtent(RecyclerView.State state)
		{
			return ChildCount;
		}

		public override int ComputeVerticalScrollExtent(RecyclerView.State state)
		{
			return ChildCount;
		}

		public override int ComputeHorizontalScrollRange(RecyclerView.State state)
		{
			return state.ItemCount;
		}

		public override int ComputeVerticalScrollRange(RecyclerView.State state)
		{
			return state.ItemCount;
		}


		public override IParcelable OnSaveInstanceState()
		{
			var state = new SavedState(SavedState.EMPTY_STATE);

			var anchorItemPosition = getPendingScrollPosition();
			if (anchorItemPosition == RecyclerView.NoPosition)
				anchorItemPosition = getFirstVisiblePosition();
			state.anchorItemPosition = anchorItemPosition;

			var itemSelection = ItemSelectionSupport.from(mRecyclerView);
			if (itemSelection != null)
				state.itemSelectionState = itemSelection.onSaveInstanceState();
			else
				state.itemSelectionState = Bundle.Empty;

			return state;
		}


		public override void OnRestoreInstanceState(IParcelable state)
		{
			mPendingSavedState = (SavedState) state;
			RequestLayout();
		}

		public Orientation getOrientation()
		{
			return mIsVertical ? Orientation.Vertical : Orientation.Horizontal;
		}

		public void setOrientation(Orientation orientation)
		{
			var isVertical = orientation == Orientation.Vertical;
			if (mIsVertical == isVertical)
				return;

			mIsVertical = isVertical;
			RequestLayout();
		}

		public int getFirstVisiblePosition()
		{
			if (ChildCount == 0)
				return 0;

			return GetPosition(GetChildAt(0));
		}

		public int getLastVisiblePosition()
		{
			var childCount = ChildCount;
			if (childCount == 0)
				return 0;

			return GetPosition(GetChildAt(childCount - 1));
		}

		protected abstract void MeasureChild(View child, Direction direction);
		protected abstract void layoutChild(View child, Direction direction);

		protected abstract bool canAddMoreViews(Direction direction, int limit);

		public class CheckedIdStates : LongSparseArray, IParcelable
		{
			public CheckedIdStates()
			{
			}

			public CheckedIdStates(Parcel @in)
			{
				var size = @in.ReadInt();
				if (size > 0)
					for (var i = 0; i < size; i++)
					{
						var key = @in.ReadLong();
						var value = @in.ReadInt();
						Put(key, value);
					}
			}

			public int DescribeContents()
			{
				return 0;
			}

			public void WriteToParcel(Parcel parcel, [GeneratedEnum] ParcelableWriteFlags flags)
			{
				var size = Size();
				parcel.WriteInt(size);

				for (var i = 0; i < size; i++)
				{
					parcel.WriteLong(KeyAt(i));
					parcel.WriteInt((int) ValueAt(i));
				}
			}

			public CheckedIdStates[] NewArray(int size)
			{
				return new CheckedIdStates[size];
			}
		}

		public class CheckedStates : SparseBooleanArray, IParcelable
		{
			private static readonly int FALSE = 0;
			private static readonly int TRUE = 1;

			public CheckedStates()
			{
			}

			public CheckedStates(Parcel @in)
			{
				var size = @in.ReadInt();
				if (size > 0)
					for (var i = 0; i < size; i++)
					{
						var key = @in.ReadInt();
						var value = @in.ReadInt() == TRUE;
						Put(key, value);
					}
			}

			public int DescribeContents()
			{
				return 0;
			}

			public void WriteToParcel(Parcel parcel, ParcelableWriteFlags flags)
			{
				var size = Size();
				parcel.WriteInt(size);

				for (var i = 0; i < size; i++)
				{
					parcel.WriteInt(KeyAt(i));
					parcel.WriteInt(ValueAt(i) ? TRUE : FALSE);
				}
			}

			public CheckedStates[] NewArray(int size)
			{
				return new CheckedStates[size];
			}
		}

		protected class SavedState : Object, IParcelable
		{
			public static readonly SavedState EMPTY_STATE = new SavedState();

			private readonly IParcelable _superState;
			public int anchorItemPosition;
			public Bundle itemSelectionState;


			public SavedState()
			{
			}

			public SavedState(IParcelable superState)
			{
				if (superState == null)
					throw new IllegalArgumentException("_superState must not be null");

				_superState = superState != EMPTY_STATE ? superState : null;
			}

			public SavedState(Parcel In)
			{
				_superState = EMPTY_STATE;
				anchorItemPosition = In.ReadInt();
				itemSelectionState = (Bundle) In.ReadParcelable(Class.ClassLoader);
			}


			public int DescribeContents()
			{
				return 0;
			}

			public virtual void WriteToParcel(Parcel @out, ParcelableWriteFlags flags)
			{
				@out.WriteInt(anchorItemPosition);
				@out.WriteParcelable(itemSelectionState, flags);
			}

			public IParcelable getSuperState()
			{
				return _superState;
			}
		}

		private class MyLinearSmoothScroller : LinearSmoothScroller
		{
			private readonly Func<int> _firstVisiblePositionFunc;
			private readonly Func<bool> _isVerticalFunc;

			public MyLinearSmoothScroller(Context context, Func<int> firstVisiblePositionFunc,
				Func<bool> isVerticalFunc) : base(context)
			{
				_firstVisiblePositionFunc = firstVisiblePositionFunc;
				_isVerticalFunc = isVerticalFunc;
			}

			protected override int VerticalSnapPreference => SnapToStart;

			protected override int HorizontalSnapPreference => SnapToStart;

			public override PointF ComputeScrollVectorForPosition(int targetPosition)
			{
				if (ChildCount == 0)
					return null;

				var direction = TargetPosition < _firstVisiblePositionFunc() ? -1 : 1;
				if (_isVerticalFunc())
					return new PointF(0, direction);
				return new PointF(direction, 0);
			}
		}
	}
}