using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Repository;
using DroidKaigi2017.Interface.Tools;
using Microsoft.WindowsAzure.MobileServices;
using Nyanto.Core;
using Reactive.Bindings;

namespace DroidKaigi2017.Service
{
    class AzureEasyTableTopicRepository : ITopicRepository
    {
	    private readonly MobileServiceClient _client;
	    private readonly IKeyValueStore _keyValueStore;
	    private bool _isDirty = true;
	    private readonly ReactiveProperty<List<TopicModel>> _topicProperty = new ReactiveProperty<List<TopicModel>>(raiseEventScheduler: TaskPoolScheduler.Default);

	    public AzureEasyTableTopicRepository(IKeyValueStore keyValueStore, MobileServiceClient client)
	    {
		    _keyValueStore = keyValueStore;
		    _client = client;
		    TopicsObservable = _topicProperty.ToReadOnlyReactiveProperty(eventScheduler: TaskPoolScheduler.Default);
	    }


		public ReadOnlyReactiveProperty<List<TopicModel>> TopicsObservable { get; }
	    public async Task LoadAsync()
	    {
			if (_isDirty)
			{
				try
				{
					var table = _client.GetTable("topics");
					var list = (await table.ReadAsync("")).ToObject<List<TopicItem>>();
					var topics = list
						.Select(x => new TopicModel()
						{
							Id = x.TopicId,
							Name = x.Name,
							Other = x.Other
							
						}).ToList();

					_topicProperty.Value = topics;

					_isDirty = false;

					_keyValueStore.CreateNew("Topics", topics);
				}
				catch (Exception e)
				{
					System.Diagnostics.Trace.TraceWarning(e.ToString());
					try
					{
						var savedata = _keyValueStore.GetValue<List<TopicModel>>("Topics");
						_topicProperty.Value = savedata;
					}
					catch (Exception exception)
					{
						Console.WriteLine(exception);
					}
				}
			}
		}
	    private class TopicItem
	    {
		    public int TopicId { get; set; }
		    public string Name { get; set; }
		    public string Other { get; set; }
	    }
	}
}
