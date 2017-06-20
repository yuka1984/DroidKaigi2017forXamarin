#region

using System;
using System.Runtime.CompilerServices;
using Android.Content;

#endregion

namespace DroidKaigi2017.Droid.Utils
{
	public interface IAppSettings
	{
		string LangurageId { get; set; }

		bool NotificationFlag { get; set; }

		bool HeadsUpFlag { get; set; }

		bool ShowLocalTimeFlag { get; set; }

		bool NotificationTestFlag { get; set; }

		bool ShowDebugOverlayView { get; set; }
	}

	public abstract class AppSettingsBase : IAppSettings
	{
		protected const bool DefaultNotificationFlag = true;
		protected const bool DefaultHeadsUpFlag = true;
		protected const bool DefaultShowLocalTimeFlag = false;
		protected const bool DefaultNotificationTestFlag = false;
		protected const bool DefaultShowDebugOverlayView = false;

		public string LangurageId
		{
			get => GetSetting("");
			set => SetSetting(value);
		}

		public bool NotificationFlag
		{
			get => GetSetting(DefaultNotificationFlag);
			set => SetSetting(value);
		}

		public bool HeadsUpFlag
		{
			get => GetSetting(DefaultHeadsUpFlag);
			set => SetSetting(value);
		}

		public bool ShowLocalTimeFlag
		{
			get => GetSetting(DefaultShowLocalTimeFlag);
			set => SetSetting(value);
		}

		public bool NotificationTestFlag
		{
			get => GetSetting(DefaultNotificationTestFlag);
			set => SetSetting(value);
		}

		public bool ShowDebugOverlayView
		{
			get => GetSetting(DefaultShowDebugOverlayView);
			set => SetSetting(value);
		}

		protected abstract T GetSetting<T>(T defaultValue, [CallerMemberName] string key = null);

		protected abstract void SetSetting<T>(T value, [CallerMemberName] string key = null);
	}

	public class AppSettings : AppSettingsBase
	{
		private readonly Context _context;

		public AppSettings(Context context)
		{
			_context = context;
		}

		protected override T GetSetting<T>(T defaultValue, [CallerMemberName] string key = null)
		{
			var pref = _context.GetSharedPreferences("DataSave", FileCreationMode.Private);
			if (defaultValue is bool)
				return (T) (object) pref.GetBoolean(key, (bool) (object) defaultValue);
			if (defaultValue is string)
				return (T) (object) pref.GetString(key, (string) (object) defaultValue);
			if (defaultValue is int)
				return (T) (object) pref.GetInt(key, (int) (object) defaultValue);
			throw new ArgumentException();
		}

		protected override void SetSetting<T>(T value, [CallerMemberName] string key = null)
		{
			var pref = _context.GetSharedPreferences("DataSave", FileCreationMode.Private);
			var edit = pref.Edit();
			if (typeof(T) == typeof(bool))
				edit.PutBoolean(key, (bool) (object) value);
			else if (typeof(T) == typeof(string))
				edit.PutString(key, (string) (object) value);
			else if (typeof(T) == typeof(int))
				edit.PutInt(key, (int) (object) value);
			else
				throw new ArgumentException();
			edit.Commit();
		}
	}
}