#region

using System;
using System.Collections.Generic;
using Android.Content;
using Java.Text;
using Java.Util;
using Reactive.Bindings;
using TimeZone = Java.Util.TimeZone;

#endregion

namespace DroidKaigi2017.Droid.Utils
{
	public static class LocalExtentions
	{
		public static string ToLocaleLanguageId(this Locale locale)
		{
			return locale.Language.ToLower();
		}
	}

	public class LocaleUtil : IDisposable
	{
		private static readonly Locale DefaultLang = Locale.English;

		private static string TAG = typeof(LocaleUtil).Name;

		private static readonly TimeZone ConferenceTimezone = TimeZone.GetTimeZone(BuildConfig.ConferenceTimezone);
		private readonly IAppSettings _appSettings;

		private readonly ReactiveProperty<string> _localeProperty;

		public readonly IReadOnlyList<Locale> SupportLang = new List<Locale> {Locale.Japanese, Locale.English};

		public LocaleUtil(IAppSettings appSettings)
		{
			_appSettings = appSettings;
			_localeProperty = new ReactiveProperty<string>(GetCurrentLanguageId());
		}

		public IObservable<string> CurrentLanguageIdObservable => _localeProperty;

		public void Dispose()
		{
			_localeProperty?.Dispose();
		}

		public void InitLocale(Context context)
		{
			SetLocale(context, GetCurrentLanguageId());
		}

		public void SetLocale(Context context, string languageId)
		{
			var config = context.Resources.Configuration;
			_appSettings.LangurageId = languageId;
			var locale = new Locale(languageId);
			Locale.Default = locale;
			config.Locale = locale;

			// updateConfiguration, deprecated in API 25.
			context.Resources.UpdateConfiguration(config, context.Resources.DisplayMetrics);

			_localeProperty.Value = languageId;
		}

		public string GetCurrentLanguageId()
		{
			// This value would be stored language id or empty.
			var languageId = _appSettings.LangurageId;
			if (string.IsNullOrEmpty(languageId))
				languageId = Locale.Default.ToLocaleLanguageId();

			foreach (var locale in SupportLang)
				if (string.Equals(languageId, locale.ToLocaleLanguageId(), StringComparison.CurrentCultureIgnoreCase))
					return languageId;

			return DefaultLang.ToLocaleLanguageId();
		}


		public string GetCurrentLanguage(Context context)
		{
			return context.GetString(GetLanguage(GetCurrentLanguageId()));
		}

		public static string GetDisplayLanguage(Context context, Locale locale)
		{
			var languageId = locale.ToLocaleLanguageId();
			return GetDisplayLanguage(context, $"lang_{languageId}_in_{languageId}");
		}

		private static string GetDisplayLanguage(Context context, string resName)
		{
			try
			{
				var resourceId = context.Resources.GetIdentifier(resName, "string", context.PackageName);
				if (resourceId > 0)
					return context.GetString(resourceId);
				throw new Exception();
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public static int GetLanguage(string languageId)
		{
			if (string.Equals(languageId, Locale.English.ToLocaleLanguageId(), StringComparison.CurrentCultureIgnoreCase))
				return Resource.String.lang_en;
			if (string.Equals(languageId, Locale.Japanese.ToLocaleLanguageId(), StringComparison.CurrentCultureIgnoreCase))
				return Resource.String.lang_ja;
			return Resource.String.lang_en;
		}

		public Date GetDisplayDate(Date date)
		{
			var formatTokyo = DateFormat.DateTimeInstance;
			formatTokyo.TimeZone = ConferenceTimezone;
			var formatLocal = DateFormat.DateTimeInstance;
			formatLocal.TimeZone = GetDisplayTimeZone();
			try
			{
				return formatLocal.Parse(formatTokyo.Format(date));
			}
			catch (ParseException e)
			{
				return date;
			}
		}

		public TimeZone GetDisplayTimeZone()
		{
			var defaultTimeZone = TimeZone.Default;
			var shouldShowLocalTime = _appSettings.ShowLocalTimeFlag;
			return shouldShowLocalTime && defaultTimeZone != null ? defaultTimeZone : ConferenceTimezone;
		}
	}
}