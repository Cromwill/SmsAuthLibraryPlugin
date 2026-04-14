using UnityEngine;
using System.Collections;

namespace Agava.Wink
{
    internal class ClickableAgreementText : ClickableText
    {
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => Links.Initialized);

            Initialize("agreement", Links.Agreement);
        }
    }
}
