namespace Nyanto.Core
{
	public enum ActivityLifeCycycle
	{
		Created = 0,
		Destroyed,
		Paused,
		Resumed,
		SaveInstanceState,
		Started,
		Stopped
	}
}