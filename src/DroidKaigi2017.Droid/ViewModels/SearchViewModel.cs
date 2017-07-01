using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017;
using DroidKaigi2017.Droid.Views;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Services;
using DroidKaigi2017.Services;
using Java.Security;
using Nyanto;
using Nyanto.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace DroidKaigi2017.Droid.ViewModels
{
	public class SearchViewModel : ViewModelBase
	{
		private readonly ISessionService _sessionService;
		private readonly IMySessionService _mySessionService;
		private readonly INavigator _navigator;

		public SearchViewModel(ISessionService sessionService, IMySessionService mySessionService, INavigator navigator)
		{
			_sessionService = sessionService;
			_mySessionService = mySessionService;
			_navigator = navigator;

			SearchCommand = IsActiveObservable.ToReactiveCommand<string>();
			SearchSessions = sessionService.Sessions
				.Select(x =>
				{
					var results = new List<SearchResultViewModel>();
					foreach (var session in x)
					{
						results.Add(new SearchResultViewModel(SearchResultViewModel.Type.Title, session.SessionModel.Title, _navigator,
							session, _mySessionService));
						results.Add(new SearchResultViewModel(SearchResultViewModel.Type.Description, session.SessionModel.Description,
							_navigator, session, _mySessionService));
						if (session.SpeakerModel != null)
						{
							results.Add(new SearchResultViewModel(SearchResultViewModel.Type.Speaker, session.SpeakerModel.Name, _navigator,
								session, _mySessionService));
						}
					}
					return results;
				})
				.CombineLatest(SearchCommand, (sessions, word) => new {sessions, word})
				.Select(x =>
				{
					if (string.IsNullOrEmpty(x.word))
						return Array.Empty<SearchResultViewModel>();

					x.sessions.ForEach(y => y.SearchCommand.CheckExecute(x.word));
					return x.sessions.Where(y => y.IsMatch).ToArray();
				})
				.ToReadOnlySwitchReactiveProperty(IsActiveObservable, initialValue: Array.Empty<SearchResultViewModel>())
				.AddTo(CompositeDisposable)
				;
			CloseCommand = new ReactiveCommand();
			CloseCommand.Subscribe(x =>
			{
				_navigator.Finish();
			});
		}

		public IReadOnlyReactiveProperty<SearchResultViewModel[]> SearchSessions { get; }

		public ReactiveCommand<string> SearchCommand { get; }

		public ReactiveCommand CloseCommand { get; }

	}

	public class SearchResultViewModel
	{
		private readonly INavigator _navigator;
		private readonly IMySessionService _mySessionService;
		private readonly Session _session;
		

		public SearchResultViewModel(Type type, string text, INavigator navigator, Session session, IMySessionService mySessionService)
		{
			_navigator = navigator;
			_session = session;
			_mySessionService = mySessionService;
			this.Text = text;
			this.ResultType = type;
			GoDetailCommand = new ReactiveCommand();
			GoDetailCommand.Subscribe(x =>
			{
				_navigator.NavigateTo(NavigationKey.GoSessionDetail, _session.SessionModel.Id);
			});
			SearchCommand = new ReactiveCommand<string>();
			SearchCommand.Subscribe(x =>
			{
				SearchWord = x;
			});
		}

		public ReactiveCommand<string> SearchCommand { get; }

		public string SearchWord { get; private set; } = "";

		public bool IsMatch => Text?.ToLower()?.Contains(SearchWord?.ToLower()) ?? false;

		public string SessionTitle => _session.SessionModel.Title;

		public string SpeakerImageUrl => _session.SpeakerModel?.ImageUrl;

		public Type ResultType { get; }

		public int RearchResultId => _session.SessionModel.Id * 100 + (int) ResultType;

		public bool IsEllipsis => ResultType == Type.Description;

		public SessionModel Session => _session.SessionModel;

		public int TextAppearanceSpanId => Resource.Style.SearchResultAppearance;

		public bool IsMySession => _mySessionService.MySessions.Any(x => x == _session.SessionModel.Id);

		public ReactiveCommand GoDetailCommand { get; }

		public string Text { get; }


		public enum Type
		{
			Title = 0,
			Description,
			Speaker,
		}

		public static int GetTypeResourceid(Type type)
		{
			switch (type)
			{
				case Type.Title:
					return Resource.Drawable.ic_title_12_vector;
				case Type.Description:
					return Resource.Drawable.ic_description_12_vector;
				case Type.Speaker:
					return Resource.Drawable.ic_speaker_12_vector;
			}
			throw new ArgumentException();
		}
	}
}