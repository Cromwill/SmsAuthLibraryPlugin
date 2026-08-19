using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Preserve]
    internal class RedirectWindowPresenter : WindowPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;

        public void FillRemoteTexts() => _xmlConfigTexts.ForEach(t => t.FillText());

        public override void Enable() => EnableCanvasGroup(_canvasGroup);

        public override void Disable() => DisableCanvasGroup(_canvasGroup);
    }
}
