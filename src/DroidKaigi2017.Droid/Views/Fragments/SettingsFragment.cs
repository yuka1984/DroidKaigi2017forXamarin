#region

using System;
using System.Linq;
using System.Reactive.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Droid.ViewModels;
using DroidKaigi2017.Droid.Views.CustomViews;
using Java.Util;
using Nyanto;
using Reactive.Bindings.Binding;
using Reactive.Bindings.Extensions;

#endregion

namespace DroidKaigi2017.Droid.Views.Fragments
{
	public class SettingsFragment : FragmentBase
	{
		public new static readonly string Tag = typeof(SettingsFragment).Name;

		private View _langurageView;
		private SettingSwitchRowView headsUpSwitch;
		private SettingSwitchRowView localTimeSwitch;
		private SettingSwitchRowView notifySwtich;
		private SettingSwitchRowView overraySwitch;
		private TextView txtLnaguage;
		public LocaleUtil LocaleUtil { get; set; }

		public SettingViewModel ViewModel { get; set; }

		public override int ViewResourceId => Resource.Layout.fragment_settings;

		protected override void Bind(View view)
		{
			_langurageView = view
				.FindViewById(Resource.Id.language_settings_container)
				.AddTo(CompositeDisposable);
			_langurageView
				.ClickAsObservable()
				.Subscribe(x => ShowLanguagesDialog());

			txtLnaguage = view.FindViewById<TextView>(Resource.Id.txt_language);

			ViewModel.CurrentLnaguageId
				.Select(x => LocaleUtil.GetDisplayLanguage(Context, new Locale(x)))
				.Subscribe(x => { txtLnaguage.Text = x; })
				.AddTo(CompositeDisposable);

			localTimeSwitch = view
					.FindViewById<SettingSwitchRowView>(Resource.Id.local_time_switch_row)
					.AddTo(CompositeDisposable)
				;
			localTimeSwitch.IsChecked = ViewModel.IsShowLocalTimeSetting.Value;
			ViewModel.IsShowLocalTimeSetting.BindTo(localTimeSwitch, sw => sw.IsChecked, BindingMode.OneWayToSource
					, targetUpdateTrigger: localTimeSwitch.ObserveProperty(x => x.IsChecked).ToUnit())
				.AddTo(CompositeDisposable);

			notifySwtich = view
					.FindViewById<SettingSwitchRowView>(Resource.Id.notification_switch_row)
					.AddTo(CompositeDisposable)
				;
			notifySwtich.IsChecked = ViewModel.IsNotify.Value;
			ViewModel.IsNotify.BindTo(notifySwtich, sw => sw.IsChecked, BindingMode.OneWayToSource
					, targetUpdateTrigger: notifySwtich.ObserveProperty(x => x.IsChecked).ToUnit())
				.AddTo(CompositeDisposable);


			view.FindViewById(Resource.Id.heads_up_border_top)
				.Visibility = ViewModel.ShowHeadsUpSettring.ToViewStates();

			headsUpSwitch = view
				.FindViewById<SettingSwitchRowView>(Resource.Id.heads_up_switch_row)
				.AddTo(CompositeDisposable);

			headsUpSwitch.Visibility = ViewModel.ShowHeadsUpSettring.ToViewStates();
			ViewModel.IsNotify.BindTo(headsUpSwitch, sw => sw.Enabled).AddTo(CompositeDisposable);
			headsUpSwitch.IsChecked = ViewModel.IsHeadsUp.Value;
			ViewModel.IsHeadsUp.BindTo(headsUpSwitch, sw => sw.IsChecked, BindingMode.OneWayToSource,
					targetUpdateTrigger: headsUpSwitch.ObserveProperty(x => x.IsChecked).ToUnit())
				.AddTo(CompositeDisposable);


			view.FindViewById(Resource.Id.heads_up_border_bottom)
				.Visibility = ViewModel.ShowHeadsUpSettring.ToViewStates();

			overraySwitch = view
				.FindViewById<SettingSwitchRowView>(Resource.Id.debug_overlay_view_switch_row)
				.AddTo(CompositeDisposable);

			overraySwitch.IsChecked = ViewModel.IsDebugOverray.Value;
			ViewModel.IsDebugOverray.BindTo(overraySwitch, sw => sw.IsChecked, BindingMode.OneWayToSource,
					targetUpdateTrigger: overraySwitch.ObserveProperty(x => x.IsChecked).ToUnit())
				.AddTo(CompositeDisposable);
		}

		public void ShowLanguagesDialog()
		{
			var locales = LocaleUtil.SupportLang;
			var languages = locales.Select(x => LocaleUtil.GetDisplayLanguage(Context, x)).ToArray();
			var languageIds = locales.Select(x => x.ToLocaleLanguageId()).ToArray();
			var currentLanguageId = LocaleUtil.GetCurrentLanguageId();

			var defaultItem = Array.IndexOf(languageIds, currentLanguageId);
			new AlertDialog.Builder(Context, Resource.Style.DialogTheme)
				.SetTitle(Resource.String.settings_language)
				.SetSingleChoiceItems(languages, defaultItem
					, (sender, args) =>
					{
						ViewModel.CurrentLnaguageId.Value = languageIds[args.Which];
						((AlertDialog) sender).Dismiss();
					})
				.SetNegativeButton(Android.Resource.String.Cancel, (sender, args) => { ((AlertDialog) sender).Dismiss(); })
				.Show()
				;
		}
	}
}