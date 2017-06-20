#region

using System;
using DroidKaigi2017.Droid.Views.Activities;

#endregion

namespace DroidKaigi2017.Droid.Utils
{
	public interface INavigator
	{
		void ReStart();
	}

	public class Navigator : INavigator
	{
		public MainActivity MainActivity { get; set; }

		public void ReStart()
		{
			if (MainActivity == null)
				throw new ArgumentException(nameof(MainActivity));

			MainActivity.Finish();
			MainActivity.StartActivity(MainActivity.CreateIntent(MainActivity));
			MainActivity.OverridePendingTransition(0, 0);
		}
	}
}