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
using DroidKaigi2017.Droid.ViewModels;
using DroidKaigi2017.Droid.Views.Fragments;

namespace DroidKaigi2017.Droid.Views.Activities
{
	[Activity(Label = "Session Information")]
	public class SessionDetailActivity : ActivityBase
	{
		private const string EXTRA_SESSION_ID = "session_id";
		private const string EXTRA_PARENT = "parent";

		public SessionDetailViewModel SessionDetailViewModel { get; set; }

		public static Intent createIntent(Context context, int sessionId, Type parentClass = null)
		{
			Intent intent = new Intent(context, typeof(SessionDetailActivity));
			intent.PutExtra(EXTRA_SESSION_ID, sessionId);
			if (parentClass != null)
			{
				intent.PutExtra(EXTRA_PARENT, parentClass.Name);
			}
			return intent;
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.activity_session_detail);
			GetComponentContext().InjectProperties(this);
			ReplaceFragment(new SessionDetailFragment(), Resource.Id.content_view);
			int sessionId = Intent.GetIntExtra(EXTRA_SESSION_ID, 0);
			SessionDetailViewModel.SelectSessionCommand.Execute(sessionId);
		}

		protected override void ConfigurationAction(ContainerBuilder containerBuilder)
		{
			
		}
	}
}