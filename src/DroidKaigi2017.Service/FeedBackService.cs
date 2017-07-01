using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Repository;
using DroidKaigi2017.Interface.Services;
using Reactive.Bindings.Notifiers;

namespace DroidKaigi2017.Service
{
    public class FeedBackService : IFeedBackService
	{
	    private readonly IFeedbackRepository _feedbackRepository;

		public IObservable<bool> IsLoading { get; }

	    private readonly BusyNotifier _busyNotifier;

		public ReadOnlyObservableCollection<SessionFeedbackModel> Feedbacks { get; }

	    public FeedBackService(IFeedbackRepository feedbackRepository)
	    {
		    _feedbackRepository = feedbackRepository;
			Feedbacks = _feedbackRepository.Feedbacks;

			_busyNotifier = new BusyNotifier();

		    IsLoading = _busyNotifier;
	    }

	    public async Task SaveAsync(SessionFeedbackModel feedback)
	    {
		    using (_busyNotifier.ProcessStart())
		    {
				await _feedbackRepository.SaveAsync(feedback);
			}		    
	    }

	    public async Task SubmitAsync(SessionFeedbackModel feedback)
	    {
		    using (_busyNotifier.ProcessStart())
		    {
			    await _feedbackRepository.SubmitAsync(feedback);

		    }
	    }

	    public async Task LoadAsync()
	    {
		    using (_busyNotifier.ProcessStart())
		    {
				await _feedbackRepository.LoadAsync();
			}
	    }

    }
}
