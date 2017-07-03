using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Repository;
using DroidKaigi2017.Interface.Tools;
using Microsoft.WindowsAzure.MobileServices;
using Reactive.Bindings;

namespace DroidKaigi2017.Service
{
    class AzureEasyTableRoomRepository:IRoomRepository
    {
	    private class RoomItem
	    {
		    public int RoomId { get; set; }
		    public string Name { get; set; }

		    public string HashTag { get; set; }
	    }

		private readonly IKeyValueStore _keyValueStore;
	    private bool _isDirty = true;
	    private readonly MobileServiceClient _client;
		private readonly ReactiveProperty<RoomModel[]> _roomProperty = new ReactiveProperty<RoomModel[]>(raiseEventScheduler: TaskPoolScheduler.Default);

		public AzureEasyTableRoomRepository(IKeyValueStore keyValueStore, MobileServiceClient client)
	    {
		    _keyValueStore = keyValueStore;
		    _client = client;
		    RoomsObservable = _roomProperty.ToReadOnlyReactiveProperty(eventScheduler: TaskPoolScheduler.Default);
	    }

	    
		public ReadOnlyReactiveProperty<RoomModel[]> RoomsObservable { get; }
	    public async Task LoadAsync()
	    {
			if (_isDirty)
			{
				try
				{
					var table = _client.GetTable("rooms");
					var list = (await table.ReadAsync("")).ToObject<List<RoomItem>>();
					var rooms = list
						.Select(x => new RoomModel()
						{
							Id = x.RoomId,
							Name = x.Name,
						}).ToArray();

					_roomProperty.Value = rooms;

					_isDirty = false;

					_keyValueStore.CreateNew("Rooms", rooms);
				}
				catch (Exception e)
				{
					System.Diagnostics.Trace.TraceWarning(e.ToString());
					try
					{
						var savedata = _keyValueStore.GetValue<RoomModel[]>("Rooms");
						_roomProperty.Value = savedata;
					}
					catch (Exception exception)
					{
						Console.WriteLine(exception);
					}
				}
			}
		}
    }
}
