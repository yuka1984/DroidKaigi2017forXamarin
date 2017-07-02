using System;

namespace DroidKaigi2017.Interface.Models
{
	public enum SessionType
	{
		Ceremony,
		Session,
		Break,
		Dinner
	}

	public static class SessionTypeConverter
	{
		public static SessionType Convert(this string sessionString)
		{
			switch (sessionString)
			{
				case CeremonyType:
					return Models.SessionType.Ceremony;
				case SessionType:
					return Models.SessionType.Session;
				case BreakType:
					return Models.SessionType.Break;
				case DinnerType:
					return Models.SessionType.Dinner;
			}
			throw new Exception();
		}
		public const string CeremonyType = "ceremony";
		public const string SessionType = "session";
		public const string BreakType = "break";
		public const string DinnerType = "dinner";
	}

	
}