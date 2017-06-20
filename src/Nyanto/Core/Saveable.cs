#region

using Android.OS;

#endregion

namespace Nyanto.Core
{
	public abstract class Saveable
	{
		public abstract void SaveTo(Bundle var1, string var2);
	}
}