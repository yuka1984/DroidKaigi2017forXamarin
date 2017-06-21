using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DroidKaigi2017.Interface.MySession;
using DroidKaigi2017.Interface.Room;
using DroidKaigi2017.Interface.Session;
using DroidKaigi2017.Interface.Speaker;
using DroidKaigi2017.Interface.Topic;
using Javax.Security.Auth;
using Nyanto;
using Nyanto.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace DroidKaigi2017.Droid.ViewModels
{
	public class SessionDetailViewModel : ViewModelBase
	{
		private readonly ISessionService _sessionService;
		private readonly IMySessionService _mySessionService;
		private readonly IRoomService _roomService;
		private readonly ITopicService _topicService;
		private readonly ISpeakerService _speakerService;

		public SessionDetailViewModel(ISessionService sessionService, IMySessionService mySessionService, IRoomService roomService, ITopicService topicService, ISpeakerService speakerService)
		{
			_sessionService = sessionService;
			_mySessionService = mySessionService;
			_roomService = roomService;
			_topicService = topicService;
			_speakerService = speakerService;

			BusyNotifier = new BusyNotifier();
			var busyDispose = BusyNotifier.ProcessStart();

			 var dataObserver = Observable.CombineLatest(
				_selectedSessionidSubject,
				_sessionService.SessionsObservable,
				_roomService.RoomsObservable,
				_topicService.TopicsObservable,
				_speakerService.SpealersObservable,
				(id, sessions, rooms, topics, speakers) => new {id, sessions, rooms, topics, speakers}
			)
			.Do(x=> busyDispose.Dispose())
			.Select(x =>
				{
					var session = x.sessions.FirstOrDefault(y => y.Id == x.id);
					if (session == null)
						return null;
					var room = x.rooms.FirstOrDefault(y => y.Id == session.RoomId);
					var topic = x.topics.FirstOrDefault(y => y.Id == session.TopicId);
					var speaker = x.speakers.FirstOrDefault(y => y.Id == session.SpeakerId);
					return new
					{
						session,
						room,
						topic,
						speaker
					};
				})
			.ToReadOnlyReactiveProperty(eventScheduler:TaskPoolScheduler.Default)
			.AddTo(CompositeDisposable)			
			;

			Title = dataObserver.Select(x => x?.session.Title)
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);

			speakerImageUrl = dataObserver.Select(x => x?.speaker?.ImageUrl)
					.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
					.AddTo(CompositeDisposable)
				;



			SelectSessionCommand = new ReactiveCommand<int>();
			SelectSessionCommand.Subscribe(x => _selectedSessionidSubject.OnNext(x)).AddTo(CompositeDisposable);

		}

		public ReactiveCommand<int> SelectSessionCommand { get; }

		private readonly Subject<int> _selectedSessionidSubject = new Subject<int>();

		public BusyNotifier BusyNotifier { get; }

		public IReadOnlyReactiveProperty<string> Title { get; }

		public IReadOnlyReactiveProperty<string> speakerImageUrl { get; }

		public IReadOnlyReactiveProperty<int> sessionVividColorResId { get; }

		public IReadOnlyReactiveProperty<int> sessionPaleColorResId { get; }

		public IReadOnlyReactiveProperty<int> sessionThemeResId { get; }

		public IReadOnlyReactiveProperty<string> languageResId { get; }

		public IReadOnlyReactiveProperty<string> sessionTimeRange { get; }

		public IReadOnlyReactiveProperty<bool> isMySession { get; }

		public IReadOnlyReactiveProperty<bool> tagContainerVisibility { get; }

		public IReadOnlyReactiveProperty<bool> speakerVisibility { get; }

		public IReadOnlyReactiveProperty<bool> slideIconVisibility { get; }
		public IReadOnlyReactiveProperty<bool> dashVideoIconVisibility { get; }
		public IReadOnlyReactiveProperty<bool> roomVisibility { get; }
		public IReadOnlyReactiveProperty<bool> topicVisibility { get; }
		public IReadOnlyReactiveProperty<bool> feedbackButtonVisiblity { get; }
		



	}
}