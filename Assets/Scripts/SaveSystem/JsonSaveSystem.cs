using System;
using System.IO;
using UnityEngine;

namespace DriftGame.System
{
    public class JsonSaveSystem : ISaveSystem
    {
        public void Save(string key, object data, Action<bool> callback = null)
        {
            var path = BuildPath(key);
            var json = JsonUtility.ToJson(data);

            using (var fileStream = new StreamWriter(path))
            {
                fileStream.Write(json);
            }

            callback?.Invoke(true);
        }

        public void Load<T>(string key, Action<T> callback)
        {
            var path = BuildPath(key);

            using (var fileStream = new StreamReader(path))
            {
                var json = fileStream.ReadToEnd();
                var data = JsonUtility.FromJson<T>(json);

                callback.Invoke(data);
            }
        }

        private string BuildPath(string key)
        {
            return Path.Combine(Application.persistentDataPath, key);
        }
    }
}
