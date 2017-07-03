using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DroidKaigi2017.Droid.Mocks;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Repository;
using DroidKaigi2017.Interface.Tools;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Reactive.Bindings;

namespace DroidKaigi2017.Service
{
    public class AzureEasyTableSessionRepository: ISessionRepository
    {
	    private readonly IKeyValueStore _keyValueStore;
	    private bool _isDirty = true;
	    private readonly MobileServiceClient _client;
	    private readonly ReactiveProperty<List<SessionModel>> _sessionProperty = new ReactiveProperty<List<SessionModel>>(raiseEventScheduler:TaskPoolScheduler.Default);		

	    public AzureEasyTableSessionRepository(IKeyValueStore keyValueStore, MobileServiceClient client)
	    {
		    _keyValueStore = keyValueStore;
		    _client = client;
		    SessionsObservable = _sessionProperty.ToReadOnlyReactiveProperty(eventScheduler:TaskPoolScheduler.Default);
	    }

	    public async Task LoadAsync()
	    {
		    if (_isDirty)
		    {
			    try
			    {
				    var table = _client.GetTable("sessions");
				    var list = (await table.ReadAsync("")).ToObject<List<SessionItem>>();
				    var session = list
					    .Select(x => new SessionModel
					    {
						    Id = x.SessionId,
						    Type = x.Type.Convert(),
						    Title = x.Title,
						    Lang = x.Lang,
						    SlideUrl = x.SlideUrl,
						    Description = x.Description,
						    EndTime = DateTimeOffset.Parse(x.EndTime),
						    StartTime = DateTimeOffset.Parse(x.StartTime),
						    RoomId = x.RoomId ?? 0,
						    SpeakerId = x.SpeakerId ?? 0,
						    MovieUrl = x.MovieUrl,
						    TopicId = x.TopicId ?? 0,
						    MovieDashUrl = x.MovieDashUrl,
						    DurationMin = int.Parse(x.DurationMin),
						    ShareUrl = x.ShareUrl
					    }).ToList();

				    _sessionProperty.Value = session;

				    _isDirty = false;

				    _keyValueStore.CreateNew("Sessions", session);
			    }
			    catch (Exception e)
			    {
				    System.Diagnostics.Trace.TraceWarning(e.ToString());
				    try
				    {
					    var savedata = _keyValueStore.GetValue<List<SessionModel>>("Sessions");
					    _sessionProperty.Value = savedata;
				    }
				    catch (Exception exception)
				    {
					    Console.WriteLine(exception);
				    }
			    }
		    }
	    }

	    public ReadOnlyReactiveProperty<List<SessionModel>> SessionsObservable { get; }

	    private class SessionItem
	    {
		    public int SessionId { get; set; }
		    public string Title { get; set; }
		    public string Description { get; set; }
		    public int? SpeakerId { get; set; }
		    public string StartTime { get; set; }
		    public string EndTime { get; set; }
		    public string DurationMin { get; set; }
		    public string Type { get; set; }
		    public int? TopicId { get; set; }
		    public int? RoomId { get; set; }
		    public string Lang { get; set; }
		    public string SlideUrl { get; set; }
		    public string MovieUrl { get; set; }
		    public string MovieDashUrl { get; set; }
		    public string ShareUrl { get; set; }
	    }
	}
}
