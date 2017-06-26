using DroidKaigi2017.Interface.Models;

namespace DroidKaigi2017.Interface.Services
{
	public class Session
	{
		public Session(SessionModel sesion, SpeakerModel speaker, RoomModel room, TopicModel topic)
		{
			Sesion = sesion;
			Speaker = speaker;
			Room = room;
			Topic = topic;
		}

		public SessionModel Sesion { get; }
		public SpeakerModel Speaker { get; }
		public RoomModel Room { get; }
		public TopicModel Topic { get; }
	}
}