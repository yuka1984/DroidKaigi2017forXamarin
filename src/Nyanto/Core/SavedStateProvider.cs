#region

using System.Collections.Generic;
using Android.OS;
using Android.Util;
using Java.Lang;
using StringBuilder = System.Text.StringBuilder;

#endregion

namespace Nyanto.Core
{
	public class StateMap
	{
		private const string Tag = "StateProvider";
		public readonly Dictionary<string, IntStateValue> Map = new Dictionary<string, IntStateValue>();
		public Bundle MSavedState { get; set; }

		public IntStateValue IntValue(string key, int defaultValue)
		{
			IntStateValue intStateValue = null;
			if (Map.ContainsKey(key))
			{
				intStateValue = Map[key];
			}
			else
			{
				var value = MSavedState != null ? MSavedState.GetInt(key, defaultValue) : defaultValue;
				intStateValue = new IntStateValue(value);

				Map.Add(key, intStateValue);
			}

			return intStateValue;
		}

		private static void TypeWarning(string key, object value, string className, ClassCastException e)
		{
			var sb = new StringBuilder();
			sb.Append("Key ");
			sb.Append(key);
			sb.Append(" expected ");
			sb.Append(className);
			sb.Append(" but value was a ");
			sb.Append(value.GetType());
			Log.Warn("StateProvider", sb.ToString());
			Log.Warn("StateProvider", "Attempt to cast generated internal exception:", e);
		}
	}

	public class SavedStateProvider
	{
		private const string TAG = "SavedStateProvider";
		private readonly StateMap _mStateMap = new StateMap();

		public IntStateValue IntStateValue(string key)
		{
			return _mStateMap.IntValue(key, 0);
		}

		public IntStateValue IntStateValue(string key, int defaultValue)
		{
			return _mStateMap.IntValue(key, defaultValue);
		}

		public void RestoreState(Bundle savedState)
		{
			_mStateMap.MSavedState = savedState;
		}

		public void SaveState(Bundle outBundle)
		{
			if (_mStateMap.MSavedState != null)
				outBundle.PutAll(_mStateMap.MSavedState);

			var map = _mStateMap.Map;
			foreach (var item in map)
				item.Value.SaveTo(outBundle, item.Key);
		}
	}
}