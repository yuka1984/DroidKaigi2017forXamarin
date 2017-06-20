#region

using System;
using Android.OS;

#endregion

namespace TwoWayView.Layout
{
	public class AnonymousIParcelableCreator<T>
	{
		private readonly Func<int, T[]> _arrayCreator;
		private readonly Func<Parcel, T> _creator;

		public AnonymousIParcelableCreator(Func<Parcel, T> creator, Func<int, T[]> arrayCreator = null)
		{
			_creator = creator;
			_arrayCreator = arrayCreator;
			if (_arrayCreator == null)
				_arrayCreator = size => new T[size];
		}

		public T CreateFromParcel(Parcel source)
		{
			return _creator(source);
		}

		public T[] NewArray(int size)
		{
			return _arrayCreator(size);
		}
	}
}