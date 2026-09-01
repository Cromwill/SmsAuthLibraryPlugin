using UnityEngine;

namespace Agava.Wink
{
    public class LoadingWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public override void Enable() => EnableCanvasGroup(_canvasGroup);
        public override void Disable() => DisableCanvasGroup(_canvasGroup);
    }
}
