using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    /// <summary>
    ///     A paywall point in the game. Put it on an object of the level (enabled when the player gets there)
    ///     or call <see cref="Reach"/> from a UnityEvent. Same as calling PaywallService.Reach() from code.
    /// </summary>
    [Preserve]
    public class PaywallTrigger : MonoBehaviour
    {
        [SerializeField] private bool _reachOnEnable = true;

        private void OnEnable()
        {
            if (_reachOnEnable)
                Reach();
        }

        public void Reach() => PaywallService.Reach();
    }
}
