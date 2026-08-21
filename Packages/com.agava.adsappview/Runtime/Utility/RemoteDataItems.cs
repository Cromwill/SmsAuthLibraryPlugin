using System;
using UnityEngine.Scripting;
using System.Collections.Generic;

namespace AdsAppView.Utility
{
    [Preserve, Serializable]
    public class RemoteDataItems
    {
        private readonly Dictionary<string, Dictionary<string, string>> _keyRemDatas = new();

        public IReadOnlyDictionary<string, Dictionary<string, string>> Data => _keyRemDatas;

        public void Add(string key, Dictionary<string, string> keyValuePair) => _keyRemDatas.Add(key, keyValuePair);
    }
}
