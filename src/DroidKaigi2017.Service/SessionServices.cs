#region

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Repository;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

#endregion

namespace DroidKaigi2017.Services
{
	public interface ISessionService
	{
		BusyNotifier BusyNotifier { get; }
		IReadOnlyReactiveProperty<Session[]> Sessions { get; }
		IReadOnlyReactiveProperty<RoomModel[]> Rooms { get; }

		int RoomCount { get; }
		Task LoadAsync();
	}

	public class SessionService : ISessionService
	{
		private readonly IRoomRepository _roomRepository;
		private readonly ISessionRepository _sessionRepository;
		private readonly ISpeakerRepository _speakerRepository;
		private readonly ITopicRepository _topicRepository;
		private IDisposable _busyDisposable;

		public SessionService(ISessionRepository sessionRepository, ISpeakerRepository speakerRepository,
			IRoomRepository roomRepository, ITopicRepository topicRepository)
		{
			_sessionRepository = sessionRepository;
			_speakerRepository = speakerRepository;
			_roomRepository = roomRepository;
			_topicRepository = topicRepository;
			BusyNotifier = new BusyNotifier();
			Rooms = _roomRepository.RoomsObservable;

			Sessions = BusyNotifier.CombineLatest(_sessionRepository.SessionsObservable
						, _speakerRepository.SpealersObservable
						, _roomRepository.RoomsObservable
						, _topicRepository.TopicsObservable,
						(busy, session, speaker, room, topic) => new {busy, session, speaker, room, topic})
					.Where(x => x.busy == false)
					.Pairwise()
					.Where(x => x.OldItem.session != x.NewItem.session
					            || x.OldItem.speaker != x.NewItem.speaker
					            || x.OldItem.room != x.NewItem.room
					            || x.OldItem.topic != x.NewItem.topic)
					.Select(x => x.NewItem)
					.Select(x =>
					{
						return x.session.Select(y => new Session(
								y,
								x.speaker.FirstOrDefault(z => z.Id == y.SpeakerId),
								x.room.FirstOrDefault(z => z.Id == y.RoomId),
								x.topic.FirstOrDefault(z => z.Id == y.TopicId)
							))
							.ToArray();
					})
					.ToReadOnlyReactiveProperty(Array.Empty<Session>())
				;
		}

		public BusyNotifier BusyNotifier { get; }
		public IReadOnlyReactiveProperty<Session[]> Sessions { get; }

		public IReadOnlyReactiveProperty<RoomModel[]> Rooms { get; }

		public int RoomCount { get; private set; }

		public async Task LoadAsync()
		{
			if (_busyDisposable == null)
				_busyDisposable = BusyNotifier.ProcessStart();
			try
			{
				await Task.WhenAll(_sessionRepository.LoadAsync(),
					_speakerRepository.LoadAsync(),
					_roomRepository.LoadAsync(),
					_topicRepository.LoadAsync());

				RoomCount = _roomRepository.RoomsObservable.Value.Length;
			}
			finally
			{
				_busyDisposable.Dispose();
				_busyDisposable = null;
			}
		}
	}
}