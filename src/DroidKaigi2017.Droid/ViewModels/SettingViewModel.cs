#region

using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Android.OS;
using DroidKaigi2017.Droid.Utils;
using Nyanto;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

#endregion

namespace DroidKaigi2017.Droid.ViewModels
{
	public class SettingViewModel : ViewModelBase
	{
		private readonly IAppSettings _appSettings;
		private readonly LocaleUtil _localeUtil;
		private readonly INavigator _navigator;

		public SettingViewModel(IAppSettings appSettings, LocaleUtil localeUtil, INavigator navigator)
		{
			_appSettings = appSettings;
			_localeUtil = localeUtil;
			_navigator = navigator;

			IsHeadsUp = new ReactiveProperty<bool>(_appSettings.HeadsUpFlag).AddTo(CompositeDisposable);
			IsHeadsUp
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => _appSettings.HeadsUpFlag = x)
				.AddTo(CompositeDisposable);

			IsNotify = new ReactiveProperty<bool>(_appSettings.NotificationFlag).AddTo(CompositeDisposable);
			IsNotify
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => { _appSettings.NotificationFlag = x; })
				.AddTo(CompositeDisposable);

			IsDebugOverray = new ReactiveProperty<bool>(_appSettings.ShowDebugOverlayView).AddTo(CompositeDisposable);
			IsDebugOverray
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => _appSettings.ShowDebugOverlayView = x)
				.AddTo(CompositeDisposable);

			IsShowLocalTimeSetting = new ReactiveProperty<bool>(_appSettings.ShowLocalTimeFlag).AddTo(CompositeDisposable);
			IsShowLocalTimeSetting
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => _appSettings.ShowLocalTimeFlag = x)
				.AddTo(CompositeDisposable);


			CurrentLnaguageId = new ReactiveProperty<string>(_localeUtil.GetCurrentLanguageId());
			CurrentLnaguageId.Skip(1).Subscribe(x => _navigator.ReStart()).AddTo(CompositeDisposable);
		}

		public ReactiveProperty<bool> IsShowLocalTimeSetting { get; }
		public bool ShowHeadsUpSettring => Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;
		public ReactiveProperty<bool> IsHeadsUp { get; }
		public ReactiveProperty<bool> IsNotify { get; }
		public ReactiveProperty<bool> IsDebugOverray { get; }
		public ReactiveProperty<string> CurrentLnaguageId { get; }
	}
}