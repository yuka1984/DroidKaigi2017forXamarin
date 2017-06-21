#region

using System;
using System.Diagnostics.Contracts;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Object = Java.Lang.Object;

#endregion

namespace TwoWayView.Core
{
	public class AnonymousOnClickListner : Java.Lang.Object, View.IOnClickListener, View.IOnLongClickListener
	{
		public Action<View> OnClickAction { get; set; }

		void View.IOnClickListener.OnClick(View v)
		{
			OnClickAction?.Invoke(v);
		}

		public Func<View, bool> OnLongClickAction { get; set; }

		bool View.IOnLongClickListener.OnLongClick(View v)
		{
			return OnLongClickAction?.Invoke(v) ?? false;
		}
	}

	public class AnonymousOnChildAttachStateChangeListener : Java.Lang.Object,
		RecyclerView.IOnChildAttachStateChangeListener
	{

		public Action<View> OnChildViewAttachedToWindowAction { get; set; }

		void RecyclerView.IOnChildAttachStateChangeListener.OnChildViewAttachedToWindow(View view)
		{
			OnChildViewAttachedToWindowAction?.Invoke(view);
		}

		public Action<View> OnChildViewDetachedFromWindowAction { get; set; }

		void RecyclerView.IOnChildAttachStateChangeListener.OnChildViewDetachedFromWindow(View view)
		{
			OnChildViewDetachedFromWindowAction?.Invoke(view);
		}
	}

	public class AnonymousIOnItemClickListener : ItemClickSupport.IOnItemClickListener, ItemClickSupport.IOnItemLongClickListener
	{
		public Action<RecyclerView, int, View> OnItemClickedAction { get; set; }
		public void onItemClicked(RecyclerView recyclerView, int position, View v)
		{
			OnItemClickedAction?.Invoke(recyclerView, position, v);
		}

		public Func<RecyclerView, int, View, bool> OnItemLongClickedAction { get; set; }
		public bool onItemLongClicked(RecyclerView recyclerView, int position, View v)
		{
			return OnItemLongClickedAction?.Invoke(recyclerView, position, v) ?? false;
		}
	}

	public class ItemClickSupport : Java.Lang.Object

	{
		private RecyclerView mRecyclerView;
		private IOnItemClickListener mOnItemClickListener;
		private IOnItemLongClickListener mOnItemLongClickListener;
		private View.IOnClickListener _mOnClickListener;
		private View.IOnLongClickListener _mOnLongClickListener;
		private RecyclerView.IOnChildAttachStateChangeListener mAttachListener;

		private ItemClickSupport(RecyclerView recyclerView)
		{
			
			_mOnClickListener = new AnonymousOnClickListner()
			{
				OnClickAction = (v) =>
				{
					if (mOnItemClickListener != null)
					{
						RecyclerView.ViewHolder holder = mRecyclerView.GetChildViewHolder(v);
						mOnItemClickListener.onItemClicked(mRecyclerView, holder.AdapterPosition, v);
					}
				}
			};
			_mOnLongClickListener = new AnonymousOnClickListner()
			{
				OnLongClickAction = v =>
				{
					if (mOnItemLongClickListener != null)
					{
						RecyclerView.ViewHolder holder = mRecyclerView.GetChildViewHolder(v);
						return mOnItemLongClickListener.onItemLongClicked(mRecyclerView, holder.AdapterPosition, v);
					}
					return false;
				}
			};
			mAttachListener = new AnonymousOnChildAttachStateChangeListener
			{
				OnChildViewAttachedToWindowAction = view =>
				{
					if (mOnItemClickListener != null)
					{
						view.SetOnClickListener(_mOnClickListener);
					}
					if (mOnItemLongClickListener != null)
					{
						view.SetOnLongClickListener(_mOnLongClickListener);
					}
				},
			};

			mRecyclerView = recyclerView;
			mRecyclerView.SetTag(Resource.Id.item_click_support, this);
			mRecyclerView.AddOnChildAttachStateChangeListener(mAttachListener);

		}

		public static ItemClickSupport addTo(RecyclerView view)
		{
			ItemClickSupport support = (ItemClickSupport) view.GetTag(Resource.Id.item_click_support);
			if (support == null)
			{
				support = new ItemClickSupport(view);
			}
			return support;
		}

		public static ItemClickSupport removeFrom(RecyclerView view)
		{
			var support = (ItemClickSupport) view.GetTag(Resource.Id.item_click_support);
			if (support != null)
			{
				support.detach(view);
			}
			return support;
		}

		public ItemClickSupport setOnItemClickListener(IOnItemClickListener listener)
		{
			mOnItemClickListener = listener;
			return this;
		}

		public ItemClickSupport setOnItemLongClickListener(IOnItemLongClickListener listener)
		{
			mOnItemLongClickListener = listener;
			return this;
		}

		private void detach(RecyclerView view)
		{
			view.RemoveOnChildAttachStateChangeListener(mAttachListener);
			view.SetTag(Resource.Id.item_click_support, null);
		}

		public interface IOnItemClickListener
		{

			void onItemClicked(RecyclerView recyclerView, int position, View v);
		}

		public interface IOnItemLongClickListener
		{

			bool onItemLongClicked(RecyclerView recyclerView, int position, View v);
		}
	}

}