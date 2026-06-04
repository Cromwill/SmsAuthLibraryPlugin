using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    internal class RedirectWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public override void Enable() => EnableCanvasGroup(_canvasGroup);

        public override void Disable() => DisableCanvasGroup(_canvasGroup);
    }
}
