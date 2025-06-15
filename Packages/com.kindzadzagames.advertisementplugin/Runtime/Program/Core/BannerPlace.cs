using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve, Serializable]
    public class BannerPlace
    {
        [field: SerializeField] public PlaceOnScreen _placeOnScreen { get; private set; }
        [field: SerializeField] public float _bannerHeight { get; private set; }
        [field: SerializeField] public float _bannerWidth { get; private set; }
    }
}
