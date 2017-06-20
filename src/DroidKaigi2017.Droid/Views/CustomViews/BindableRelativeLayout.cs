#region

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;

#endregion

namespace DroidKaigi2017.Droid.Views.CustomViews
{
	public abstract class BindableRelativeLayout : RelativeLayout, INotifyPropertyChanged
	{
		public BindableRelativeLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public BindableRelativeLayout(Context context) : base(context)
		{
		}

		public BindableRelativeLayout(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public BindableRelativeLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs,
			defStyleAttr)
		{
		}

		public BindableRelativeLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context,
			attrs, defStyleAttr, defStyleRes)
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(storage, value)) return false;

			storage = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		protected bool SetProperty<TType, TProperty>(TType targetClass, Expression<Func<TType, TProperty>> selector,
			TProperty value, [CallerMemberName] string propertyName = null)
		{
			var oldValue = AccessorCache<TType>.LookupGet(selector).Invoke(targetClass);

			if (Equals(oldValue, value)) return false;

			AccessorCache<TType>.LookupSet(selector).Invoke(targetClass, value);
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}