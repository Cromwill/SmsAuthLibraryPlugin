using UnityEngine.Scripting;
using UnityEngine.UI;

namespace KinDzaDzaGames.AdvertisementPlugin.DTO
{
    [Preserve]
    public class RaycastTarget : Graphic
    {
        public override void SetMaterialDirty() { return; }
        public override void SetVerticesDirty() { return; }
    }
}
