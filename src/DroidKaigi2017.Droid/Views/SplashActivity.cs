using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autofac;
using DroidKaigi2017.Droid.Views.Activities;
using DroidKaigi2017.Droid.Views.Fragments;
using DroidKaigi2017.Interface.Services;

namespace DroidKaigi2017.Droid.Views
{
	[Activity(Label = "DroidKaigi2017", MainLauncher = true, LaunchMode = LaunchMode.SingleTask, NoHistory = true)]
	public class SplashActivity : ActivityBase
	{
		protected override void ConfigurationAction(ContainerBuilder containerBuilder)
		{
			
		}

		public ISessionService SessionService { get; set; }
		public IMySessionService MySessionService { get; set; }
		public IFeedBackService FeedBackService { get; set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			GetComponentContext().InjectProperties(this);

			SetContentView(Resource.Layout.activity_splash);

			ReplaceFragment(new SplashFragment(), Resource.Id.content_view);

		}

		protected override void OnStart()
		{
			base.OnStart();

			Task.WhenAll(SessionService.LoadAsync(),
					MySessionService.LoadAsync(),
					FeedBackService.LoadAsync())
				.ContinueWith(task =>
				{
					StartActivity(MainActivity.CreateIntent(this));
					Finish();
				});


		}
	}
}