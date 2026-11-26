using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace KinDzaDza.RateUs
{
    public static class SheetRemoteConfigs
    {
        private const char CellSeporator = '|';

        private static RateUsConfigProvider _api;

        public static RemoteDataItems Texts { get; private set; }

        public static IEnumerator Initialize(RateUsConfigProvider api)
        {
            _api = api;

            bool isComplete = false;
            var wait = new WaitUntil(() => isComplete);

            Debug.Log($"[RateUs SheetRemoteConfigs] Start load file");
            LoadText(() => isComplete = true);

            yield return wait;
        }

        private static async void LoadText(Action onComplete)
        {
            var responce = await _api.GetRemoteServerConfig();

            if (responce.statusCode == UnityWebRequest.Result.Success)
            {
                string cvsRawData = responce.body;
                Texts = GetData(cvsRawData);
                Debug.Log($"[RateUs SheetRemoteConfigs] Complete to load file");
                onComplete?.Invoke();
            }
            else
            {
                Debug.LogError($"[RateUs SheetRemoteConfigs] Fail to load file");
                onComplete?.Invoke();
            }
        }

        private static RemoteDataItems GetData(string cvsRawData)
        {
            char lineEnding = GetPlatformSpecificLineEnd();
            string[] rows = cvsRawData.Split(lineEnding);
            int dataStartRawIndex = 1;
            int dataStartCellIndex = 1;

            RemoteDataItems data = new();

            for (int r = dataStartRawIndex; r < rows.Length; r++) //columns[0] - Raw Key on table, keyValuePairs -> <Column(key), columns[n](Value)>
            {
                string[] columnsKey = rows[0].Split(CellSeporator);
                string[] columns = rows[r].Split(CellSeporator);

                Dictionary<string, string> keyValuePairs = new();

                for (int c = dataStartCellIndex; c < columns.Length; c++)
                {
                    string key = columnsKey[c].TrimEnd();
                    string value = columns[c].TrimEnd();
                    keyValuePairs.Add(key, value);
                }

                data.Add(columns[0], keyValuePairs);
            }

            return data;
        }

        private static char GetPlatformSpecificLineEnd()
        {
            char lineEnding = '\n';

            return lineEnding;
        }
    }
}
