using System;

namespace DriftGame.System
{
    public interface ISaveSystem
    {
        void Save(string key, object data, Action<bool> callback = null);
        void Load<T>(string key, Action<T> callback);
    }
}
