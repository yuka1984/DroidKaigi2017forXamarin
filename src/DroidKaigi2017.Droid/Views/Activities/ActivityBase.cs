#region

using Android.Content;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Calligraphy;
using Nyanto;

#endregion

namespace DroidKaigi2017.Droid.Views.Activities
{
	public abstract class ActivityBase : AppCompatActivityBase
	{
		static ActivityBase()
		{
			AppCompatDelegate.CompatVectorFromResourcesEnabled = true;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Android.Resource.Id.Home:
					OnBackPressed();
					break;
			}

			return base.OnOptionsItemSelected(item);
		}

		protected void ReplaceFragment(Fragment fragment, int layoutResId)
		{
			var ft = SupportFragmentManager.BeginTransaction();
			ft.Replace(layoutResId, fragment);
			ft.Commit();
		}

		protected void InitBackToolbar(Toolbar toolbar)
		{
			SetSupportActionBar(toolbar);
			if (SupportActionBar != null)
			{
				SupportActionBar.Title = toolbar.Title;
				SupportActionBar.SetDisplayHomeAsUpEnabled(true);
				SupportActionBar.SetDisplayShowHomeEnabled(true);
				SupportActionBar.SetDisplayShowTitleEnabled(true);
				SupportActionBar.SetHomeButtonEnabled(true);
			}
		}

		protected override void AttachBaseContext(Context @base)
		{
			base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
		}
	}
}