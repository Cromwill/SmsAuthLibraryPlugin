using TMPro;
using UnityEngine;
using SmsAuthAPI.Utility;
using System.Collections.Generic;

namespace Agava.Wink
{
    public class XmlCarouselConfigTexts : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [Header("Games Count")]
        [SerializeField] private XMLKeys _gamesKey;
        [SerializeField] private XMLValues _gamesValue;
        [SerializeField] private string _gamesFallbackText;
        [Header("Cartoons Count")]
        [SerializeField] private XMLKeys _cartoonsKey;
        [SerializeField] private XMLValues _cartoonsValue;
        [SerializeField] private string _cartoonsFallbackText;
        [Header("Children Channels Count")]
        [SerializeField] private XMLKeys _channelsKey;
        [SerializeField] private XMLValues _channelsValue;
        [SerializeField] private string _channelsFallbackText;

        private Dictionary<CarouselID, string> _texts = new Dictionary<CarouselID, string>();

        public void FillTexts()
        {
            if (SheetRemoteConfigs.Texts != null)
            {
                _texts.Add(CarouselID.GamesCount, SheetRemoteConfigs.Texts.Data[_gamesKey.ToString()][_gamesValue.ToString()]);
                _texts.Add(CarouselID.CartoonsCount, SheetRemoteConfigs.Texts.Data[_cartoonsKey.ToString()][_cartoonsValue.ToString()]);
                _texts.Add(CarouselID.ChannelsCount, SheetRemoteConfigs.Texts.Data[_channelsKey.ToString()][_channelsValue.ToString()]);
            }
            else
            {
                Debug.Log($"XML TEXT: download remote failed, used prepared texts for Carousel.");

                _texts.Add(CarouselID.GamesCount, _gamesFallbackText);
                _texts.Add(CarouselID.CartoonsCount, _cartoonsFallbackText);
                _texts.Add(CarouselID.ChannelsCount, _channelsFallbackText);
            }
        }

        public void SetText(CarouselID carouselID)
        {
            if(_texts.TryGetValue(carouselID, out string text))
                _text.text = text;
        }
    }
}
