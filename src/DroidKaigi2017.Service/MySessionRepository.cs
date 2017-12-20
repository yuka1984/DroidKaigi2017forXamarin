using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Repository;
using DroidKaigi2017.Interface.Tools;
using Microsoft.WindowsAzure.MobileServices;
using Reactive.Bindings;

namespace DroidKaigi2017.Service
{
    public class MySessionRepository : IMySessionRepository
    {
	    private readonly IKeyValueStore _keyValueStore;

	    public MySessionRepository(IKeyValueStore keyValueStore)
	    {
		    _keyValueStore = keyValueStore;
		    MySessions = _mySessionModels.ToReadOnlyReactiveCollection();
	    }

	    private ObservableCollection<MySessionModel> _mySessionModels = new ObservableCollection<MySessionModel>();
	    private const string Key = "MySessionRepository";
	    public ReadOnlyObservableCollection<MySessionModel> MySessions { get; }
	    public Task LoadAsync()
	    {
		    return Task.Run(() =>
		    {
				var list = _keyValueStore.GetValue<List<MySessionModel>>(key: Key);
			    if (list != null)
			    {
				    _mySessionModels.Clear();
				    list.ForEach(x => _mySessionModels.Add(x));
			    }
			});
		    
	    }

	    public Task Add(int sessionId)
	    {
		    return Task.Run(() =>
		    {
				if(_mySessionModels.Any(x=> x?.SessionId == sessionId))
					return;

				_mySessionModels.Add(new MySessionModel
				{
					Id = Guid.NewGuid().ToString(),
					SessionId = sessionId
				});
			    _keyValueStore.CreateNew(Key, _mySessionModels.ToList());
		    });
		    
	    }

	    public Task Remove(int sessionId)
	    {
			return Task.Run(() =>
			{
				var session = _mySessionModels.FirstOrDefault(x => x?.SessionId == sessionId);
				if (session != null)
				{
					_mySessionModels.Remove(session);
					_keyValueStore.CreateNew(Key, _mySessionModels.ToList());
				}				
			});
		}
    }
}
