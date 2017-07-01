using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Repository;
using DroidKaigi2017.Interface.Services;
using Reactive.Bindings;

namespace DroidKaigi2017.Services
{
	public class MySessionService : IMySessionService
	{
	    private readonly IMySessionRepository _mySessionRepository;
	    public MySessionService(IMySessionRepository mySessionRepository)
	    {
		    _mySessionRepository = mySessionRepository;
		    MySessions = _mySessionRepository.MySessions.ToReadOnlyReactiveCollection(x => x.SessionId);
	    }

		public ReadOnlyObservableCollection<int> MySessions { get; }

	    public Task AddAsync(int sessionId)
	    {
		    return _mySessionRepository.Add(sessionId);
	    }

	    public Task RemoveAsync(int sessionid)
	    {
		    return _mySessionRepository.Remove(sessionid);
	    }

		public Task LoadAsync()
		{
			return _mySessionRepository.LoadAsync();
		}
	}


}
