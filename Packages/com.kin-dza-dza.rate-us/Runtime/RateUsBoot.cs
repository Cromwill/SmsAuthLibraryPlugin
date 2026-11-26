using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

namespace KinDzaDza.RateUs
{
    public class RateUsBoot : MonoBehaviour
    {
        [SerializeField] private RateUsPopup _rateUsPopup;
        [SerializeField] private RateUsPopupTimer _rateUsPopupTimer;
        [SerializeField] private Store _store;
        [SerializeField] private string _serverPath = "gamesrtkbd.tech";

        private RateUsConfigProvider _rateUsConfigProvider;

        private bool IsRateUsShown() => PlayerPrefs.GetInt(Constants.RateUsShownKey, Constants.FalseValueInt) == Constants.TrueValueInt;

        private IEnumerator Start()
        {
            if (!IsRateUsShown())
            {
                _rateUsConfigProvider = new RateUsConfigProvider(_store, _serverPath);

                Task<RateUsConfig> getConfigTask = _rateUsConfigProvider.GetConfig();
                yield return new WaitUntil(() => getConfigTask.IsCompleted);

                if (getConfigTask.IsFaulted || getConfigTask.Result == null || string.IsNullOrEmpty(getConfigTask.Result.StoreLink))
                {
                    Debug.LogException(getConfigTask.Exception);
                    yield break;
                }

                RateUsConfig config = getConfigTask.Result;

                yield return SheetRemoteConfigs.Initialize(_rateUsConfigProvider);

                _rateUsPopup.Construct(config.StoreLink, _store, config.TriggerTimeSeconds);
            }
        }
    }
}
