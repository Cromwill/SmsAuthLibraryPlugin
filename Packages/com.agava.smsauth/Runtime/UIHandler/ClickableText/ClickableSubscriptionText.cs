using UnityEngine;
using System.Collections;

namespace Agava.Wink
{
    internal class ClickableSubscriptionText : ClickableText
    {
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => Links.Initialized);

            Initialize("subscription", Links.Subscription, () => AnalyticsWinkService.SendSubscriptionLink());
        }
    }
}
