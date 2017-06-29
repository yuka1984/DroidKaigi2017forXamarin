#region

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;
using Autofac;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Droid.Views.Fragments;
using Reactive.Bindings.Extensions;
using Fragment = Android.Support.V4.App.Fragment;
using Toolbar = Android.Support.V7.Widget.Toolbar;

#endregion

namespace DroidKaigi2017.Droid.Views.Activities
{
	[Activity(Label = "DroidKaigi2017", MainLauncher = true, LaunchMode = LaunchMode.SingleTask,
		ConfigurationChanges = ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.Orientation
	)]
	public class MainActivity : ActivityBase
	{
		private Fragment _sessionFragment;
		private Fragment _SettingsFragment;

		private BottomNavigationView bottomNav;
		private TextView titleTextView;
		public LocaleUtil LocaleUtil { get; set; }

		public static Intent CreateIntent(Context context)
		{
			return new Intent(context, typeof(MainActivity));
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			GetComponentContext().InjectProperties(this);

			LocaleUtil.InitLocale(this);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);


			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

			bottomNav = FindViewById<BottomNavigationView>(Resource.Id.bottom_nav)
				.AddTo(CompositeDisposable);

			titleTextView = FindViewById<TextView>(Resource.Id.title)
				.AddTo(CompositeDisposable);

			bottomNav.NavigationItemSelected += (sender, args) =>
			{
				switch (args.Item.ItemId)
				{
					case Resource.Id.nav_settings:
						SwitchFragment(_SettingsFragment, _SettingsFragment.Tag);
						break;
					case Resource.Id.nav_sessions:
						SwitchFragment(_sessionFragment, SessionsFragment.Tag);
						break;
				}
			};

			InitFragments(bundle);

			GetComponentContext().InjectProperties(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		protected override void ConfigurationAction(ContainerBuilder containerBuilder)
		{
			containerBuilder.Register((c) => new Navigator(this)).As<INavigator>().SingleInstance();
		}

		private void InitFragments(Bundle savedInstanceState)
		{
			_SettingsFragment = SupportFragmentManager.FindFragmentByTag(SettingsFragment.Tag);
			_sessionFragment = SupportFragmentManager.FindFragmentByTag(SessionsFragment.Tag);

			if (_SettingsFragment == null)
				_SettingsFragment = new SettingsFragment();

			if (_sessionFragment == null)
				_sessionFragment = new SessionsFragment();

			if (savedInstanceState == null)
				SwitchFragment(_sessionFragment, SessionsFragment.Tag);
		}

		private bool SwitchFragment(Fragment fragment, string tag)
		{
			if (fragment.IsAdded)
				return false;

			var manager = SupportFragmentManager;
			var ft = manager.BeginTransaction();

			var currentFragment = manager.FindFragmentById(Resource.Id.content_view);
			if (currentFragment != null)
				ft.Detach(currentFragment);
			if (fragment.IsDetached)
				ft.Attach(fragment);
			else
				ft.Add(Resource.Id.content_view, fragment, tag);
			ft.SetTransition((int) FragmentTransit.FragmentFade)
				.Commit();

			// NOTE: When this method is called by user's continuous hitting at the same time,
			// transactions are queued, so necessary to reflect commit instantly before next transaction starts.
			manager.ExecutePendingTransactions();

			return true;
		}
	}
}