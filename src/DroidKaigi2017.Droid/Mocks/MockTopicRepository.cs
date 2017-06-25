#region

using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Repository;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Droid.Mocks
{
	public class MockTopicRepository : ITopicRepository
	{
		private readonly ReactiveProperty<List<TopicModel>> _topiReactiveProperty;

		private bool IsDirty = true;

		public MockTopicRepository()
		{
			_topiReactiveProperty = new ReactiveProperty<List<TopicModel>>(initialValue: new List<TopicModel>(),
				raiseEventScheduler: TaskPoolScheduler.Default);
			TopicsObservable = _topiReactiveProperty.ToReadOnlyReactiveProperty(eventScheduler: TaskPoolScheduler.Default);
		}

		public ReadOnlyReactiveProperty<List<TopicModel>> TopicsObservable { get; }

		public async Task LoadAsync()
		{
			if (!IsDirty)
				return;
			;
			await Task.Delay(5000).ConfigureAwait(false);
			_topiReactiveProperty.Value = new List<TopicModel>
			{
				new TopicModel
				{
					Id = 0,
					Name = "None",
					Other = ""
				},
				new TopicModel
				{
					Id = 1,
					Name = "ProductivityAndTooling",
					Other = ""
				},
				new TopicModel
				{
					Id = 2,
					Name = "ArchitectureAndDevelopmentProcessMethodology",
					Other = ""
				},
				new TopicModel
				{
					Id = 3,
					Name = "Hardware",
					Other = ""
				},
				new TopicModel
				{
					Id = 4,
					Name = "UiAndDesign",
					Other = ""
				},
				new TopicModel
				{
					Id = 5,
					Name = "QualityAndSustainability",
					Other = ""
				},
				new TopicModel
				{
					Id = 6,
					Name = "Platform",
					Other = ""
				},
				new TopicModel
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