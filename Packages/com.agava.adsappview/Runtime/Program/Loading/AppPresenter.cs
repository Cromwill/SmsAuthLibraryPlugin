using TMPro;
using UnityEngine;

namespace AdsAppView.Program
{
    public class AppPresenter : MonoBehaviour
    {
        [field: SerializeField] public AppAuthenticator AppAuthenticator { get; private set; }

        [field: SerializeField] public TMP_Text AppLabel;

        public void SetAppName(string remoteAppName) => AppLabel.text = remoteAppName;
    }
}
