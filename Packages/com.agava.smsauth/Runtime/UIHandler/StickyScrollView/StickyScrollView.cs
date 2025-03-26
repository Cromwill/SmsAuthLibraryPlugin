using UnityEngine;
using UnityEngine.UI;

namespace Agava.Wink
{
    public class StickyScrollView : MonoBehaviour
    {
        [field: SerializeField] public ScrollRect ScrollRect { get; private set; }
        [field: SerializeField] public StickyElement StickyElement { get; private set; }
    }
}
