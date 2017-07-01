using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;
using Reactive.Bindings;
using Reactive.Bindings.Notifiers;

namespace DroidKaigi2017.Interface.Services
{
	public interface ISessionService
	{
		BusyNotifier BusyNotifier { get; }
		IReadOnlyReactiveProperty<Session[]> Sessions { get; }
		IReadOnlyReactiveProperty<RoomModel[]> Rooms { get; }

		int RoomCount { get; }
		Task LoadAsync();
	}
}