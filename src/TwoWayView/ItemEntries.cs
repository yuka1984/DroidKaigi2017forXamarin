#region

using System;
using Java.Util;

#endregion

namespace TwoWayView.Layout
{
	internal class ItemEntries
	{
		private static readonly int MIN_SIZE = 10;
		private int mAdapterSize;

		private BaseLayoutManager.ItemEntry[] mItemEntries;
		private bool mRestoringItem;

		private int sizeForPosition(int position)
		{
			var len = mItemEntries.Length;
			while (len <= position)
				len *= 2;

			// We don't apply any constraints while restoring
			// item entries.
			if (!mRestoringItem && len > mAdapterSize)
				len = mAdapterSize;

			return len;
		}

		private void ensureSize(int position)
		{
			if (mItemEntries == null)
			{
				mItemEntries = new BaseLayoutManager.ItemEntry[Math.Max(position, MIN_SIZE) + 1];
				Arrays.Fill(mItemEntries, null);
			}
			else if (position >= mItemEntries.Length)
			{
				var oldItemEntries = mItemEntries;
				mItemEntries = new BaseLayoutManager.ItemEntry[sizeForPosition(position)];
				//JavaSystem.Arraycopy(oldItemEntries, 0, mItemEntries, 0, oldItemEntries.Length);
				Array.Copy(oldItemEntries, mItemEntries, oldItemEntries.Length);
				Arrays.Fill(mItemEntries, oldItemEntries.Length, mItemEntries.Length, null);
			}
		}

		public BaseLayoutManager.ItemEntry getItemEntry(int position)
		{
			if (mItemEntries == null || position >= mItemEntries.Length)
				return null;

			return mItemEntries[position];
		}

		public void putItemEntry(int position, BaseLayoutManager.ItemEntry entry)
		{
			ensureSize(position);
			mItemEntries[position] = entry;
		}

		public void restoreItemEntry(int position, BaseLayoutManager.ItemEntry entry)
		{
			mRestoringItem = true;
			putItemEntry(position, entry);
			mRestoringItem = false;
		}

		public int size()
		{
			return mItemEntries != null ? mItemEntries.Length : 0;
		}

		public void setAdapterSize(int adapterSize)
		{
			mAdapterSize = adapterSize;
		}

		public void invalidateItemLanesAfter(int position)
		{
			if (mItemEntries == null || position >= mItemEntries.Length)
				return;

			for (var i = position; i < mItemEntries.Length; i++)
			{
				var entry = mItemEntries[i];
				if (entry != null)
					entry.invalidateLane();
			}
		}

		public void clear()
		{
			if (mItemEntries != null)
				Arrays.Fill(mItemEntries, null);
		}

		public void offsetForRemoval(int positionStart, int itemCount)
		{
			if (mItemEntries == null || positionStart >= mItemEntries.Length)
				return;

			ensureSize(positionStart + itemCount);
			Array.Copy(mItemEntries, positionStart + itemCount, mItemEntries, positionStart,
				mItemEntries.Length - positionStart - itemCount);
			// TODO:Arrays.Fill
			Arrays.Fill(mItemEntries, mItemEntries.Length - itemCount, mItemEntries.Length, null);
		}

		public void offsetForAddition(int positionStart, int itemCount)
		{
			if (mItemEntries == null || positionStart >= mItemEntries.Length)
				return;

			ensureSize(positionStart + itemCount);

			Array.Copy(mItemEntries, positionStart, mItemEntries, positionStart + itemCount,
				mItemEntries.Length - positionStart - itemCount);
			// TODO:Arrays.Fill
			Arrays.Fill(mItemEntries, positionStart, positionStart + itemCount, null);
		}
	}
}