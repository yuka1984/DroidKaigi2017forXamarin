#region

using System.Collections.Generic;
using Android.App;
using Android.Support.V4.App;
using Java.Lang;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

#endregion

namespace Nyanto.Core
{
	public class HolderFragmentManager<T>
	{
		private readonly Application.IActivityLifecycleCallbacks _mActivityCallbacks;

		private readonly Dictionary<Activity, HolderFragment<T>> _mNotCommittedActivityHolders =
			new Dictionary<Activity, HolderFragment<T>>();

		private readonly Dictionary<Fragment, HolderFragment<T>> _mNotCommittedFragmentHolders =
			new Dictionary<Fragment, HolderFragment<T>>();

		private readonly FragmentManager.FragmentLifecycleCallbacks _mParentDestroyedCallback;
		private bool _mActivityCallbacksIsAdded;

		public HolderFragmentManager()
		{
			_mActivityCallbacks = new EmptyActivityLifecycleCallbacks
			{
				OnActivityDestroyedAction = activity =>
				{
					if (_mNotCommittedActivityHolders.ContainsKey(activity))
						_mNotCommittedActivityHolders.Remove(activity);
				}
			};

			_mParentDestroyedCallback = new EmptyFragmentLifecycleCallbacks
			{
				OnFragmentDestroyedAction = (fm, f) =>
				{
					if (_mNotCommittedFragmentHolders.ContainsKey(f))
						_mNotCommittedFragmentHolders.Remove(f);
				}
			};
		}

		public void HolderFragmentCreated(Fragment holderFragment)
		{
			var parentFragment = holderFragment.ParentFragment;
			if (parentFragment != null)
			{
				_mNotCommittedFragmentHolders.Remove(parentFragment);
				parentFragment.FragmentManager.UnregisterFragmentLifecycleCallbacks(_mParentDestroyedCallback);
			}
			else
			{
				_mNotCommittedActivityHolders.Remove(holderFragment.Activity);
			}
		}

		public static HolderFragment<T> FindHolderFragment(FragmentManager manager)
		{
			if (manager.IsDestroyed)
				throw new IllegalStateException("Can\'t access ViewModels from onDestroy");
			var fragmentByTag = manager.FindFragmentByTag("android.arch.lifecycle.state.StateProviderHolderFragment");

			if (fragmentByTag != null && !(fragmentByTag is HolderFragment<T>))
				throw new IllegalStateException("Unexpected fragment instance was returned by HOLDER_TAG");
			return (HolderFragment<T>) fragmentByTag;
		}

		public static HolderFragment<T> CreateHolderFragment(FragmentManager fragmentManager)
		{
			var holder = new HolderFragment<T>();
			fragmentManager.BeginTransaction()
				.Add(holder, "android.arch.lifecycle.state.StateProviderHolderFragment")
				.CommitAllowingStateLoss();
			return holder;
		}

		public HolderFragment<T> HolderFragmentFor(FragmentActivity activity)
		{
			var fm = activity.SupportFragmentManager;
			var holder = FindHolderFragment(fm);
			if (holder != null)
				return holder;

			if (_mNotCommittedActivityHolders.ContainsKey(activity))
				return _mNotCommittedActivityHolders[activity];

			if (!_mActivityCallbacksIsAdded)
			{
				_mActivityCallbacksIsAdded = true;
				activity.Application.RegisterActivityLifecycleCallbacks(_mActivityCallbacks);
			}

			holder = CreateHolderFragment(fm);
			_mNotCommittedActivityHolders.Add(activity, holder);
			return holder;
		}

		public HolderFragment<T> HolderFragmentFor(Fragment parentFragment)
		{
			var fm = parentFragment.ChildFragmentManager;
			var holder = FindHolderFragment(fm);
			if (holder != null)
				return holder;

			if (_mNotCommittedFragmentHolders.ContainsKey(parentFragment))
				return _mNotCommittedFragmentHolders[parentFragment];
			parentFragment.FragmentManager.RegisterFragmentLifecycleCallbacks(_mParentDestroyedCallback, false);
			holder = CreateHolderFragment(fm);
			_mNotCommittedFragmentHolders.Add(parentFragment, holder);
			return holder;
		}
	}
}