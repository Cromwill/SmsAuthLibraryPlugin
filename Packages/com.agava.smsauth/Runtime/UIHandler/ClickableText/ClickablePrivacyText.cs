using UnityEngine;
using System.Collections;

namespace Agava.Wink
{
    internal class ClickablePrivacyText : ClickableText
    {
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => Links.Initialized);

            Initialize("policy", Links.Privacy);
        }
    }
}
