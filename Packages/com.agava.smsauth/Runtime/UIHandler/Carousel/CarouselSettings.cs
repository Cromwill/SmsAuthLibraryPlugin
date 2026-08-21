using System;
using UnityEngine;
using AdsAppView.Utility;
using UnityEngine.Scripting;
using System.Collections.Generic;
using SheetRemoteConfigs = SmsAuthAPI.Utility.SheetRemoteConfigs;

namespace Agava.Wink
{
    [Serializable, Preserve]
    public class CarouselSettings
    {
        private const string LineTransitionPattern = "end";

        [SerializeField] private List<CarouselData> _carouselDatas;

        private XMLValues _storeXMLValue;

        public IReadOnlyList<CarouselData> CarouselDatas => _carouselDatas;

        public void Construct(Store storeName)
        {
            if (SheetRemoteConfigs.Texts != null)
            {
                _storeXMLValue = GetValueByStore(storeName);
                Dictionary<string, string> data;

                for (int i = 0; i < _carouselDatas.Count; i++)
                {
                    if(SheetRemoteConfigs.Texts.Data.TryGetValue(_carouselDatas[i].AppAuthenticator.ToString(), out data))
                        _carouselDatas[i].SetAppName(data[_storeXMLValue.ToString()].Replace($"{{{LineTransitionPattern}}}", "\n"));
                    else
                        Debug.Log($"XML TEXT: download remote success, but can't find data with key {_carouselDatas[i].AppAuthenticator.ToString()}");
                }
            }
            else
            {
                Debug.Log($"XML TEXT: download remote failed, used prepared app names with store {storeName}");
            }
        }

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
