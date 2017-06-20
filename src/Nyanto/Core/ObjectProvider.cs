#region

using System;

#endregion

namespace Nyanto.Core
{
	public class ObjectProvider<T>
	{
		private readonly Func<T> _factory;
		private readonly ObjectStore<T> _store;

		public ObjectProvider(IObjectStoreOwner<T> owner, Func<T> factory = null) : this(owner.GetStore(), factory)
		{
		}

		public ObjectProvider(ObjectStore<T> store, Func<T> factory = null)
		{
			_factory = factory ?? GetDefaultFactory();
			_store = store;
		}

		public T Get(Type objType)
		{
			var fullname = objType.FullName;
			return Get("android.arch.lifecycle.ViewModelProvider.DefaultKey:" + fullname);
		}

		public T Get(string key)
		{
			var obj = _store.Get(key);
			if (obj != null)
				return obj;

			obj = _factory.Invoke();
			_store.Add(key, obj);
			return obj;
		}

		private static Func<T> GetDefaultFactory()
		{
			return Activator.CreateInstance<T>;
		}
	}
}