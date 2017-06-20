#region

using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Room;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Droid.Mocks
{
	public class MockRoomService : IRoomService
	{
		private readonly ReactiveProperty<RoomModel[]> _roomProperty;

		private bool IsDirty = true;

		public MockRoomService()
		{
			_roomProperty = new ReactiveProperty<RoomModel[]>(initialValue: Array.Empty<RoomModel>(),
				raiseEventScheduler: TaskPoolScheduler.Default);
			RoomsObservable = _roomProperty.ToReadOnlyReactiveProperty(eventScheduler: TaskPoolScheduler.Default);
		}

		public ReadOnlyReactiveProperty<RoomModel[]> RoomsObservable { get; }

		public async Task LoadAsync()
		{
			if (!IsDirty)
				return;
			await Task.Delay(5000).ConfigureAwait(false);
			_roomProperty.Value = new[]
			{
				new RoomModel
				{
					Id = 1,
					IsSession = true,
					Name = "もっさり"
				},
				new RoomModel
				{
					Id = 2,
					IsSession = true,
					Name = "かっちり"
				},
				new RoomModel
				{
					Id = 3,
					IsSession = true,
					Name = "どっきり"
				},
				new RoomModel
				{
					Id = 4,
					IsSession = true,
					Name = "ちょっぴり"
				},
				new RoomModel
				{
					Id = 5,
					IsSession = true,
					Name = "ばったり"
				},
				new RoomModel
				{
					Id = 6,
					IsSession = true,
					Name = "しっかり"
				}
			};
			IsDirty = false;
		}
	}
}