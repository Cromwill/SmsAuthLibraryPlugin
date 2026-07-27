using UnityEngine;
using System.Collections;

namespace Agava.Wink
{
    internal class ClickableSubscriptionText : ClickableText
    {
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => Links.Instance != null);
            yield return new WaitUntil(() => Links.Instance.Initialized);

            Initialize("subscription", Links.Instance.Subscription, () => AnalyticsWinkService.SendSubscriptionLink());
        }
    }
}
