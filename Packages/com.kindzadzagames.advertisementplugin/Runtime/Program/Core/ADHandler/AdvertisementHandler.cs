using UnityEngine.Scripting;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve]
    public abstract class AdvertisementHandler
    {
        protected abstract bool CanLoadAd();
        protected abstract void LoadAd();
        protected abstract bool AdIsLoaded();
        protected abstract void ShowAd();
        protected abstract void DestroyAd();
        protected abstract string GetPlacementName();
    }
}
