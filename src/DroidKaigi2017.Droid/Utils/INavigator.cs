#region

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using DroidKaigi2017.Droid.ViewModels;
using DroidKaigi2017.Droid.Views.Activities;

#endregion

namespace DroidKaigi2017.Droid.Utils
{
	public interface INavigator
	{
		void ReStart();
		void NavigateTo(string key, params object[] param);

		Task<bool> DisplayAlert(int titleResId, int messageResId, int okResId, int cancelResId);
		void OpenUrl(string url);
		void Finish();
	}

	public static class NavigationKey
	{
		public const string GoSessionDetail = "SessionDetail";
		public const string GoSessionFeedBack = "SessonFeedBack";
		public const string GoSearch = "GoSearch";
	}

	public class Navigator : INavigator
	{
		public readonly Activity _MainActivity;

		public Navigator(Activity mainActivity)
		{
			_MainActivity = mainActivity;
		}

		public void ReStart()
		{
			_MainActivity.Finish();
			_MainActivity.StartActivity(MainActivity.CreateIntent(_MainActivity));
			_MainActivity.OverridePendingTransition(0, 0);
		}

		public void NavigateTo(string key, params object[] param)
		{
			switch (key)
			{
				case NavigationKey.GoSessionDetail:
					NavigateToSessionDetail((SessionViewModel)param[0]);
					break;
				case NavigationKey.GoSessionFeedBack:
					NavigateToSessionFeedBack((int)param[0]);
					break;
				case NavigationKey.GoSearch:
					NavigateToSearch();
					break;
					
			}
		}

		public Task<bool> DisplayAlert(int titleResId, int messageResId, int okResId, int cancelResId)
		{
			var taskCompletionSource = new TaskCompletionSource<bool>();

			new AlertDialog.Builder(_MainActivity, Resource.Style.DialogTheme)
				.SetTitle(titleResId)
				.SetMessage(messageResId)
				.SetPositiveButton(okResId, (sender, args) => {taskCompletionSource.SetResult(true);})
				.SetNegativeButton(cancelResId, (sender, args) =>{taskCompletionSource.SetResult(false);})
				.Show();

			return taskCompletionSource.Task;
		}

		public void OpenUrl(string url)
		{
			Android.Net.Uri uri = Android.Net.Uri.Parse(url);
			Intent i = new Intent(Intent.ActionView, uri);
			_MainActivity.StartActivity(i);
		}

		public void Finish()
		{
			_MainActivity.Finish();
		}

		private void NavigateToSessionDetail(SessionViewModel sessionViewModel)
		{
			_MainActivity.StartActivity(
				SessionDetailActivity.createIntent(_MainActivity, sessionViewModel.SessionId, typeof(MainActivity)));
		}

		private void NavigateToSessionFeedBack(int sessionId)
		{
			_MainActivity.StartActivity(SessionFeedbackActivity.CreateIntent(_MainActivity, sessionId));
		}

		private void NavigateToSearch()
		{
			_MainActivity.StartActivity(SearchActivity.CreateIntent(_MainActivity));
		}
	}
}