using System;
using Android.Animation;

namespace DroidKaigi2017.Droid.Views.CustomViews
{
	public class AnonymousAnimatorListener : Java.Lang.Object, Animator.IAnimatorListener
	{
		public Action<Animator> OnAnimationCancelAction { get; set; }
		public void OnAnimationCancel(Animator animation)
		{
			OnAnimationCancelAction?.Invoke(animation);
		}

		public Action<Animator> OnAnimationEndAction { get; set; }
		public void OnAnimationEnd(Animator animation)
		{
			OnAnimationEndAction?.Invoke(animation);
		}

		public Action<Animator> OnAnimationRepeatAction { get; set; }
		public void OnAnimationRepeat(Animator animation)
		{
			OnAnimationRepeatAction?.Invoke(animation);
		}

		public Action<Animator> OnAnimationStartAction { get; set; }
		public void OnAnimationStart(Animator animation)
		{
			OnAnimationStartAction?.Invoke(animation);
		}
	}
}