#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Repository;
using Newtonsoft.Json;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Droid.Mocks
{
	public class MockSessionRepository : ISessionRepository
	{
		private readonly ReactiveProperty<List<SessionModel>> _sessionReactiveProperty;

		private bool IsDirty = true;

		public MockSessionRepository()
		{
			_sessionReactiveProperty = new ReactiveProperty<List<SessionModel>>(initialValue: new List<SessionModel>(),
				raiseEventScheduler: TaskPoolScheduler.Default);
			SessionsObservable = _sessionReactiveProperty.ToReadOnlyReactiveProperty(eventScheduler: TaskPoolScheduler.Default,
				mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);
		}

		public ReadOnlyReactiveProperty<List<SessionModel>> SessionsObservable { get; }

		public async Task LoadAsync()
		{
			if (!IsDirty)
				return;
			;

			await Task.Delay(5000).ConfigureAwait(false);
			var httpClient = new HttpClient();
			var r = await httpClient.GetStringAsync("https://droidkaigi.github.io/2017/sessions.json");
			var ent = JsonConvert.DeserializeObject<List<Session>>(r);

			_sessionReactiveProperty.Value = ent.Select(x => new SessionModel
				{
					DurationMin = x.DurationMin,
					EndTime = x.EndTime,
					Id = x.Id,
					Lang = x.Lang,
					Type = SessionType.Session,
					StartTime = x.StartTime,
					SpeakerId = x.Speaker?.Id ?? 0,
					RoomId = x.Room?.Id ?? 0,
					Title = x.Title,
					MovieDashUrl = x.MovieDashUrl,
					MovieUrl = x.MovieUrl,
					ShareUrl = x.ShareUrl,
					SlideUrl = x.SlideUrl,
					TopicId = x.TopicModel?.Id ?? 0,
					Description = x.Description
				})
				.ToList();
			IsDirty = false;
		}

		public class Session
		{
			public const string CeremonyType = "CEREMONY";
			public const string SessionType = "SESSION";
			public const string BreakType = "BREAK";
			public const string DinnerType = "DINNER";

			[JsonProperty("id")]
			public int Id { get; set; }

			[JsonProperty("title")]
			public string Title { get; set; }

			[JsonProperty("desc")]
			public string Description { get; set; }

			[JsonProperty("speaker")]
			public Speaker Speaker { get; set; }

			[JsonProperty("stime")]
			public DateTimeOffset StartTime { get; set; }

			[JsonProperty("etime")]
			public DateTimeOffset EndTime { get; set; }

			[JsonProperty("duration_min")]
			public int DurationMin { get; set; }

			[JsonProperty("type")]
			public string Type { get; set; }

			[JsonProperty("topic")]
			public TopicModel TopicModel { get; set; }

			[JsonProperty("room")]
			public Room Room { get; set; }

			[JsonProperty("lang")]
			public string Lang { get; set; }

			[JsonProperty("slide_url")]
			public string SlideUrl { get; set; }

			[JsonProperty("movie_url")]
			public string MovieUrl { get; set; }

			[JsonProperty("movie_dash_url")]
			public string MovieDashUrl { get; set; }

			[JsonProperty("share_url")]
			public string ShareUrl { get; set; }
		}

		public class Room
		{
			[JsonProperty("id")]
			public int Id { get; set; }

			[JsonProperty("name")]
			public string Name { get; set; }
		}

		public class Speaker
		{
			[JsonProperty("id")]
			public int Id { get; set; }

			[JsonProperty("name")]
			public string Name { get; set; }

			[JsonProperty("image_url")]
			public string ImageUrl { get; set; }

			[JsonProperty("twitter_name")]
			public string TwitterName { get; set; }
		}
	}
}