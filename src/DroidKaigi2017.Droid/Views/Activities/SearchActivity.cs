using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autofac;
using DroidKaigi2017.Droid.Views.Fragments;

namespace DroidKaigi2017.Droid.Views.Activities
{
	[Activity(Label = "SearchActivity")]
	public class SearchActivity : ActivityBase
	{
		private activity_search_holder holder;
		public static Intent CreateIntent(Context context)
		{
			return new Intent(context, typeof(SearchActivity));
		}

	protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			OverridePendingTransition(0, Resource.Animation.activity_fade_exit);
			SetContentView(Resource.Layout.activity_search);
			holder = new activity_search_holder(this);
			InitBackToolbar(holder.toolbar);
			ReplaceFragment(new SearchFragment(), Resource.Id.content_view);
		}

		public override void Finish()
		{
			base.Finish();
			OverridePendingTransition(0, Resource.Animation.activity_fade_exit);
		}

		public override void OnBackPressed()
		{
			base.OnBackPressed();
			Finish();
		}

		protected override void ConfigurationAction(ContainerBuilder containerBuilder)
		{
				
		}
	}
}