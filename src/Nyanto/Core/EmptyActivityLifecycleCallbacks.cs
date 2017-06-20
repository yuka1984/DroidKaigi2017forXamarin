#region

using System;
using Android.App;
using Android.OS;
using Object = Java.Lang.Object;

#endregion

namespace Nyanto.Core
{
	public class EmptyActivityLifecycleCallbacks : Object, Application.IActivityLifecycleCallbacks
	{
		public Action<Activity, Bundle> OnActivityCreatedAction { get; set; }

		public Action<Activity> OnActivityDestroyedAction { get; set; }

		public Action<Activity> OnActivityPausedAction { get; set; }

		public Action<Activity> OnActivityResumedAction { get; set; }

		public Action<Activity, Bundle> OnActivitySaveInstanceStateAction { get; set; }

		public Action<Activity> OnActivityStartedAction { get; set; }

		public Action<Activity> OnActivityStoppedAction { get; set; }

		public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
		{
			OnActivityCreatedAction?.Invoke(activity, savedInstanceState);
		}

		public void OnActivityDestroyed(Activity activity)
		{
			OnActivityDestroyedAction?.Invoke(activity);
		}

		public void OnActivityPaused(Activity activity)
		{
			OnActivityPausedAction?.Invoke(activity);
		}

		public void OnActivityResumed(Activity activity)
		{
			OnActivityResumedAction?.Invoke(activity);
		}

		public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
		{
			OnActivitySaveInstanceStateAction?.Invoke(activity, outState);
		}

		public void OnActivityStarted(Activity activity)
		{
			OnActivityStartedAction?.Invoke(activity);
		}

		public void OnActivityStopped(Activity activity)
		{
			OnActivityStoppedAction?.Invoke(activity);
		}
	}
}