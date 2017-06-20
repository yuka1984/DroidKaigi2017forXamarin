#region

using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Topic;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Droid.Mocks
{
	public class MockTopicService : ITopicService
	{
		private readonly ReactiveProperty<List<Topic>> _topiReactiveProperty;

		private bool IsDirty = true;

		public MockTopicService()
		{
			_topiReactiveProperty = new ReactiveProperty<List<Topic>>(initialValue: new List<Topic>(),
				raiseEventScheduler: TaskPoolScheduler.Default);
			TopicsObservable = _topiReactiveProperty.ToReadOnlyReactiveProperty(eventScheduler: TaskPoolScheduler.Default);
		}

		public ReadOnlyReactiveProperty<List<Topic>> TopicsObservable { get; }

		public async Task LoadAsync()
		{
			if (!IsDirty)
				return;
			;
			await Task.Delay(5000).ConfigureAwait(false);
			_topiReactiveProperty.Value = new List<Topic>
			{
				new Topic
				{
					Id = 0,
					Name = "None",
					Other = ""
				},
				new Topic
				{
					Id = 1,
					Name = "ProductivityAndTooling",
					Other = ""
				},
				new Topic
				{
					Id = 2,
					Name = "ArchitectureAndDevelopmentProcessMethodology",
					Other = ""
				},
				new Topic
				{
					Id = 3,
					Name = "Hardware",
					Other = ""
				},
				new Topic
				{
					Id = 4,
					Name = "UiAndDesign",
					Other = ""
				},
				new Topic
				{
					Id = 5,
					Name = "QualityAndSustainability",
					Other = ""
				},
				new Topic
				{
					Id = 6,
					Name = "Platform",
					Other = ""
				},
				new Topic
				{
					Id = 7,
					Name = "Other",
					Other = ""
				}
			};
			IsDirty = false;
		}
	}
}