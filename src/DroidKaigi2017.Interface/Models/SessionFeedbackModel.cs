namespace DroidKaigi2017.Interface.Models
{
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