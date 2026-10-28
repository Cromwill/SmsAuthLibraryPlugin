using Agava.Wink;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private PaywallVariants _paywallVariant = PaywallVariants.FirstCustomPlace;

    private void Start()
    {
        WinkSignInHandlerUI.Instance.TryShowPaywall(_paywallVariant);
    }
}
