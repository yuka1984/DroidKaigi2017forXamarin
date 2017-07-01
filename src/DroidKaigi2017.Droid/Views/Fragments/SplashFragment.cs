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
using Java.Lang;
using Java.Util.Concurrent;
using Nyanto;

namespace DroidKaigi2017.Droid.Views.Fragments
{
	public class SplashFragment : FragmentBase
	{
		private readonly IScheduledExecutorService _scheduledExecutorService = Executors.NewSingleThreadScheduledExecutor();
		public override int ViewResourceId => Resource.Layout.fragment_splash;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = base.OnCreateView(inflater, container, savedInstanceState);
			var accessor = new fragment_splash_holder(view);

			_scheduledExecutorService.ScheduleAtFixedRate(new Runnable(() =>
			{
				accessor.particle_animation_view.PostInvalidate();
			}), 0, 40L, TimeUnit.Milliseconds);

			return view;
		}

		protected override void Bind(View view)
		{
			

		}

		public override void OnDestroyView()
		{
			base.OnDestroyView();
			_scheduledExecutorService.Shutdown();
		}
	}
}