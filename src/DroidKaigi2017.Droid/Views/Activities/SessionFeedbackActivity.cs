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
using DroidKaigi2017.Droid.ViewModels;
using DroidKaigi2017.Droid.Views.Fragments;

namespace DroidKaigi2017.Droid.Views.Activities
{
	[Activity(Label = "SessionFeedBack", Exported = false, Theme = "@style/AppTheme.Translucent.Half")]
	public class SessionFeedbackActivity : ActivityBase
	{
		private const string EXTRA_SESSION_ID = "session_id";

		public SessionFeedbackViewModel FeedbackViewModel { get; set; }

		public static Intent CreateIntent(Context context, int sessionId)
		{
			Intent intent = new Intent(context, typeof(SessionFeedbackActivity));
			intent.PutExtra(EXTRA_SESSION_ID, sessionId);
			return intent;
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			
			SetContentView(Resource.Layout.activity_session_feedback);
			var holder = new activity_session_feedback_holder(this);

			InitBackToolbar(holder.toolbar);

			int sessionId = Intent.GetIntExtra(EXTRA_SESSION_ID, 0);
			ReplaceFragment(new SessionFeedbackFragment(), Resource.Id.content_view);
			FeedbackViewModel.LoadCommand.CheckExecute(sessionId);
		}

		protected override void ConfigurationAction(ContainerBuilder containerBuilder)
		{
			containerBuilder.Register(c => new Navigator(this)).As<INavigator>().SingleInstance();
		}
	}
}