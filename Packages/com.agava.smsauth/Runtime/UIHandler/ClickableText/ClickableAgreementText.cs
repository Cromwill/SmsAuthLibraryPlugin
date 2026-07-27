using UnityEngine;
using System.Collections;

namespace Agava.Wink
{
    internal class ClickableAgreementText : ClickableText
    {
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => Links.Instance != null);
            yield return new WaitUntil(() => Links.Instance.Initialized);

            Initialize("agreement", Links.Instance.Agreement);
        }
    }
}
