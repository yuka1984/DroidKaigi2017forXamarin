#region

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.App;
using Android.OS;
using Object = Java.Lang.Object;

#endregion

namespace Nyanto.Core
{
	public class ActivityLifecycleManager : Object, Application.IActivityLifecycleCallbacks
	{
		private readonly Dictionary<int, ActivityLifeCycycle> _dic = new Dictionary<int, ActivityLifeCycycle>();

		private readonly Subject<Tuple<int, ActivityLifeCycycle>> _lifecycleSubject =
			new Subject<Tuple<int, ActivityLifeCycycle>>();

		public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
		{
			Upsert(activity, ActivityLifeCycycle.Created);
		}


		public void OnActivityDestroyed(Activity activity)
		{
			var hash = activity.GetHashCode();
			if (_dic.ContainsKey(hash))
				_dic.Remove(hash);
			_lifecycleSubject.OnNext(Tuple.Create(hash, ActivityLifeCycycle.Destroyed));
		}

		public void OnActivityPaused(Activity activity)
		{
			Upsert(activity, ActivityLifeCycycle.Paused);
		}

		public void OnActivityResumed(Activity activity)
		{
			Upsert(activity, ActivityLifeCycycle.Resumed);
		}

		public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
		{
			Upsert(activity, ActivityLifeCycycle.SaveInstanceState);
		}

		public void OnActivityStarted(Activity activity)
		{
			Upsert(activity, ActivityLifeCycycle.Started);
		}

		public void OnActivityStopped(Activity activity)
		{
			Upsert(activity, ActivityLifeCycycle.Stopped);
		}

		public ActivityLifeCycycle GetState(Activity activity)
		{
			var hash = activity.GetHashCode();
			if (_dic.ContainsKey(hash))
				return _dic[hash];
			return ActivityLifeCycycle.Destroyed;
		}

		public IObservable<bool> GetActivityAvtiveStateObsrevable(Activity activity)
		{
			return _lifecycleSubject.Where(x => x.Item1 == activity.GetHashCode())
				.Select(x =>
				{
					return x.Item2 == ActivityLifeCycycle.Resumed || x.Item2 == ActivityLifeCycycle.Started ||
					       x.Item2 == ActivityLifeCycycle.Paused;
				});
		}

		private void Upsert(Activity activity, ActivityLifeCycycle lifecycle)
		{
			var hash = activity.GetHashCode();
			if (!_dic.ContainsKey(hash))
				_dic.Add(hash, lifecycle);
			else
				_dic[hash] = lifecycle;
			_lifecycleSubject.OnNext(Tuple.Create(hash, lifecycle));
		}
	}
}