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
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Interface.MySession;
using DroidKaigi2017.Interface.Room;
using DroidKaigi2017.Interface.Session;
using DroidKaigi2017.Interface.Speaker;
using DroidKaigi2017.Interface.Topic;
using Java.Net;
using Java.Util;
using Javax.Security.Auth;
using Nyanto;
using Nyanto.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using Observable = System.Reactive.Linq.Observable;

namespace DroidKaigi2017.Droid.ViewModels
{
	public class SessionDetailViewModel : ViewModelBase
	{
		private readonly ISessionService _sessionService;
		private readonly IMySessionService _mySessionService;
		private readonly IRoomService _roomService;
		private readonly ITopicService _topicService;
		private readonly ISpeakerService _speakerService;
		private readonly INavigator _navigator;
		private readonly LocaleUtil _localeUtil;

		public SessionDetailViewModel(ISessionService sessionService, IMySessionService mySessionService, IRoomService roomService, ITopicService topicService, ISpeakerService speakerService, LocaleUtil localeUtil, INavigator navigator)
		{
			_sessionService = sessionService;
			_mySessionService = mySessionService;
			_roomService = roomService;
			_topicService = topicService;
			_speakerService = speakerService;
			_localeUtil = localeUtil;
			_navigator = navigator;

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

			RoomName = dataObserver.Select(x=> x?.room?.Name)
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);

			Topic = dataObserver.Select(x=> x?.topic?.Name)
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);

			SpeakerName = dataObserver.Select(x=> x?.speaker?.Name)
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);

			SessionId = dataObserver.Select(x => x?.session.Id)
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);

			Title = dataObserver.Select(x => x?.session.Title)
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable).AddTo(CompositeDisposable);

			Description = dataObserver.Select(x => x?.session?.Description)
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);

			var topiccolor = dataObserver.Select(x =>
			{
				return x?.topic != null ? TopicColor.GettopiColor(x.topic) : TopicColor.None;
			});

			speakerImageUrl = dataObserver.Select(x => x?.speaker?.ImageUrl)
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);
				;

			sessionVividColorResId = topiccolor.Select(x => x.VividColorResId)
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);

			sessionPaleColorResId = topiccolor.Select(x => x.PaleColorResId)
					.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
					.AddTo(CompositeDisposable)
				;
			sessionThemeResId = topiccolor.Select(x => x.ThemeId)
					.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
					.AddTo(CompositeDisposable)
				;
			languageResId = dataObserver.Select(x => x?.session?.Lang)
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);


			StartTime = dataObserver.Select(x=> x?.session?.StartTime.DateTime.ToJavaDate())
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);


			EndTime = dataObserver.Select(x => x?.session?.EndTime.DateTime.ToJavaDate())
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);


			isMySession = _mySessionService.MySessions.ObserveProperty(x => x.Count)
				.CombineLatest(dataObserver.Select(x => x?.session), (c, s) => s)
				.Select(session =>
				{
					if (session == null)
						return false;

					return _mySessionService.MySessions.Any(y => y.SessionId == session.Id);
				})
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);

			tagContainerVisibility = dataObserver.Select(x => x?.session)
				.Select(x =>
				{
					if (x == null)
						return false;
					return x.Type != SessionType.Dinner;
				})
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);
			speakerVisibility = tagContainerVisibility;

			SelectSessionCommand = new ReactiveCommand<int>();
			SelectSessionCommand.Subscribe(x => _selectedSessionidSubject.OnNext(x)).AddTo(CompositeDisposable);

			FavCommand = new ReactiveCommand();
			FavCommand.Subscribe(x =>
			{
				if (SessionId.Value.HasValue)
				{
					if (isMySession.Value)
					{
						_mySessionService.Remove(SessionId.Value.Value);
					}
					else
					{
						_mySessionService.Add(SessionId.Value.Value);
					}
				}
				
			}).AddTo(CompositeDisposable);

			IsMovieVisibility = dataObserver.Select(x => !string.IsNullOrEmpty(x?.session?.MovieUrl)
			                                             && !string.IsNullOrEmpty(x?.session?.MovieDashUrl))
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);
			MovieCommand = IsMovieVisibility.ToReactiveCommand();
			MovieCommand.Subscribe(x => { });

			IsSlideEnabled = dataObserver.Select(x => !string.IsNullOrEmpty(x?.session?.SlideUrl))
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);
			SlideUrl = dataObserver.Select(x => x?.session?.SlideUrl)
				.ToReadOnlyReactiveProperty()
				.AddTo(CompositeDisposable);

			SlideCommand = IsSlideEnabled.ToReactiveCommand();
			SlideCommand.Subscribe(x =>
			{
				_navigator.OpenUrl(SlideUrl.Value);
			});

			feedbackButtonVisiblity = dataObserver.Select(x => !(x?.session?.Type == SessionType.Dinner))
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);

			FeedBackCommand = new ReactiveCommand();
			FeedBackCommand.Subscribe(x => { }).AddTo(CompositeDisposable);

			CloseCommand = new ReactiveCommand();
			CloseCommand.Subscribe(x =>
			{
				_navigator.Finish();
			});
		}

		public ReactiveCommand<int> SelectSessionCommand { get; }

		private readonly Subject<int> _selectedSessionidSubject = new Subject<int>();

		public BusyNotifier BusyNotifier { get; }

		public IReadOnlyReactiveProperty<int?> SessionId { get; }

		public IReadOnlyReactiveProperty<string> Title { get; }

		public IReadOnlyReactiveProperty<string> Description { get; }

		public IReadOnlyReactiveProperty<string> speakerImageUrl { get; }

		public IReadOnlyReactiveProperty<int> sessionVividColorResId { get; }

		public IReadOnlyReactiveProperty<int> sessionPaleColorResId { get; }

		public IReadOnlyReactiveProperty<int> sessionThemeResId { get; }

		public IReadOnlyReactiveProperty<string> languageResId { get; }

		public IReadOnlyReactiveProperty<Date> StartTime { get; }

		public IReadOnlyReactiveProperty<Date> EndTime { get; }

		public IReadOnlyReactiveProperty<bool> isMySession { get; }

		public IReadOnlyReactiveProperty<bool> tagContainerVisibility { get; }

		public IReadOnlyReactiveProperty<bool> speakerVisibility { get; }

		public IReadOnlyReactiveProperty<string> RoomName { get; }
		public IReadOnlyReactiveProperty<string> Topic { get; }
		public IReadOnlyReactiveProperty<string> SpeakerName { get; }

		public IReadOnlyReactiveProperty<bool> feedbackButtonVisiblity { get; }

		public IReadOnlyReactiveProperty<bool> IsMovieVisibility { get; }

		
		public ReactiveCommand MovieCommand { get; }

		public ReactiveCommand FavCommand { get; }

		public ReactiveCommand FeedBackCommand { get; }

		public ReactiveCommand SlideCommand { get; }
		public ReactiveCommand CloseCommand { get; }
		public IReadOnlyReactiveProperty<bool> IsSlideEnabled { get; }
		public IReadOnlyReactiveProperty<string> SlideUrl { get; }






	}
}