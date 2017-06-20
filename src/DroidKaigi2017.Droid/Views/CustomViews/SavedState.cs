#region

using Android.OS;
using Java.Lang;

#endregion

namespace DroidKaigi2017.Droid.Views.CustomViews
{
	public class SavedState : Object, IParcelable
	{
		public static SavedState EMPTY_STATE = new SavedState();

		private readonly IParcelable _superState;
		public int anchorItemPosition;
		public Bundle itemSelectionState;


		public SavedState()
		{
		}

		public SavedState(IParcelable superState)
		{
			if (superState == null)
				throw new IllegalArgumentException("_superState must not be null");

			_superState = superState != EMPTY_STATE ? superState : null;
		}

		public SavedState(Parcel In)
		{
			_superState = EMPTY_STATE;
			anchorItemPosition = In.ReadInt();
			itemSelectionState = (Bundle) In.ReadParcelable(Class.ClassLoader);
		}


		public int DescribeContents()
		{
			return 0;
		}

		public void WriteToParcel(Parcel @out, ParcelableWriteFlags flags)
		{
			@out.WriteInt(anchorItemPosition);
			@out.WriteParcelable(itemSelectionState, flags);
		}

		public IParcelable getSuperState()
		{
			return _superState;
		}
	}
}