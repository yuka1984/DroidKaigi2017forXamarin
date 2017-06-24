#region

using System;
using System.Linq;
using System.Reactive.Linq;
using Android.Content;
using DroidKaigi2017.Droid.Annotations;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Interface.MySession;
using DroidKaigi2017.Interface.Room;
using DroidKaigi2017.Interface.Session;
using DroidKaigi2017.Interface.Speaker;
using DroidKaigi2017.Interface.Topic;
using Nyanto;
using Nyanto.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

#endregion

namespace DroidKaigi2017.Droid.ViewModels
{
	public class SessionViewModel : ViewModelBase
	{
		private readonly int? _colSpan;
		private readonly Context _context;
		private readonly IDateUtil _dateUtil;
		private readonly IMySessionService _mySessionService;
		private readonly RoomModel _roomModel;
		private readonly IRoomService _roomService;

		private readonly int? _rowSpan;
		private readonly SessionModel _sessionModel;
		private readonly SpeakerModel _speakerModel;
		private readonly ISpeakerService _speakerService;
		private readonly Topic _topic;
		private readonly ITopicService _topicService;
		private readonly INavigator _navigator;

		public SessionViewModel(Context context, SessionModel sessionModel, IMySessionService mySessionService,
			IRoomService roomService,
			ISpeakerService speakerService, ITopicService topicService, IDateUtil dateUtil, INavigator navigator)
		{
			_context = context;
			_mySessionService = mySessionService;
			_roomService = roomService;
			_speakerService = speakerService;
			_topicService = topicService;
			_sessionModel = sessionModel;
			_dateUtil = dateUtil;
			_navigator = navigator;

			_speakerModel = _speakerService.SpealersObservable.Value?.FirstOrDefault(x => x.Id == sessionModel.SpeakerId);
			_roomModel = _roomService.RoomsObservable.Value?.FirstOrDefault(x => x.Id == sessionModel.RoomId);
			_topic = _topicService.TopicsObservable.Value?.FirstOrDefault(x => x.Id == _sessionModel?.Id);

			RoomCount = roomService.RoomsObservable?.Value.Length ?? 0;
			IsCheckVisible = mySessionService.MySessions
				.ObserveProperty(x => x.Count)
				.ToUnit()
				.Select(x => { return mySessionService.MySessions.Any(y => y.SessionId == sessionModel.Id); })
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable, initialValue:  mySessionService.MySessions.Any(x => x.SessionId == sessionModel.Id))
				.AddTo(CompositeDisposable);

			CheckCommand = new ReactiveCommand<bool>();
			CheckCommand.Subscribe(x =>
			{
				if (x)
					_mySessionService.Add(sessionModel.Id);
				else
					_mySessionService.Remove(sessionModel.Id);
			});

			GoDetailCommand = new ReactiveCommand();
			GoDetailCommand.Subscribe(x => { _navigator.NavigateTo(NavigationKey.GoSessionDetail, this); });
		}

		private SessionViewModel(int rowSpan = 1, int colSpan = 1)
		{
			_colSpan = colSpan;
			_rowSpan = rowSpan;
			IsCheckVisible = new ReactiveProperty<bool>(false).ToReadOnlyReactiveProperty();
		}

		public string ShortStartTime => _sessionModel?.StartTime != null
			? _dateUtil.GetHourMinute(_sessionModel.StartTime.UtcDateTime)
			: "";

		public DateTime StartTime => _sessionModel?.StartTime.UtcDateTime ?? DateTime.MinValue;

		public DateTime EndTime => _sessionModel?.EndTime.UtcDateTime ?? DateTime.MaxValue;

		public int SessionId => _sessionModel?.Id ?? -1;

		public string Title => _sessionModel?.Title;
		public string SpeakerName => _speakerModel?.Name;
		public string RoomName => _roomModel?.Name;
		public string LanguageId => _sessionModel?.Lang;

		public string Minutes => _context?.GetString(Resource.String.session_minutes, _sessionModel?.DurationMin);

		public int TitleMaxLines => IsLongBreak ? 6 : 3;

		public int SpeakerNameMaxLines => IsLongBreak ? 3 : 1;

		public int BackgroundResourceId
		{
			get
			{
				if (IsSelectable)
				{
					var now = DateTimeOffset.Now;
					if (_sessionModel?.StartTime <= now && _sessionModel?.EndTime >= now)
						return Resource.Drawable.clickable_purple;
					return Resource.Drawable.clickable_white;
				}
				return Resource.Drawable.bg_empty_session;
			}
		}


		public int TopicColorResourceId
		{
			get
			{
				if (_topic != null && IsSelectable)
					return TopicColor.GettopiColor(_topic).MiddleColorResId;
				return Android.Resource.Color.Transparent;
			}
		}


		public bool IsSelectable => _sessionModel != null && _sessionModel.Type != SessionType.Break;

		public ReactiveCommand GoDetailCommand { get; }
		public ReactiveCommand<bool> CheckCommand { get; }
		public IReadOnlyReactiveProperty<bool> IsCheckVisible { get; }

		public bool IsNormalSession => _sessionModel != null && _sessionModel?.Type != SessionType.Break &&
		                               _sessionModel?.Type != SessionType.Dinner;

		public bool IsLanguageVisible => !string.IsNullOrEmpty(_sessionModel?.Lang);

		public int RoomCount { get; }

		public string FormattedDate => _sessionModel?.StartTime != null
			? _dateUtil.GetMonthDate(_sessionModel.StartTime.UtcDateTime)
			: "";

		private bool IsLongBreak => _sessionModel?.DurationMin > 30 && _sessionModel?.Type != SessionType.Break;

		public int RowSpan
		{
			get
			{
				if (_rowSpan.HasValue)
					return _rowSpan.Value;

				return IsLongBreak ? 2 : 1;
			}
		}

		public int ColSpan
		{
			get
			{
				if (_colSpan.HasValue)
					return _colSpan.Value;

				switch (_sessionModel.Type)
				{
					case SessionType.Ceremony:
						return 3;
					case SessionType.Break:
					case SessionType.Dinner:
						return RoomCount;
					default:
						return 1;
				}
			}
		}

		public static SessionViewModel Create(int rowSpan = 1, int colSpan = 1)
		{
			return new SessionViewModel(rowSpan, colSpan);
		}
	}
}