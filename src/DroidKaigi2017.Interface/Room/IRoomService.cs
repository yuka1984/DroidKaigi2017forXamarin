#region

using System.Threading.Tasks;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Interface.Room
{
	public interface IRoomService
	{
		ReadOnlyReactiveProperty<RoomModel[]> RoomsObservable { get; }

		Task LoadAsync();
	}
}