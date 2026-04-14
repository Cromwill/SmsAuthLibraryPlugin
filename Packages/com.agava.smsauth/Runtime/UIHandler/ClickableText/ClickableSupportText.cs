using UnityEngine;
using System.Collections;

namespace Agava.Wink
{
    internal class ClickableSupportText : ClickableText
    {
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => Links.Initialized);

            Initialize("support", Links.Support, () => AnalyticsWinkService.SendSupportLink());
        }
    }
}
