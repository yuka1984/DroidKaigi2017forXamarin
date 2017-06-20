namespace Nyanto.Core
{
	public interface IObjectStoreOwner<T>
	{
		ObjectStore<T> GetStore();
	}
}