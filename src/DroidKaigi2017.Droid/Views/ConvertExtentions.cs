#region

using Android.Views;

#endregion

namespace DroidKaigi2017.Droid.Views
{
	public static class ConvertExtentions
	{
		public static ViewStates ToViewStates(this bool value)
		{
			return value ? ViewStates.Visible : ViewStates.Gone;
		}
	}
}