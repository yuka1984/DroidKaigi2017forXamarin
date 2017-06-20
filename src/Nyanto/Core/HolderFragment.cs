#region

using Android.OS;
using Android.Support.V4.App;

#endregion

namespace Nyanto.Core
{
	public class HolderFragment<T> : Fragment
	{
		private const string LogTag = "ObjectStores";
		public const string HolderTag = "android.arch.lifecycle.state.StateProviderHolderFragment";
		private static readonly HolderFragmentManager<T> SHolderFragmentManager = new HolderFragmentManager<T>();
		private readonly SavedStateProvider _mSavedStateProvider = new SavedStateProvider();
		private readonly ObjectStore<T> _mStore = new ObjectStore<T>();

		public HolderFragment()
		{
			RetainInstance = true;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			_mSavedStateProvider.RestoreState(savedInstanceState);
			SHolderFragmentManager.HolderFragmentCreated(this);
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			_mSavedStateProvider.SaveState(outState);
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			_mStore.Clear();
		}

		public SavedStateProvider GetSavedStateProvider()
		{
			return _mSavedStateProvider;
		}

		public ObjectStore<T> GetObjectStore()
		{
			return _mStore;
		}

		public static HolderFragment<T> HolderFragmentFor(FragmentActivity activity)
		{
			return SHolderFragmentManager.HolderFragmentFor(activity);
		}

		public static HolderFragment<T> HolderFragmentFor(Fragment fragment)
		{
			return SHolderFragmentManager.HolderFragmentFor(fragment);
		}
	}
}