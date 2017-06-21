#region

using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using TwoWayView.Core;
using DividerItemDecoration = TwoWayView.Layout.DividerItemDecoration;
using Fragment = Android.Support.V4.App.Fragment;

#endregion

namespace TwoWayView.Sample
{
	public class LayoutFragment : Fragment
	{
		private static readonly string ARG_LAYOUT_ID = "layout_id";
		private TextView mCountText;

		private int mLayoutId;
		private TextView mPositionText;

		private Layout.TwoWayView mRecyclerView;
		private TextView mStateText;
		private Toast mToast;

		public static LayoutFragment newInstance(int layoutId)
		{
			var fragment = new LayoutFragment();

			var args = new Bundle();
			args.PutInt(ARG_LAYOUT_ID, layoutId);
			fragment.Arguments = args;

			return fragment;
		}


		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			mLayoutId = Arguments.GetInt(ARG_LAYOUT_ID);
		}


		public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState)
		{
			return inflater.Inflate(mLayoutId, container, false);
		}


		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);

			Activity activity = Activity;

			mToast = Toast.MakeText(activity, "", ToastLength.Long);
			mToast.SetGravity(GravityFlags.Center, 0, 0);

			mRecyclerView = (Layout.TwoWayView) view.FindViewById(Resource.Id.list);
			mRecyclerView.HasFixedSize = true;
			mRecyclerView.LongClickable = true;

			mPositionText = (TextView) view.RootView.FindViewById(Resource.Id.position);
			mCountText = (TextView) view.RootView.FindViewById(Resource.Id.count);

			mStateText = (TextView) view.RootView.FindViewById(Resource.Id.state);
			updateState(RecyclerView.ScrollStateIdle);

			var itemClick = ItemClickSupport.addTo(mRecyclerView);

			itemClick.setOnItemClickListener(new AnonymousIOnItemClickListener
			{
				OnItemClickedAction = (parent, position, v) =>
				{
					mToast.SetText("Item clicked: " + position);
					mToast.Show();
				}
			});

			itemClick.setOnItemLongClickListener(new AnonymousIOnItemClickListener()
			{
				OnItemLongClickedAction = (parent, position, v) =>
				{
					mToast.SetText("Item long clicked: " + position);
					mToast.Show();
					return true;
				}
			});
			/*

		mRecyclerView.SetOnScrollListener(new RecyclerView.OnScrollListener() {
			
			public void onScrollStateChanged(RecyclerView recyclerView, int scrollState)
			{
			updateState(scrollState);
		});

		
		public void onScrolled(RecyclerView recyclerView, int i, int i2)
		{
			mPositionText.SetText("First: " + mRecyclerView.getFirstVisiblePosition());
			mCountText.SetText("Count: " + mRecyclerView.ChildCount);
		}
		});
		*/

			var divider = Resources.GetDrawable(Resource.Drawable.divider);
			mRecyclerView.AddItemDecoration(new DividerItemDecoration(divider));

			mRecyclerView.SetAdapter(new LayoutAdapter(activity, mRecyclerView, mLayoutId));
		}

		private void updateState(int scrollState)
		{
			var stateName = "Undefined";
			switch (scrollState)
			{
				case RecyclerView.ScrollStateIdle:
					stateName = "Idle";
					break;

				case RecyclerView.ScrollStateDragging:
					stateName = "Dragging";
					break;

				case RecyclerView.ScrollStateSettling:
					stateName = "Flinging";
					break;
			}

			mStateText.Text = stateName;
		}

		public int getLayoutId()
		{
			return Arguments.GetInt(ARG_LAYOUT_ID);
		}
	}
}