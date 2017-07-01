using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Reactive.Bindings;

namespace Nyanto.Binding
{
	public class ReadOnlyObsrvableCollectionAdapter<TViewModel> : RecyclerView.Adapter
	{
		private readonly ReadOnlyObservableCollection<TViewModel> _collection;
		private readonly Func<ViewGroup, int, RecyclerView.ViewHolder> _createViewHolderFunc;
		private readonly Action<RecyclerView.ViewHolder, TViewModel> _bindAction;
		private IDisposable collectionChangeDisposable = null;

		public ReadOnlyObsrvableCollectionAdapter(ReadOnlyObservableCollection<TViewModel> collection
			, Func<ViewGroup, int, RecyclerView.ViewHolder> createViewHolderFunc
			, Action<RecyclerView.ViewHolder, TViewModel> bind)
		{
			_collection = collection;
			_createViewHolderFunc = createViewHolderFunc;
			_bindAction = bind;
		}

		public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
		{
			base.OnAttachedToRecyclerView(recyclerView);
			collectionChangeDisposable = _collection
				.ToCollectionChanged()
				.Subscribe(x =>
				{
					switch (x.Action)
					{
						case NotifyCollectionChangedAction.Add:
							NotifyItemInserted(x.Index);
							break;
						case NotifyCollectionChangedAction.Move:
							NotifyItemMoved(x.OldIndex, x.Index);
							break;
						case NotifyCollectionChangedAction.Remove:
							NotifyItemRemoved(x.Index);
							break;
						case NotifyCollectionChangedAction.Replace:
							NotifyItemChanged(x.Index);
							break;
						case NotifyCollectionChangedAction.Reset:
							NotifyDataSetChanged();
							break;
					}
				});
		}

		public override void OnDetachedFromRecyclerView(RecyclerView recyclerView)
		{
			base.OnDetachedFromRecyclerView(recyclerView);
			collectionChangeDisposable?.Dispose();
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			_bindAction(holder, _collection[position]);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			return _createViewHolderFunc(parent, viewType);
		}

		public override int ItemCount => _collection.Count;
	}

	public static class ObservableCollectionHelper
	{
		public static ReadOnlyObsrvableCollectionAdapter<TViewModel> ToAdapter<TViewModel>(
			this ReadOnlyObservableCollection<TViewModel> collection
			, Func<ViewGroup, int, RecyclerView.ViewHolder> createViewHolderFunc
			, Action<RecyclerView.ViewHolder, TViewModel> bindAction)
		{
			return new ReadOnlyObsrvableCollectionAdapter<TViewModel>(collection, createViewHolderFunc, bindAction);
		}
	}
}