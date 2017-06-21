#region

using System;
using Android.Support.V4.App;
using Autofac;
using Nyanto.Core;

#endregion

namespace Nyanto
{
	public static class LifeTimeProviders
	{
		public static ObjectProvider<ILifetimeScope> GetProvider(Fragment fragment)
		{
			var fragmentActivity = fragment.Activity;
			if (fragmentActivity == null)
				throw new ArgumentException("Can\'t create ViewModelProvider for detached fragment");
			return new ObjectProvider<ILifetimeScope>(ObjectStores<ILifetimeScope>.Of(fragment), () =>
			{
				var activitybase = fragmentActivity as AppCompatActivityBase;
				if (activitybase == null)
					throw new ArgumentException();

				return activitybase.GetComponentContext().BeginLifetimeScope();
			});
		}

		public static ObjectProvider<ILifetimeScope> Of(FragmentActivity activity, Action<ContainerBuilder> configurationAction = null)
		{
			return new ObjectProvider<ILifetimeScope>(ObjectStores<ILifetimeScope>.Of(activity), () =>
			{
				var mainApp = activity.Application as ApplicationBase;
				if (mainApp == null)
					throw new ArgumentException();
				if (configurationAction == null)
				{
					return mainApp.ApplicationComponentContext.BeginLifetimeScope();
				}

				return mainApp.ApplicationComponentContext.BeginLifetimeScope(configurationAction);

			});
		}
	}
}