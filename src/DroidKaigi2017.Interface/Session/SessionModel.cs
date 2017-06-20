#region

using System;

#endregion

namespace DroidKaigi2017.Interface.Session
{
	public class SessionModel
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public int SpeakerId { get; set; }

		public DateTimeOffset StartTime { get; set; }

		public DateTimeOffset EndTime { get; set; }

		public int DurationMin { get; set; }

		public SessionType Type { get; set; }

		public int TopicId { get; set; }

		public int RoomId { get; set; }

		public string Lang { get; set; }

		public string SlideUrl { get; set; }

		public string MovieUrl { get; set; }

		public string MovieDashUrl { get; set; }

		public string ShareUrl { get; set; }
	}
}