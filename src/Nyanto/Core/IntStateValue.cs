#region

using Android.OS;

#endregion

namespace Nyanto.Core
{
	public class IntStateValue : Saveable
	{
		public IntStateValue(int i)
		{
			Value = i;
		}

		public int Value { get; set; }

		public override void SaveTo(Bundle savedState, string key)
		{
			savedState.PutInt(key, Value);
		}
	}

	public class SavedStateValue<T> : Saveable where T : IParcelable
	{
		private SavedStateValue(T mValue)
		{
			Value = mValue;
		}

		public T Value { get; set; }

		public override void SaveTo(Bundle savedState, string key)
		{
			savedState.PutParcelable(key, Value);
		}
	}
}