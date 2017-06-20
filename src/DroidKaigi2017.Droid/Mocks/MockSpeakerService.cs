#region

using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Speaker;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Droid.Mocks
{
	public class MockSpeakerService : ISpeakerService
	{
		private readonly ReactiveProperty<List<SpeakerModel>> _speakerProperty;

		private bool IsDirty = true;

		public MockSpeakerService()
		{
			_speakerProperty = new ReactiveProperty<List<SpeakerModel>>(initialValue: new List<SpeakerModel>(),
				raiseEventScheduler: TaskPoolScheduler.Default);
			SpealersObservable = _speakerProperty.ToReadOnlyReactiveProperty(eventScheduler: TaskPoolScheduler.Default);
		}

		public ReadOnlyReactiveProperty<List<SpeakerModel>> SpealersObservable { get; }

		public async Task LoadAsync()
		{
			if (IsDirty)
				return;
			await Task.Delay(5000);
			_speakerProperty.Value = new List<SpeakerModel>
			{
				new SpeakerModel
				{
					Id = 1,
					ImageUrl = "http://www.starico-03.com/stamp/outline/a218311-0.png",
					Name = "あざらしさん-1",
					TwitterName = @"@azarashiSan"
				}
			};
			IsDirty = false;
		}
	}
}