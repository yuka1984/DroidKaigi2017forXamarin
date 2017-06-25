#region

using System.Windows.Input;
using Android.Views;
using Reactive.Bindings;

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

	public static class ICommandExtentions
	{
		public static void CheckExecute(this ICommand command, object param)
		{
			if(command == null)
				return;
			if (command.CanExecute(param))
			{
				command.Execute(param);
			}
		}

		public static void CheckExecute<T>(this ReactiveCommand<T> command, T param)
		{
			if (command == null)
				return;
			if (command.CanExecute())
			{
				command.Execute(param);
			}
		}
	}
}