using UnityEngine;
using System.Collections;

namespace Agava.Wink
{
    internal class ClickableSupportText : ClickableText
    {
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => Links.Instance != null);
            yield return new WaitUntil(() => Links.Instance.Initialized);

            Initialize("support", Links.Instance.Support, () => AnalyticsWinkService.SendSupportLink());
        }
    }
}
