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
using DroidKaigi2017.Services;
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
		private readonly IDateUtil _dateUtil;
		private readonly IMySessionService _mySessionService;
		private readonly int? _rowSpan;
		private readonly INavigator _navigator;
		private readonly Session _session;

		public SessionViewModel(Session session, int roomCount, IMySessionService mySessionService, IDateUtil dateUtil, INavigator navigator)
		{
			_mySessionService = mySessionService;
			_dateUtil = dateUtil;
			_navigator = navigator;
			_session = session;

			this.RoomCount = roomCount;
			IsCheckVisible = mySessionService.MySessions
				.ObserveProperty(x => x.Count)
				.ToUnit()
				.Select(x => { return mySessionService.MySessions.Any(y => y == session.SessionModel?.Id); })
				.DistinctUntilChanged()
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable, initialValue:  mySessionService.MySessions.Any(x => x == session.SessionModel?.Id))
				.AddTo(CompositeDisposable);

			CheckCommand = new ReactiveCommand<bool>();
			CheckCommand.Subscribe(x =>
			{
				if (x)
					_mySessionService.AddAsync(session.SessionModel.Id);
				else
					_mySessionService.RemoveAsync(session.SessionModel.Id);
			});

			GoDetailCommand = new ReactiveCommand();
			GoDetailCommand.Subscribe(x => { _navigator.NavigateTo(NavigationKey.GoSessionDetail, this.SessionId); });
		}

		private SessionViewModel(int rowSpan = 1, int colSpan = 1)
		{
			_colSpan = colSpan;
			_rowSpan = rowSpan;
			IsCheckVisible = new ReactiveProperty<bool>(false).ToReadOnlyReactiveProperty();
		}

		public string ShortStartTime => _session?.SessionModel?.StartTime != null
			? _dateUtil.GetHourMinute(_session.SessionModel.StartTime.UtcDateTime)
			: "";

		public DateTime StartTime => _session?.SessionModel?.StartTime.UtcDateTime ?? DateTime.MinValue;

		public DateTime EndTime => _session?.SessionModel?.EndTime.UtcDateTime ?? DateTime.MaxValue;

		public int SessionId => _session?.SessionModel?.Id ?? -1;

		public string Title => _session?.SessionModel?.Title;
		public string SpeakerName => _session?.SpeakerModel?.Name;
		public string RoomName => _session?.RoomModel?.Name;
		public string LanguageId => _session?.SessionModel?.Lang;

		public int? Minutes => _session?.SessionModel?.DurationMin;

		public int TitleMaxLines => IsLongBreak ? 6 : 3;

		public int SpeakerNameMaxLines => IsLongBreak ? 3 : 1;

		public int BackgroundResourceId
		{
			get
			{
				if (IsSelectable)
				{
					var now = DateTimeOffset.Now;
					if (_session?.SessionModel?.StartTime <= now && _session?.SessionModel?.EndTime >= now)
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
				if (_session?.TopicModel != null && IsSelectable)
					return TopicColor.GettopiColor(_session.TopicModel).MiddleColorResId;
				return Android.Resource.Color.Transparent;
			}
		}


		public bool IsSelectable => _session?.SessionModel != null && _session.SessionModel.Type != SessionType.Break;

		public ReactiveCommand GoDetailCommand { get; }
		public ReactiveCommand<bool> CheckCommand { get; }
		public IReadOnlyReactiveProperty<bool> IsCheckVisible { get; }

		public bool IsNormalSession => _session?.SessionModel != null && _session?.SessionModel?.Type != SessionType.Break &&
		                               _session?.SessionModel?.Type != SessionType.Dinner;

		public bool IsLanguageVisible => !string.IsNullOrEmpty(_session?.SessionModel?.Lang);

		public int RoomCount { get; }

		public string FormattedDate => _session?.SessionModel?.StartTime != null
			? _dateUtil.GetMonthDate(_session.SessionModel.StartTime.UtcDateTime)
			: "";

		private bool IsLongBreak => _session?.SessionModel?.DurationMin > 30 && _session?.SessionModel?.Type != SessionType.Break;

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

				switch (_session?.SessionModel.Type)
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