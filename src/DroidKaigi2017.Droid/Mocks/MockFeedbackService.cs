using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DroidKaigi2017.Interface.Feedback;

namespace DroidKaigi2017.Droid.Mocks
{
	public class MockFeedbackService: IFeedbackService
	{
		public MockFeedbackService()
		{
			_feedbacks = new ObservableCollection<SessionFeedbackModel>();
			Feedbacks = new ReadOnlyObservableCollection<SessionFeedbackModel>(_feedbacks);
		}
		private ObservableCollection<SessionFeedbackModel> _feedbacks;
		public ReadOnlyObservableCollection<SessionFeedbackModel> Feedbacks { get; }

		public Task SubmitAsync(SessionFeedbackModel feedback)
		{
			var fb = _feedbacks.FirstOrDefault(x => x.SessionId == feedback.SessionId);
			if (fb != null && fb.IsSubmitted)
			{
				return Task.CompletedTask;
			}
			else if (fb != null && fb.IsSubmitted == false)
			{
				_feedbacks.Remove(_feedbacks.FirstOrDefault(x => x.SessionId == feedback.SessionId));
			}
			feedback.IsSubmitted = true;
			_feedbacks.Add(feedback);
			return Task.CompletedTask;
		}

		public Task SaveAsync(SessionFeedbackModel feedback)
		{
			var fb = _feedbacks.FirstOrDefault(x => x.SessionId == feedback.SessionId);
			if (fb != null && fb.IsSubmitted)
			{
				return Task.CompletedTask;
			}
			else if (fb != null && fb.IsSubmitted == false)
			{
				_feedbacks.Remove(_feedbacks.FirstOrDefault(x => x.SessionId == feedback.SessionId));
			}
			feedback.IsSubmitted = false;
			_feedbacks.Add(feedback);
			return Task.CompletedTask;
		}
	}
}