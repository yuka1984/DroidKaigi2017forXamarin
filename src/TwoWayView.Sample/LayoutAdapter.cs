#region

using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using TwoWayView.Layout;
using StaggeredGridLayoutManager = TwoWayView.Layout.StaggeredGridLayoutManager;

#endregion

namespace TwoWayView.Sample
{
	public class LayoutAdapter : RecyclerView.Adapter
	{
		private static readonly int COUNT = 100;

		private readonly Context mContext;
		private int mCurrentItemId;
		private readonly List<int?> mItems;
		private readonly int mLayoutId;
		private readonly Layout.TwoWayView mRecyclerView;

		public LayoutAdapter(Context context, Layout.TwoWayView recyclerView, int layoutId)
		{
			mContext = context;
			mItems = new List<int?>(COUNT);
			for (var i = 0; i < COUNT; i++)
				addItem(i);

			mRecyclerView = recyclerView;
			mLayoutId = layoutId;
		}


		public override int ItemCount => mItems.Count;

		public void addItem(int position)
		{
			var id = mCurrentItemId++;
			mItems.Insert(position, id);
			NotifyItemInserted(position);
		}

		public void removeItem(int position)
		{
			mItems.RemoveAt(position);
			NotifyItemRemoved(position);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var view = LayoutInflater.From(mContext).Inflate(Resource.Layout.item, parent, false);
			return new SimpleViewHolder(view);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder h, int position)
		{
			var holder = h as SimpleViewHolder;
			holder.title.Text = mItems[position].ToString();

			var isVertical = mRecyclerView.GetOrientation() == Orientation.Vertical;
			var itemView = holder.ItemView;

			var itemId = mItems[position];

			if (mLayoutId == Resource.Layout.layout_staggered_grid)
			{
				int dimenId;
				if (itemId % 3 == 0)
					dimenId = Resource.Dimension.staggered_child_medium;
				else if (itemId % 5 == 0)
					dimenId = Resource.Dimension.staggered_child_large;
				else if (itemId % 7 == 0)
					dimenId = Resource.Dimension.staggered_child_xlarge;
				else
					dimenId = Resource.Dimension.staggered_child_small;

				int span;
				if (itemId == 2)
					span = 2;
				else
					span = 1;

				var size = mContext.Resources.GetDimensionPixelSize(dimenId);

				var lp =
					(StaggeredGridLayoutManager.LayoutParams) itemView.LayoutParameters;

				if (!isVertical)
				{
					lp.span = span;
					lp.Width = size;
					itemView.LayoutParameters = lp;
				}
				else
				{
					lp.span = span;
					lp.Height = size;
					itemView.LayoutParameters = lp;
				}
			}
			else if (mLayoutId == Resource.Layout.layout_spannable_grid)
			{
				var lp =
					(SpannableGridLayoutManager.LayoutParams) itemView.LayoutParameters;

				var span1 = itemId == 0 || itemId == 3 ? 2 : 1;
				var span2 = itemId == 0 ? 2 : (itemId == 3 ? 3 : 1);

				var colSpan = isVertical ? span2 : span1;
				var rowSpan = isVertical ? span1 : span2;

				if (lp.RowSpan != rowSpan || lp.ColSpan != colSpan)
				{
					lp.RowSpan = rowSpan;
					lp.ColSpan = colSpan;
					itemView.LayoutParameters = lp;
				}
			}
		}

		public class SimpleViewHolder : RecyclerView.ViewHolder
		{
			public TextView title;

			public SimpleViewHolder(View view) : base(view)
			{
				;
				title = (TextView) view.FindViewById(Resource.Id.title);
			}
		}
	}
}