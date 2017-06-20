namespace DroidKaigi2017.Interface.Room
{
	public class RoomModel
	{
		public int Id { get; set; }
		public string Name { get; set; }

		/// <summary>セッションが行われる</summary>
		public bool IsSession { get; set; }
	}
}