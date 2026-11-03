using Agava.Wink;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void Start()
    {
        WinkSignInHandlerUI.Instance.TryShowPaywall(PaywallVariants.FirstCustomPlace);
    }
}
