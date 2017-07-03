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
using Reactive.Bindings;

namespace DroidKaigi2017.Service
{
    class AzureEasyTableSpeakerRepository : ISpeakerRepository
    {
	    private readonly MobileServiceClient _client;
	    private readonly IKeyValueStore _keyValueStore;
	    private bool _isDirty = true;
	    private readonly ReactiveProperty<List<SpeakerModel>> _speakerProperty = new ReactiveProperty<List<SpeakerModel>>(raiseEventScheduler: TaskPoolScheduler.Default);

	    public AzureEasyTableSpeakerRepository(IKeyValueStore keyValueStore, MobileServiceClient client)
	    {
		    _keyValueStore = keyValueStore;
		    _client = client;
		    SpealersObservable = _speakerProperty.ToReadOnlyReactiveProperty(eventScheduler: TaskPoolScheduler.Default);
	    }
		public ReadOnlyReactiveProperty<List<SpeakerModel>> SpealersObservable { get; }
	    public async Task LoadAsync()
	    {
			if (_isDirty)
			{
				try
				{
					var table = _client.GetTable("speakers");
					var list = (await table.ReadAsync("")).ToObject<List<SpeakerItem>>();
					var topics = list
						.Select(x => new SpeakerModel()
						{
							Id = x.SpeakerId,
							Name = x.Name,
							ImageUrl = "https://droidkaigi.github.io/2017" + x.ImageUrl,
							TwitterName = x.TwitterName,

						}).ToList();

					_speakerProperty.Value = topics;

					_isDirty = false;

					_keyValueStore.CreateNew("Speakers", topics);
				}
				catch (Exception e)
				{
					System.Diagnostics.Trace.TraceWarning(e.ToString());
					try
					{
						var savedata = _keyValueStore.GetValue<List<SpeakerModel>>("Speakers");
						_speakerProperty.Value = savedata;
					}
					catch (Exception exception)
					{
						Console.WriteLine(exception);
					}
				}
			}
		}
	    private class SpeakerItem
	    {
		    public int SpeakerId { get; set; }

		    public string Name { get; set; }

		    public string ImageUrl { get; set; }

		    public string TwitterName { get; set; }
	    }
	}
}
