using UnityEngine.Scripting;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve]
    public interface IAdBlocker
    {
        public bool DisplayBlocked { get; }

        public void RemoveRestriction();
    }
}
