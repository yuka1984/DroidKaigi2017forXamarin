using DroidKaigi2017.Interface.Models;

namespace DroidKaigi2017.Services
{
	public class Session
	{
		public Session(SessionModel sessionModel, SpeakerModel speakerModel, RoomModel roomModel, TopicModel topicModel)
		{
			SessionModel = sessionModel;
			SpeakerModel = speakerModel;
			RoomModel = roomModel;
			TopicModel = topicModel;
		}

		public SessionModel SessionModel { get; }
		public SpeakerModel SpeakerModel { get; }
		public RoomModel RoomModel { get; }
		public TopicModel TopicModel { get; }
	}
}