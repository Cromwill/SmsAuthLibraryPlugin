using UnityEngine;

namespace Agava.Wink
{
    public class OrientationСhangeWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private OrientationСhangeAnimation _orientationСhangeAnimation;

        public override void Enable() => EnableCanvasGroup(_canvasGroup);

        public override void Disable() => DisableCanvasGroup(_canvasGroup);
    }
}
