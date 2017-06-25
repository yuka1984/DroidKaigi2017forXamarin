using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace DroidKaigi2017.Interface.Feedback
{
    public interface IFeedbackService
    {
		ReadOnlyObservableCollection<SessionFeedbackModel> Feedbacks { get; }
		Task SubmitAsync(SessionFeedbackModel feedback);

	    Task SaveAsync(SessionFeedbackModel feedback);
    }

	public class SessionFeedbackModel
	{
		public int SessionId { get; set; }

		public string SessionTitle { get; set; }

		public int Relevancy { get; set; }

		public int AsExpected { get; set; }

		public int Difficulty { get; set; }

		public int Knowledgeable { get; set; }
		public string Comment { get; set; }

		public bool IsSubmitted { get; set; }
	}
}
