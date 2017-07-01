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
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Droid.Views.Fragments;

namespace DroidKaigi2017.Droid.Views.Activities
{
	[Activity(Label = "MySessionsActivity")]
	public class MySessionsActivity : ActivityBase
	{
		public static Intent CreateIntent(Context context)
		{
			return new Intent(context, typeof(MySessionsActivity));
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_my_sessions);

			var accessor = new activity_my_sessions_holder(this);
			InitBackToolbar(accessor.toolbar);

			ReplaceFragment(new MySessionsFragment(), Resource.Id.content_view);
		}

		protected override void ConfigurationAction(ContainerBuilder containerBuilder)
		{
			containerBuilder.Register(c => new Navigator(this)).As<INavigator>().SingleInstance();
		}
	}
}