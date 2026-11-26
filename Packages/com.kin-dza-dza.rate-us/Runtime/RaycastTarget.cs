using UnityEngine.UI;
using UnityEngine.Scripting;

namespace KinDzaDza.RateUs
{
    [Preserve]
    public class RaycastTarget : Graphic
    {
        public override void SetMaterialDirty() { return; }
        public override void SetVerticesDirty() { return; }
    }
}
