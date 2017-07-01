using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Interface.Services;
using Java.Util;
using Nyanto;
using Reactive.Bindings;

namespace DroidKaigi2017.Droid.ViewModels
{
	public class MySessionsViewModel : ViewModelBase
	{
		private readonly IMySessionService _mySessionService;
		private readonly INavigator _navigator;
		private readonly ISessionService _sessionService;

		public MySessionsViewModel(IMySessionService mySessionService, INavigator navigator, ISessionService sessionService)
		{
			_mySessionService = mySessionService;
			_navigator = navigator;
			_sessionService = sessionService;
			MySessions = _mySessionService.MySessions.ToReadOnlyReactiveCollection(x => new MySessionViewModel(x, _sessionService, _navigator));
		}

		public ReadOnlyObservableCollection<MySessionViewModel> MySessions { get; }
	}

	public class MySessionViewModel
	{
		private readonly ISessionService _sessionService;
		private readonly INavigator _navigator;
		private readonly Session _session;

		public MySessionViewModel(int sessionId, ISessionService sessionService, INavigator navigator)
		{
			_sessionService = sessionService;
			_navigator = navigator;
			_session = _sessionService.Sessions.Value.FirstOrDefault(x => x.SessionModel.Id == sessionId);
			GoDetailCommand = new ReactiveCommand();
			GoDetailCommand.Subscribe(x => { _navigator.NavigateTo(NavigationKey.GoSessionDetail, sessionId); });
		}

		public string SessionTitle => _session?.SessionModel.Title;

		public string RoomName => _session?.RoomModel?.Name;

		public string SpeakerImageUrl => _session?.SpeakerModel?.ImageUrl;

		public Date StartTime => _session?.SessionModel.StartTime.DateTime.ToJavaDate();

		public Date EndTime => _session?.SessionModel.EndTime.DateTime.ToJavaDate();

		public ReactiveCommand GoDetailCommand { get; }

	}
}