#region

using System;
using System.Linq;
using System.Reactive.Linq;
using Android.Content;
using DroidKaigi2017.Droid.Annotations;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Repository;
using DroidKaigi2017.Interface.Services;
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
		private readonly IMySessionRepository _mySessionRepository;
		private readonly int? _rowSpan;
		private readonly INavigator _navigator;
		private readonly Session _session;

		public SessionViewModel(Context context, Session session, int roomCount, IMySessionRepository mySessionRepository, IDateUtil dateUtil, INavigator navigator)
		{
			_context = context;
			_mySessionRepository = mySessionRepository;
			_dateUtil = dateUtil;
			_navigator = navigator;
			_session = session;

			this.RoomCount = roomCount;
			IsCheckVisible = mySessionRepository.MySessions
				.ObserveProperty(x => x.Count)
				.ToUnit()
				.Select(x => { return mySessionRepository.MySessions.Any(y => y.SessionId == session.Sesion?.Id); })
				.DistinctUntilChanged()
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable, initialValue:  mySessionRepository.MySessions.Any(x => x.SessionId == session.Sesion?.Id))
				.AddTo(CompositeDisposable);

			CheckCommand = new ReactiveCommand<bool>();
			CheckCommand.Subscribe(x =>
			{
				if (x)
					_mySessionRepository.Add(session.Sesion.Id);
				else
					_mySessionRepository.Remove(session.Sesion.Id);
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

		public string ShortStartTime => _session?.Sesion?.StartTime != null
			? _dateUtil.GetHourMinute(_session.Sesion.StartTime.UtcDateTime)
			: "";

		public DateTime StartTime => _session?.Sesion?.StartTime.UtcDateTime ?? DateTime.MinValue;

		public DateTime EndTime => _session?.Sesion?.EndTime.UtcDateTime ?? DateTime.MaxValue;

		public int SessionId => _session?.Sesion?.Id ?? -1;

		public string Title => _session?.Sesion?.Title;
		public string SpeakerName => _session?.Speaker?.Name;
		public string RoomName => _session?.Room?.Name;
		public string LanguageId => _session?.Sesion?.Lang;

		public string Minutes => _context?.GetString(Resource.String.session_minutes, _session?.Sesion?.DurationMin);

		public int TitleMaxLines => IsLongBreak ? 6 : 3;

		public int SpeakerNameMaxLines => IsLongBreak ? 3 : 1;

		public int BackgroundResourceId
		{
			get
			{
				if (IsSelectable)
				{
					var now = DateTimeOffset.Now;
					if (_session?.Sesion?.StartTime <= now && _session?.Sesion?.EndTime >= now)
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
				if (_session?.Topic != null && IsSelectable)
					return TopicColor.GettopiColor(_session.Topic).MiddleColorResId;
				return Android.Resource.Color.Transparent;
			}
		}


		public bool IsSelectable => _session?.Sesion != null && _session.Sesion.Type != SessionType.Break;

		public ReactiveCommand GoDetailCommand { get; }
		public ReactiveCommand<bool> CheckCommand { get; }
		public IReadOnlyReactiveProperty<bool> IsCheckVisible { get; }

		public bool IsNormalSession => _session?.Sesion != null && _session?.Sesion?.Type != SessionType.Break &&
		                               _session?.Sesion?.Type != SessionType.Dinner;

		public bool IsLanguageVisible => !string.IsNullOrEmpty(_session?.Sesion?.Lang);

		public int RoomCount { get; }

		public string FormattedDate => _session?.Sesion?.StartTime != null
			? _dateUtil.GetMonthDate(_session.Sesion.StartTime.UtcDateTime)
			: "";

		private bool IsLongBreak => _session?.Sesion?.DurationMin > 30 && _session?.Sesion?.Type != SessionType.Break;

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

				switch (_session?.Sesion.Type)
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