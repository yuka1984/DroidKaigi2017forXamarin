#region

using System;

#endregion

namespace DroidKaigi2017.Droid.Utils
{
	public class AnonymousDisposable : IDisposable
	{
		private readonly Action dispose;
		private bool isDisposed;

		public AnonymousDisposable(Action dispose)
		{
			this.dispose = dispose;
		}

		public void Dispose()
		{
			if (!isDisposed)
			{
				isDisposed = true;
				dispose();
			}
		}
	}
}