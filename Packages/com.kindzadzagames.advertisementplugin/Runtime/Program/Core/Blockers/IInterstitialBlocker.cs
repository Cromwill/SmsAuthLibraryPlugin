using UnityEngine.Scripting;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve]
    public interface IInterstitialBlocker : IAdBlocker
    {
        public bool InterstitialDisplayBlocked { get; }
    }
}
