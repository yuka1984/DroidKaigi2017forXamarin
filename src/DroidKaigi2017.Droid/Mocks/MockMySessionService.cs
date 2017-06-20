#region

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.MySession;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Droid.Mocks
{
	public class MockMySessionService : IMySessionService
	{
		private readonly ObservableCollection<MySessionModel> _mySession;

		public MockMySessionService()
		{
			_mySession = new ObservableCollection<MySessionModel>();
			MySessions = _mySession.ToReadOnlyReactiveCollection(TaskPoolScheduler.Default);
		}

		public ReadOnlyObservableCollection<MySessionModel> MySessions { get; }

		public async Task LoadAsync()
		{
			await Task.Delay(5000).ConfigureAwait(false);
		}

		public async Task Add(int sessionId)
		{
			await Task.Delay(100).ConfigureAwait(false);
			_mySession.Add(new MySessionModel
			{
				Id = Guid.NewGuid().ToString(),
				SessionId = sessionId
			});
		}

		public async Task Remove(int sessionId)
		{
			await Task.Delay(100).ConfigureAwait(false);
			if (_mySession.Any(x => x.SessionId == sessionId))
				_mySession.Remove(_mySession.First(x => x.SessionId == sessionId));
		}
	}
}