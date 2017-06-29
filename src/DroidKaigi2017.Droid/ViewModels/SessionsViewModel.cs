#region

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
using DroidKaigi2017.Services;
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
		private readonly IDateUtil _dateUtil;
		private readonly IMySessionService _mySessionService;
		private readonly ISessionService _sessionService;
		private readonly INavigator _navigator;

		public SessionsViewModel(IMySessionService mySessionService,
			IDateUtil dateUtil,  INavigator navigator, ISessionService sessionService)
		{
			_mySessionService = mySessionService;
			_dateUtil = dateUtil;
			_navigator = navigator;
			_sessionService = sessionService;
			BusyNotifier = _sessionService.BusyNotifier.ToReadOnlySwitchReactiveProperty(IsActiveObservable)
				.AddTo(CompositeDisposable);

			SessionsObservable = _sessionService.Sessions
				.Do(x => { StartTimes = x.Select(y => y.SessionModel.StartTime).Distinct().ToList(); })
				.Select(x =>
					x.Select(y =>
							new SessionViewModel(y, _sessionService.RoomCount, _mySessionService, _dateUtil, _navigator))
						.ToList())
				.Do(x => x.ForEach(y => Subscribe(y)))
				.Select(x=> AdjustViewModels(x))
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable, initialValue: new List<SessionViewModel>())
				.AddTo(CompositeDisposable)
				;

			sessionService.Rooms.Subscribe(x =>
			{
				SessionRooms = x;
			});

			GoSearchCommand = _sessionService.BusyNotifier.Inverse().ToReactiveCommand();
			GoSearchCommand.Subscribe(x =>
			{
				_navigator.NavigateTo(NavigationKey.GoSearch);
			});

			LoadCommand = new AsyncReactiveCommand();
			LoadCommand.Subscribe(async x =>
			{
				await _sessionService.LoadAsync();
			});
		}

		public IObservable<List<SessionViewModel>> SessionsObservable { get; }


		public AsyncReactiveCommand LoadCommand { get; }
		public ReactiveCommand GoSearchCommand { get;}
		public RoomModel[] SessionRooms { get; private set; }
		public List<DateTimeOffset> StartTimes { get; private set; }

		public IReadOnlyReactiveProperty<bool> BusyNotifier { get; }

		private List<SessionViewModel> AdjustViewModels(List<SessionViewModel> viewModels)
		{
			var sessionMap = new Dictionary<string, SessionViewModel>();

			foreach (var sessionViewModel in viewModels)
			{
				var roomname = sessionViewModel.RoomName;
				if (string.IsNullOrEmpty(roomname))
					roomname = _sessionService.Rooms.Value.FirstOrDefault().Name;
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
				for (var i = 0; i < _sessionService.Rooms.Value.Length; i++)
				{
					var room = _sessionService.Rooms.Value[i];
					var key = GenerateStimeRoomKey(startTime, room.Name);
					if (sessionMap.ContainsKey(key))
					{
						var vm = sessionMap[key];
						if (vm.FormattedDate != lastFormattedDate)
						{
							lastFormattedDate = vm.FormattedDate;
							adjustedViewModels.Add(SessionViewModel.Create(1, _sessionService.Rooms.Value.Length));
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