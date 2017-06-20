#region

using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Math = System.Math;

#endregion

namespace TwoWayView.Core
{
	public class ItemSelectionSupport : Object
	{
		public static int INVALID_POSITION = -1;

		private static readonly string STATE_KEY_CHOICE_MODE = "choiceMode";
		private static readonly string STATE_KEY_CHECKED_STATES = "CheckedStates";
		private static readonly string STATE_KEY_CHECKED_ID_STATES = "CheckedIdStates";
		private static readonly string STATE_KEY_CHECKED_COUNT = "CheckedCount";

		private static readonly int CHECK_POSITION_SEARCH_DISTANCE = 20;


		private readonly RecyclerView mRecyclerView;
		private readonly TouchListener mTouchListener;
		private int mCheckedCount;
		private TwoWayLayoutManager.CheckedIdStates mCheckedIdStates;
		private TwoWayLayoutManager.CheckedStates mCheckedStates;

		private ChoiceMode mChoiceMode = ChoiceMode.None;

		private ItemSelectionSupport(RecyclerView recyclerView)
		{
			mRecyclerView = recyclerView;

			mTouchListener = new TouchListener(recyclerView, this);
			recyclerView.AddOnItemTouchListener(mTouchListener);
		}

		private void updateOnScreenCheckedViews()
		{
			var count = mRecyclerView.ChildCount;
			for (var i = 0; i < count; i++)
			{
				var child = mRecyclerView.GetChildAt(i);
				var position = mRecyclerView.GetChildPosition(child);
				setViewChecked(child, mCheckedStates.Get(position));
			}
		}

		/**
		 * Returns the number of items currently selected. This will only be valid
		 * if the choice mode is not {@link ChoiceMode#NONE} (default).
		 *
		 * <p>To determine the specific items that are currently selected, use one of
		 * the <code>getChecked*</code> methods.
		 *
		 * @return The number of items currently selected
		 *
		 * @see #getCheckedItemPosition()
		 * @see #getCheckedItemPositions()
		 * @see #getCheckedItemIds()
		 */
		public int getCheckedItemCount()
		{
			return mCheckedCount;
		}

		/**
		 * Returns the Checked state of the specified position. The result is only
		 * valid if the choice mode has been set to {@link ChoiceMode#SINGLE}
		 * or {@link ChoiceMode#MULTIPLE}.
		 *
		 * @param position The item whose Checked state to return
		 * @return The item's Checked state or <code>false</code> if choice mode
		 *         is invalid
		 *
		 * @see #setChoiceMode(ChoiceMode)
		 */
		public bool isItemChecked(int position)
		{
			if (mChoiceMode != ChoiceMode.None && mCheckedStates != null)
				return mCheckedStates.Get(position);

			return false;
		}

		/**
		 * Returns the currently Checked item. The result is only valid if the choice
		 * mode has been set to {@link ChoiceMode#SINGLE}.
		 *
		 * @return The position of the currently Checked item or
		 *         {@link #INVALID_POSITION} if nothing is selected
		 *
		 * @see #setChoiceMode(ChoiceMode)
		 */
		public int getCheckedItemPosition()
		{
			if (mChoiceMode == ChoiceMode.Single && mCheckedStates != null && mCheckedStates.Size() == 1)
				return mCheckedStates.KeyAt(0);

			return INVALID_POSITION;
		}

		/**
		 * Returns the set of Checked items in the list. The result is only valid if
		 * the choice mode has not been set to {@link ChoiceMode#NONE}.
		 *
		 * @return  A SparseBooleanArray which will return true for each call to
		 *          get(int position) where position is a position in the list,
		 *          or <code>null</code> if the choice mode is set to
		 *          {@link ChoiceMode#NONE}.
		 */
		public SparseBooleanArray getCheckedItemPositions()
		{
			if (mChoiceMode != ChoiceMode.None)
				return mCheckedStates;

			return null;
		}

		/**
		 * Returns the set of Checked items ids. The result is only valid if the
		 * choice mode has not been set to {@link ChoiceMode#NONE} and the adapter
		 * has stable IDs.
		 *
		 * @return A new array which contains the id of each Checked item in the
		 *         list.
		 *
		 * @see android.support.v7.widget.RecyclerView.Adapter#HasStableIds
		 */
		public long[] getCheckedItemIds()
		{
			if (mChoiceMode == ChoiceMode.None
			    || mCheckedIdStates == null || mRecyclerView.GetAdapter() == null)
				return new long[0];

			var count = mCheckedIdStates.Size();
			var ids = new long[count];

			for (var i = 0; i < count; i++)
				ids[i] = mCheckedIdStates.KeyAt(i);

			return ids;
		}

		/**
		 * Sets the Checked state of the specified position. The is only valid if
		 * the choice mode has been set to {@link ChoiceMode#SINGLE} or
		 * {@link ChoiceMode#MULTIPLE}.
		 *
		 * @param position The item whose Checked state is to be Checked
		 * @param Checked The new Checked state for the item
		 */
		public void setItemChecked(int position, bool Checked)
		{
			if (mChoiceMode == ChoiceMode.None)
				return;

			var adapter = mRecyclerView.GetAdapter();

			if (mChoiceMode == ChoiceMode.Multiple)
			{
				var oldValue = mCheckedStates.Get(position);
				mCheckedStates.Put(position, Checked);

				if (mCheckedIdStates != null && adapter.HasStableIds)
					if (Checked)
						mCheckedIdStates.Put(adapter.GetItemId(position), position);
					else
						mCheckedIdStates.Delete(adapter.GetItemId(position));

				if (oldValue != Checked)
					if (Checked)
						mCheckedCount++;
					else
						mCheckedCount--;
			}
			else
			{
				var updateIds = mCheckedIdStates != null && adapter.HasStableIds;

				// Clear all values if we're checking something, or unchecking the currently
				// selected item
				if (Checked || isItemChecked(position))
				{
					mCheckedStates.Clear();

					if (updateIds)
						mCheckedIdStates.Clear();
				}

				// This may end up selecting the Checked we just cleared but this way
				// we ensure length of mCheckStates is 1, a fact getCheckedItemPosition relies on
				if (Checked)
				{
					mCheckedStates.Put(position, true);

					if (updateIds)
						mCheckedIdStates.Put(adapter.GetItemId(position), position);

					mCheckedCount = 1;
				}
				else if (mCheckedStates.Size() == 0 || !mCheckedStates.ValueAt(0))
				{
					mCheckedCount = 0;
				}
			}

			updateOnScreenCheckedViews();
		}


		public void setViewChecked(View view, bool Checked)
		{
			if (view is ICheckable)
				((ICheckable) view).Checked = Checked;
			else if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
				view.Activated = Checked;
		}

		/**
		 * Clears any choices previously set.
		 */
		public void clearChoices()
		{
			if (mCheckedStates != null)
				mCheckedStates.Clear();

			if (mCheckedIdStates != null)
				mCheckedIdStates.Clear();

			mCheckedCount = 0;
			updateOnScreenCheckedViews();
		}

		/**
		 * Returns the current choice mode.
		 *
		 * @see #setChoiceMode(ChoiceMode)
		 */
		public ChoiceMode getChoiceMode()
		{
			return mChoiceMode;
		}

		/**
		 * Defines the choice behavior for the List. By default, Lists do not have any choice behavior
		 * ({@link ChoiceMode#NONE}). By setting the choiceMode to {@link ChoiceMode#SINGLE}, the
		 * List allows up to one item to  be in a chosen state. By setting the choiceMode to
		 * {@link ChoiceMode#MULTIPLE}, the list allows any number of items to be chosen.
		 *
		 * @param choiceMode One of {@link ChoiceMode#NONE}, {@link ChoiceMode#SINGLE}, or
		 * {@link ChoiceMode#MULTIPLE}
		 */
		public void setChoiceMode(ChoiceMode choiceMode)
		{
			if (mChoiceMode == choiceMode)
				return;

			mChoiceMode = choiceMode;

			if (mChoiceMode != ChoiceMode.None)
			{
				if (mCheckedStates == null)
					mCheckedStates = new TwoWayLayoutManager.CheckedStates();

				var adapter = mRecyclerView.GetAdapter();
				if (mCheckedIdStates == null && adapter != null && adapter.HasStableIds)
					mCheckedIdStates = new TwoWayLayoutManager.CheckedIdStates();
			}
		}

		public void onAdapterDataChanged()
		{
			var adapter = mRecyclerView.GetAdapter();
			if (mChoiceMode == ChoiceMode.None || adapter == null || !adapter.HasStableIds)
				return;

			var itemCount = adapter.ItemCount;

			// Clear out the positional check states, we'll rebuild it below from IDs.
			mCheckedStates.Clear();

			for (var CheckedIndex = 0; CheckedIndex < mCheckedIdStates.Size(); CheckedIndex++)
			{
				var currentId = mCheckedIdStates.KeyAt(CheckedIndex);
				var currentPosition = (int) mCheckedIdStates.ValueAt(CheckedIndex);

				var newPositionId = adapter.GetItemId(currentPosition);
				if (currentId != newPositionId)
				{
					// Look around to see if the ID is nearby. If not, uncheck it.
					var start = Math.Max(0, currentPosition - CHECK_POSITION_SEARCH_DISTANCE);
					var end = Math.Min(currentPosition + CHECK_POSITION_SEARCH_DISTANCE, itemCount);

					var found = false;
					for (var searchPos = start; searchPos < end; searchPos++)
					{
						var searchId = adapter.GetItemId(searchPos);
						if (currentId == searchId)
						{
							found = true;
							mCheckedStates.Put(searchPos, true);
							mCheckedIdStates.SetValueAt(CheckedIndex, searchPos);
							break;
						}
					}

					if (!found)
					{
						mCheckedIdStates.Delete(currentId);
						mCheckedCount--;
						CheckedIndex--;
					}
				}
				else
				{
					mCheckedStates.Put(currentPosition, true);
				}
			}
		}

		public Bundle onSaveInstanceState()
		{
			var state = new Bundle();

			state.PutInt(STATE_KEY_CHOICE_MODE, (int) mChoiceMode);
			state.PutParcelable(STATE_KEY_CHECKED_STATES, mCheckedStates);
			state.PutParcelable(STATE_KEY_CHECKED_ID_STATES, mCheckedIdStates);
			state.PutInt(STATE_KEY_CHECKED_COUNT, mCheckedCount);

			return state;
		}

		public void OnRestoreInstanceState(Bundle state)
		{
			mChoiceMode = (ChoiceMode) state.GetInt(STATE_KEY_CHOICE_MODE);
			mCheckedStates = (TwoWayLayoutManager.CheckedStates) state.GetParcelable(STATE_KEY_CHECKED_STATES);
			mCheckedIdStates = (TwoWayLayoutManager.CheckedIdStates) state.GetParcelable(STATE_KEY_CHECKED_ID_STATES);
			mCheckedCount = state.GetInt(STATE_KEY_CHECKED_COUNT);

			// TODO confirm ids here
		}

		public static ItemSelectionSupport addTo(RecyclerView recyclerView)
		{
			var itemSelectionSupport = from(recyclerView);
			if (itemSelectionSupport == null)
			{
				itemSelectionSupport = new ItemSelectionSupport(recyclerView);
				recyclerView.SetTag(Resource.Id.twowayview_item_selection_support, itemSelectionSupport);
			}

			return itemSelectionSupport;
		}

		public static void removeFrom(RecyclerView recyclerView)
		{
			var itemSelection = from(recyclerView);
			if (itemSelection == null)
				return;

			itemSelection.clearChoices();

			recyclerView.RemoveOnItemTouchListener(itemSelection.mTouchListener);
			recyclerView.SetTag(Resource.Id.twowayview_item_selection_support, null);
		}

		public static ItemSelectionSupport from(RecyclerView recyclerView)
		{
			if (recyclerView == null)
				return null;

			return (ItemSelectionSupport) recyclerView.GetTag(Resource.Id.twowayview_item_selection_support);
		}

		private class TouchListener : ClickItemTouchListener
		{
			private readonly ItemSelectionSupport _owner;

			public TouchListener(RecyclerView recyclerView, ItemSelectionSupport owner) : base(recyclerView)
			{
				_owner = owner;
			}

			public override bool PerformItemClick(RecyclerView parent, View view, int position, long id)
			{
				var adapter = _owner.mRecyclerView.GetAdapter();
				var checkedStateChanged = false;

				if (_owner.mChoiceMode == ChoiceMode.Multiple)
				{
					var check = !_owner.mCheckedStates.Get(position, false);

					_owner.mCheckedStates.Put(position, check);

					if (_owner.mCheckedIdStates != null && adapter.HasStableIds)
						if (check)
							_owner.mCheckedIdStates.Put(adapter.GetItemId(position), position);
						else
							_owner.mCheckedIdStates.Delete(adapter.GetItemId(position));

					if (check)
						_owner.mCheckedCount++;
					else
						_owner.mCheckedCount--;

					checkedStateChanged = true;
				}
				else if (_owner.mChoiceMode == ChoiceMode.Single)
				{
					var check = !_owner.mCheckedStates.Get(position, false);
					if (check)
					{
						_owner.mCheckedStates.Clear();
						_owner.mCheckedStates.Put(position, true);

						if (_owner.mCheckedIdStates != null && adapter.HasStableIds)
						{
							_owner.mCheckedIdStates.Clear();
							_owner.mCheckedIdStates.Put(adapter.GetItemId(position), position);
						}

						_owner.mCheckedCount = 1;
					}
					else if (_owner.mCheckedStates.Size() == 0 || !_owner.mCheckedStates.ValueAt(0))
					{
						_owner.mCheckedCount = 0;
					}

					checkedStateChanged = true;
				}

				if (checkedStateChanged)
					_owner.updateOnScreenCheckedViews();

				return false;
			}


			public override bool PerformItemLongClick(RecyclerView parent, View view, int position, long id)
			{
				return true;
			}
		}
	}
}