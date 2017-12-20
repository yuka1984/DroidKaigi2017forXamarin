using System;
using DroidKaigi2017.Interface.Tools;
using Newtonsoft.Json;

namespace DroidKaigi2017.iOS.Utils
{
    public class KeyValueStore : IKeyValueStore
    {
        private const string basedir = "kvs";

        public KeyValueStore()
        {

        }

        public bool Create(string key, object value)
        {
            if (Plugin.Settings.CrossSettings.Current.Contains(key))
            {
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(key, JsonConvert.SerializeObject(value));
            }
            return true;
        }

        public bool CreateNew(string key, object value)
        {
            return Plugin.Settings.CrossSettings.Current.AddOrUpdateValue(key, JsonConvert.SerializeObject(value));
        }

        public void Delete(string key)
        {
            Plugin.Settings.CrossSettings.Current.Remove(key);
        }

        public T GetValue<T>(string key)
        {
            var json = Plugin.Settings.CrossSettings.Current.GetValueOrDefault<string>(key);
            if (string.IsNullOrEmpty(json))
                return default(T);

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
