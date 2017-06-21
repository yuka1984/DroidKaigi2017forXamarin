#region

using System;
using DroidKaigi2017.Droid.ViewModels;
using DroidKaigi2017.Droid.Views.Activities;

#endregion

namespace DroidKaigi2017.Droid.Utils
{
	public interface INavigator
	{
		void ReStart();
		void NavigateToSessionDetail(SessionViewModel sessionViewModel);
	}

	public class Navigator : INavigator
	{
		public readonly MainActivity _MainActivity;

		public Navigator(MainActivity mainActivity)
		{
			_MainActivity = mainActivity;
		}

		public void ReStart()
		{
			_MainActivity.Finish();
			_MainActivity.StartActivity(MainActivity.CreateIntent(_MainActivity));
			_MainActivity.OverridePendingTransition(0, 0);
		}

		public void NavigateToSessionDetail(SessionViewModel sessionViewModel)
		{
			_MainActivity.StartActivity(
				SessionDetailActivity.createIntent(_MainActivity, sessionViewModel.SessionId, typeof(MainActivity)));
		}
	}
}