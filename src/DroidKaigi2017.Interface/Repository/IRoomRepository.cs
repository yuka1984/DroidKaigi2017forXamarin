#region

using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Interface.Repository
{
	public interface IRoomRepository
	{
		ReadOnlyReactiveProperty<RoomModel[]> RoomsObservable { get; }

		Task LoadAsync();
	}
}