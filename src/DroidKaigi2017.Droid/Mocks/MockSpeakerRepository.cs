#region

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
	public class MockSpeakerRepository : ISpeakerRepository
	{
		private readonly ReactiveProperty<List<SpeakerModel>> _speakerProperty;

		private bool IsDirty = true;

		public MockSpeakerRepository()
		{
			_speakerProperty = new ReactiveProperty<List<SpeakerModel>>(initialValue: new List<SpeakerModel>(),
				raiseEventScheduler: TaskPoolScheduler.Default);
			SpealersObservable = _speakerProperty.ToReadOnlyReactiveProperty(eventScheduler: TaskPoolScheduler.Default);
		}

		public ReadOnlyReactiveProperty<List<SpeakerModel>> SpealersObservable { get; }

		public async Task LoadAsync()
		{
			if (!IsDirty)
				return;
			await Task.Delay(5000).ConfigureAwait(false);
			var httpClient = new HttpClient();
			var r = await httpClient.GetStringAsync("https://droidkaigi.github.io/2017/sessions.json");
			var ent = JsonConvert.DeserializeObject<List<MockSessionRepository.Session>>(r);

			_speakerProperty.Value = ent.Select(x => new SpeakerModel
				{
					Id = x.Speaker?.Id ?? -1,
					ImageUrl = "https://droidkaigi.github.io/2017" + x.Speaker?.ImageUrl,
					Name = x.Speaker?.Name,
					TwitterName = x.Speaker?.TwitterName
				})
				.ToList();
			IsDirty = false;
		}
	}
}