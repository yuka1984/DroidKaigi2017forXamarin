#region

using Android.Support.V4.App;

#endregion

namespace Nyanto.Core
{
	public static class ObjectStores<T>
	{
		public static ObjectStore<T> Of(FragmentActivity activity)
		{
			return HolderFragment<T>.HolderFragmentFor(activity).GetObjectStore();
		}

		public static ObjectStore<T> Of(Fragment fragment)
		{
			return HolderFragment<T>.HolderFragmentFor(fragment).GetObjectStore();
		}
	}
}