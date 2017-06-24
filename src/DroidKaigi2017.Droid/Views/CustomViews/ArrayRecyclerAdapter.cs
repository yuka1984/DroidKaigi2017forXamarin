#region

using System.Collections.Generic;
using System.Reactive.Disposables;
using Android.Content;
using Android.Support.V7.Widget;

#endregion

namespace DroidKaigi2017.Droid.Views.CustomViews
{
	public abstract class ArrayRecyclerAdapter<T> : RecyclerView.Adapter
	{
		protected CompositeDisposable CompositeDisposable = new CompositeDisposable();
		private readonly List<T> _list;
		protected RecyclerView _recyclerView = null;

		public ArrayRecyclerAdapter(Context context) : this(context, new List<T>())
		{
		}

		public ArrayRecyclerAdapter(Context context, List<T> list)
		{
			Context = context;
			_list = list;
		}

		public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
		{
			CompositeDisposable = new CompositeDisposable();
			base.OnAttachedToRecyclerView(recyclerView);
			_recyclerView = recyclerView;
		}

		public override void OnDetachedFromRecyclerView(RecyclerView recyclerView)
		{
			base.OnDetachedFromRecyclerView(recyclerView);
			CompositeDisposable.Dispose();
		}

		public override int ItemCount => _list.Count;

		public Context Context { get; }

		public void Reset(IList<T> items)
		{
			Clear();
			AddRange(items);
			NotifyDataSetChanged();
		}

		public void Clear()
		{
			_list.Clear();
		}

		public void AddRange(IList<T> items)
		{
			_list.AddRange(items);
		}

		public T Get(int position)
		{
			return _list[position];
		}

		public void Add(T item)
		{
			_list.Add(item);
		}

		public void AddRangeWithNotify(IList<T> items)
		{
			var position = ItemCount;
			AddRange(items);
			NotifyItemInserted(position);
		}
	}
}