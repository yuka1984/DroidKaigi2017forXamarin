#region

using System;
using System.Collections.Generic;

#endregion

namespace Nyanto.Core
{
	public class ObjectStore<T>
	{
		private readonly Dictionary<string, T> mMap = new Dictionary<string, T>();

		public void Add(string key, T @object)
		{
			lock (mMap)
			{
				if (mMap.ContainsKey(key))
				{
					(@object as IDisposable)?.Dispose();
					mMap.Remove(key);
				}
				mMap.Add(key, @object);
			}
		}

		public T Get(string key)
		{
			lock (mMap)
			{
				if (mMap.ContainsKey(key))
					return mMap[key];
				return default(T);
			}
		}

		public void Clear()
		{
			lock (mMap)
			{
				foreach (var keyValue in mMap)
					(keyValue.Value as IDisposable)?.Dispose();
				mMap.Clear();
			}
		}
	}
}