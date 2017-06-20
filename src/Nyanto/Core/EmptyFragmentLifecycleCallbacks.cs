#region

using System;
using Android.OS;
using Android.Support.V4.App;

#endregion

namespace Nyanto.Core
{
	public class EmptyFragmentLifecycleCallbacks : FragmentManager.FragmentLifecycleCallbacks
	{
		public Action<FragmentManager, Fragment, Bundle> OnFragmentActivityCreatedAction { get; set; }

		public Action<FragmentManager, Fragment> OnFragmentDestroyedAction { get; set; }

		public override void OnFragmentActivityCreated(FragmentManager fm, Fragment f, Bundle savedInstanceState)
		{
			base.OnFragmentActivityCreated(fm, f, savedInstanceState);
			OnFragmentActivityCreatedAction?.Invoke(fm, f, savedInstanceState);
		}

		public override void OnFragmentDestroyed(FragmentManager fm, Fragment f)
		{
			base.OnFragmentDestroyed(fm, f);
			OnFragmentDestroyedAction?.Invoke(fm, f);
		}
	}
}