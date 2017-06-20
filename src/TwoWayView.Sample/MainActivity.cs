#region

using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Java.Lang;
using ActionBar = Android.Support.V7.App.ActionBar;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

#endregion

namespace TwoWayView.Sample
{
	[Activity(Label = "TwoWayView.Sample", MainLauncher = true, Theme = "@style/AppTheme")]
	public class MainActivity : AppCompatActivity
	{
		private readonly string ARG_SELECTED_LAYOUT_ID = "selectedLayoutId";

		private readonly int DEFAULT_LAYOUT = Resource.Layout.layout_list;

		private int mSelectedLayoutId;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			var actionBar = SupportActionBar;
			actionBar.NavigationMode = (int) ActionBarNavigationMode.Tabs;
			actionBar.SetDisplayShowTitleEnabled(false);
			actionBar.SetDisplayShowHomeEnabled(false);

			mSelectedLayoutId = DEFAULT_LAYOUT;
			if (savedInstanceState != null)
				mSelectedLayoutId = savedInstanceState.GetInt(ARG_SELECTED_LAYOUT_ID);

			AddLayoutTab(
				actionBar, Resource.Layout.layout_list, Resource.Drawable.ic_list, "list");
			AddLayoutTab(
				actionBar, Resource.Layout.layout_grid, Resource.Drawable.ic_grid, "grid");
			AddLayoutTab(
				actionBar, Resource.Layout.layout_staggered_grid, Resource.Drawable.ic_staggered, "staggered");
			AddLayoutTab(
				actionBar, Resource.Layout.layout_spannable_grid, Resource.Drawable.ic_spannable, "spannable");
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			outState.PutInt(ARG_SELECTED_LAYOUT_ID, mSelectedLayoutId);
		}

		private void AddLayoutTab(ActionBar actionBar, int layoutId, int iconId, string tag)
		{
			var tab = actionBar.NewTab()
				.SetText("")
				.SetIcon(iconId)
				.SetTabListener(new MyTabListner(layoutId, tag, this));
			actionBar.AddTab(tab, layoutId == mSelectedLayoutId);
		}

		public class MyTabListner : Object, ActionBar.ITabListener
		{
			private readonly int _layoutId;
			private readonly MainActivity _owner;
			private readonly string _tag;
			private Fragment _fragment;

			public MyTabListner(int layoutId, string tag, MainActivity owner)
			{
				_layoutId = layoutId;
				_tag = tag;
				_owner = owner;
			}

			public void OnTabReselected(ActionBar.Tab tab, FragmentTransaction ft)
			{
			}

			public void OnTabSelected(ActionBar.Tab tab, FragmentTransaction ft)
			{
				_fragment = _owner.SupportFragmentManager.FindFragmentByTag(_tag);
				if (_fragment == null)
				{
					_fragment = LayoutFragment.newInstance(_layoutId);
					ft.Add(Resource.Id.content, _fragment, _tag);
				}
				else
				{
					ft.Attach(_fragment);
				}

				_owner.mSelectedLayoutId = _fragment.Id;
			}

			public void OnTabUnselected(ActionBar.Tab tab, FragmentTransaction ft)
			{
				if (_fragment != null)
					ft.Detach(_fragment);
			}
		}
	}
}