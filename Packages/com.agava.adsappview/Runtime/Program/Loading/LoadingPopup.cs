using UnityEngine;
using AdsAppView.Utility;
using System.Collections.Generic;

namespace AdsAppView.Program
{
    public class LoadingPopup : MonoBehaviour
    {
        private const string LineTransitionPattern = "end";

        [SerializeField] private List<AppPresenter> _appPresenters;

        private XMLValues _storeXMLValue;

        public void Construct(Store storeName)
        {
            _storeXMLValue = GetValueByStore(storeName);

            Dictionary<string, string> data;

            for (int i = 0; i < _appPresenters.Count; i++)
            {
                if (SheetRemoteConfigs.Texts.Data.TryGetValue(_appPresenters[i].AppAuthenticator.ToString(), out data))
                    _appPresenters[i].SetAppName(data[_storeXMLValue.ToString()].Replace($"{{{LineTransitionPattern}}}", "\n"));
                else
                    Debug.Log($"XML TEXT: download remote success, but can't find data with key {_appPresenters[i].AppAuthenticator}");
            }
        }

        public void Activate() => gameObject.SetActive(true);

        public void Deactivate() => gameObject.SetActive(false);

        private XMLValues GetValueByStore(Store storeName)
        {
            return storeName switch
            {
                Store.AppStore => XMLValues.Value2,
                Store.RuStore => XMLValues.Value3,
                Store.Huawei => XMLValues.Value4,
                _ => XMLValues.Value1,
            };
        }
    }
}
