#region

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Android.Views;

#endregion

namespace DroidKaigi2017.Droid.Utils
{
	public class AnonymousOnGlobalLayoutListenerView : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
	{
		public Action OnGlobalLayoutAction { get; set; }
		public void OnGlobalLayout()
		{
			OnGlobalLayoutAction?.Invoke();
		}
	}

	public interface IOnGlobalLayoutListener
	{
		bool OnGlobalLayout();
	}

	public class AnonymousOnGlobalLayoutListener : IOnGlobalLayoutListener
	{
		public Func<bool> OnGlobalLayoutAction { get; set; }
		public bool OnGlobalLayout()
		{
			return OnGlobalLayoutAction?.Invoke() ?? false;
		}
	}

	public class ViewUtil
	{
		public static readonly string Tag = typeof(ViewUtil).Name;

		public void AddOneTimeOnGlobalLayoutListener(View view, IOnGlobalLayoutListener globalLayoutListener)
		{
			ViewTreeObserver.IOnGlobalLayoutListener l = null;
			l = new AnonymousOnGlobalLayoutListenerView
			{
				OnGlobalLayoutAction = () =>
				{
					if (globalLayoutListener.OnGlobalLayout())
						view.ViewTreeObserver.RemoveOnGlobalLayoutListener(l);
				}
			};
		}
	}
}