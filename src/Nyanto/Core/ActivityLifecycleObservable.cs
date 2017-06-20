#region

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.App;
using Android.OS;
using Object = Java.Lang.Object;

#endregion

namespace Nyanto.Core
{
	public class ActivityLifecycleObservable : Object, Application.IActivityLifecycleCallbacks
	{
		private readonly Subject<ActivityLifeCycycle> _activityLifeCycleSubject = new Subject<ActivityLifeCycycle>();

		public ActivityLifecycleObservable()
		{
			ActivityEnabledObservable = _activityLifeCycleSubject
				.Where(x => x == ActivityLifeCycycle.Started || x == ActivityLifeCycycle.Stopped)
				.Select(x => x == ActivityLifeCycycle.Started ? true : false);
		}

		public IObservable<ActivityLifeCycycle> LifeCycleObservable => _activityLifeCycleSubject;
		public IObservable<bool> ActivityEnabledObservable { get; }

		public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
		{
			_activityLifeCycleSubject.OnNext(ActivityLifeCycycle.Created);
		}

		public void OnActivityDestroyed(Activity activity)
		{
			_activityLifeCycleSubject.OnNext(ActivityLifeCycycle.Destroyed);
			_activityLifeCycleSubject.OnCompleted();
		}

		public void OnActivityPaused(Activity activity)
		{
			_activityLifeCycleSubject.OnNext(ActivityLifeCycycle.Paused);
		}

		public void OnActivityResumed(Activity activity)
		{
			_activityLifeCycleSubject.OnNext(ActivityLifeCycycle.Resumed);
		}

		public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
		{
			_activityLifeCycleSubject.OnNext(ActivityLifeCycycle.SaveInstanceState);
			;
		}

		public void OnActivityStarted(Activity activity)
		{
			_activityLifeCycleSubject.OnNext(ActivityLifeCycycle.Started);
		}

		public void OnActivityStopped(Activity activity)
		{
			_activityLifeCycleSubject.OnNext(ActivityLifeCycycle.Stopped);
		}

		public void Dispose()
		{
			_activityLifeCycleSubject.Dispose();
		}
	}
}