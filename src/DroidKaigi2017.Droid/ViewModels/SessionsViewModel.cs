﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.Content;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Repository;
using Nyanto;
using Nyanto.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

#endregion

namespace DroidKaigi2017.Droid.ViewModels
{
	public class SessionsViewModel : ViewModelBase
	{
		private readonly Context _context;
		private readonly IDateUtil _dateUtil;
		private readonly IMySessionRepository _mySessionRepository;
		private readonly IRoomRepository _roomRepository;
		private readonly ISessionRepository _sessionRepository;
		private readonly ISpeakerRepository _speakerRepository;
		private readonly ITopicRepository _topicRepository;
		private readonly INavigator _navigator;

		public readonly BusyNotifier BusyNotifier = new BusyNotifier();

		public SessionsViewModel(ISessionRepository sessionRepository, IRoomRepository roomRepository, IMySessionRepository mySessionRepository,
			IDateUtil dateUtil, Context context, ISpeakerRepository speakerRepository, ITopicRepository topicRepository, INavigator navigator)
		{
			_sessionRepository = sessionRepository;
			_roomRepository = roomRepository;
			_mySessionRepository = mySessionRepository;
			_dateUtil = dateUtil;
			_context = context;
			_speakerRepository = speakerRepository;
			_topicRepository = topicRepository;
			_navigator = navigator;

			var dispose = BusyNotifier.ProcessStart();
			var loadingObservable = Observable.FromAsync(() =>
				Task
					.WhenAll(
						_sessionRepository.LoadAsync()
						, _roomRepository.LoadAsync()
						, _speakerRepository.LoadAsync()
						, _topicRepository.LoadAsync())
					.ContinueWith(task => dispose.Dispose()));

			SessionsObservable = loadingObservable.Select(x => new List<SessionViewModel>())
					.Concat(
						_sessionRepository.SessionsObservable
							.Do(x => { StartTimes = x.Select(y => y.StartTime).Distinct().ToList(); })
							.CombineLatest(
								_roomRepository.RoomsObservable.Do(x => SessionRooms = x.ToList()),
								_speakerRepository.SpealersObservable,
								_topicRepository.TopicsObservable, (session, room, speaker, topic) => new {session, room, speaker, topic})
							.Select(x =>
								x.session.Select(y =>
										new SessionViewModel(_context, y, _mySessionRepository, _roomRepository, _speakerRepository, _topicRepository, _dateUtil,
											_navigator))
									.ToList())
							.Do(x =>
							{
								x.ForEach(y => this.Subscribe(y));
							})
							.Select(x => AdjustViewModels(x)))
					.ToReadOnlySwitchReactiveProperty(switchSource: base.IsActiveObservable,
						initialValue: new List<SessionViewModel>(), eventScheduler: TaskPoolScheduler.Default)
					.AddTo(CompositeDisposable)
				;

			GoSearchCommand = BusyNotifier.Inverse().ToReactiveCommand();
			GoSearchCommand.Subscribe(x =>
			{
				_navigator.NavigateTo(NavigationKey.GoSearch);
			});
		}

		public IObservable<List<SessionViewModel>> SessionsObservable { get; }

		public ReactiveCommand GoSearchCommand { get;}
		public List<RoomModel> SessionRooms { get; private set; }
		public List<DateTimeOffset> StartTimes { get; private set; }

		private List<SessionViewModel> AdjustViewModels(List<SessionViewModel> viewModels)
		{
			var sessionMap = new Dictionary<string, SessionViewModel>();

			foreach (var sessionViewModel in viewModels)
			{
				var roomname = sessionViewModel.RoomName;
				if (string.IsNullOrEmpty(roomname))
					roomname = _roomRepository.RoomsObservable.Value?.FirstOrDefault()?.Name;
				var key = GenerateStimeRoomKey(sessionViewModel.StartTime, roomname);
				if (!sessionMap.ContainsKey(key))
					sessionMap.Add(key, sessionViewModel);
			}

			var adjustedViewModels = new List<SessionViewModel>();

			string lastFormattedDate = null;
			foreach (var startTime in StartTimes)
			{
				if (lastFormattedDate == null)
					lastFormattedDate = _dateUtil.GetMonthDate(startTime.UtcDateTime);
				var sameTimes = new List<SessionViewModel>();
				var maxRowSpan = 1;
				for (var i = 0; i < SessionRooms.Count; i++)
				{
					var room = SessionRooms[i];
					var key = GenerateStimeRoomKey(startTime, room.Name);
					if (sessionMap.ContainsKey(key))
					{
						var vm = sessionMap[key];
						if (vm.FormattedDate != lastFormattedDate)
						{
							lastFormattedDate = vm.FormattedDate;
							adjustedViewModels.Add(SessionViewModel.Create(1, SessionRooms.Count));
						}
						sameTimes.Add(vm);

						if (vm.RowSpan > maxRowSpan)
							maxRowSpan = vm.RowSpan;

						for (int j = 1, colSize = vm.ColSpan; j < colSize; j++)
							i++;
					}
					else
					{
						var vm = SessionViewModel.Create();
						sameTimes.Add(vm);
					}
				}
				var copiedTmp = new List<SessionViewModel>(sameTimes);
				foreach (var viewModel in sameTimes)
					if (viewModel.RowSpan < maxRowSpan)
						copiedTmp.Add(SessionViewModel.Create(maxRowSpan - viewModel.RowSpan));
				adjustedViewModels.AddRange(copiedTmp);
			}

			return adjustedViewModels;
		}


		private string GenerateStimeRoomKey(DateTimeOffset stime, string roomName)
		{
			return _dateUtil.GetLongFormatDate(stime.UtcDateTime) + "_" + roomName;
		}
	}
}