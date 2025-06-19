using UnityEngine.Scripting;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve]
    public interface IBannerBlocker : IAdBlocker
    {
        public bool BannerDisplayBlocked { get; }
    }
}
