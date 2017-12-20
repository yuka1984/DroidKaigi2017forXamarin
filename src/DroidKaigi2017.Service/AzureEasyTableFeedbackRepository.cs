using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Repository;
using DroidKaigi2017.Interface.Tools;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reactive.Bindings;

namespace DroidKaigi2017.Service
{
    public class AzureEasyTableFeedbackRepository : IFeedbackRepository
    {
	    private const string Key = "AzureEasyTableFeedbackRepository";
	    private readonly IKeyValueStore keyValueStore;
	    private readonly MobileServiceClient _client;
		private readonly ObservableCollection<SessionFeedbackModel> _feedbacks = new ObservableCollection<SessionFeedbackModel>();
	    private IMobileServiceTable _table;


		public AzureEasyTableFeedbackRepository(IKeyValueStore keyValueStore, MobileServiceClient client)
	    {
		    this.keyValueStore = keyValueStore;
		    _client = client;
		    _table = client.GetTable("feedbacks");
		    Feedbacks = _feedbacks.ToReadOnlyReactiveCollection();
	    }

	    public ReadOnlyObservableCollection<SessionFeedbackModel> Feedbacks { get; }
	    public async Task SubmitAsync(SessionFeedbackModel feedback)
	    {
		    var fb = _feedbacks.FirstOrDefault(x => x.SessionId == feedback.SessionId);
		    if (fb != null && fb.IsSubmitted)
		    {
			    return;
		    }
		    else if (fb != null && fb.IsSubmitted == false)
		    {
			    _feedbacks.Remove(_feedbacks.FirstOrDefault(x => x.SessionId == feedback.SessionId));
		    }

			await _table.InsertAsync(JObject.FromObject(new FeedBack
			{
				SessionId = feedback.SessionId,
				Difficulty = feedback.Difficulty,
				Relevancy = feedback.Relevancy,
				Knowledgeable = feedback.Knowledgeable,
				AsExpected = feedback.AsExpected,
				Comment = feedback.Comment,
			}));
		    
			feedback.IsSubmitted = true;
		    _feedbacks.Add(feedback);
		    keyValueStore.CreateNew(Key, _feedbacks.ToList());
		}

	    public Task SaveAsync(SessionFeedbackModel feedback)
	    {
		    return Task.Run(() =>
		    {
			    var fb = _feedbacks.FirstOrDefault(x => x.SessionId == feedback.SessionId);
			    if (fb != null && fb.IsSubmitted)
			    {
				    return;
			    }
			    else if (fb != null && fb.IsSubmitted == false)
			    {
				    _feedbacks.Remove(_feedbacks.FirstOrDefault(x => x.SessionId == feedback.SessionId));
			    }
			    feedback.IsSubmitted = false;
			    _feedbacks.Add(feedback);
			    keyValueStore.CreateNew(Key, _feedbacks.ToList());
		    });

	    }

	    public Task LoadAsync()
	    {
		    return Task.Run(() =>
		    {
				var list = keyValueStore.GetValue<List<SessionFeedbackModel>>(Key);
			    if (list != null)
			    {
				    _feedbacks.Clear();
				    list.ForEach(x => _feedbacks.Add(x));
			    }
			});
	    }

	    public class FeedBack
	    {
		    public int SessionId { get; set; }
		    public int Relevancy { get; set; }

		    public int AsExpected { get; set; }

		    public int Difficulty { get; set; }

		    public int Knowledgeable { get; set; }
		    public string Comment { get; set; }
	    }
    }
}
