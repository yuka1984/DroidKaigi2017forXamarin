using System;
using System.Collections.Generic;
using System.Text;

namespace DroidKaigi2017.Interface.Tools
{
	public interface IKeyValueStore
	{
		bool Create(string key, object value);
		bool CreateNew(string key, object value);
		void Delete(string key);
		T GetValue<T>(string key);
	}
}
