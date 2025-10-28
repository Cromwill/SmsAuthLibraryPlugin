using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Scripting;
using KinDzaDzaGames.AdvertisementPlugin.DTO;

namespace KinDzaDzaGames.AdvertisementPlugin
{
    [Preserve]
    public class InterstitialPlayer : MonoBehaviour
    {
        private const int OpenAdOfferCount = 3;
        private const int FirstOpenAdOffer = 1;

        private InterstitialHandler _interstitialHandler;
        private AdsSdkSettingsData _settingsData;
        private bool _vip;
        private float _showInterval;
        private float _elapsedTime = 0;
        private Coroutine _coroutine;
        private bool _paused;
        private int _adOfferCount = 0;
        private int _actualAdOfferOpener = FirstOpenAdOffer;

        public event Action OpenAdOffer;

        public void Construct(InterstitialHandler interstitialHandler, AdsSdkSettingsData settingsData, bool vip)
        {
            _interstitialHandler = interstitialHandler ?? throw new ArgumentNullException(nameof(interstitialHandler));
            _settingsData = settingsData ?? throw new ArgumentNullException(nameof(settingsData));
            _vip = vip;

            _showInterval = _settingsData.first_timer;

            _interstitialHandler.InterstitialClosed += OnInterstitialClosed;
        }

        public void Dispose()
        {
            _interstitialHandler.InterstitialClosed -= OnInterstitialClosed;

            if(_coroutine != null)
                StopCoroutine(_coroutine);
        }

        public void ChangeSubscribeStatus(bool vip)
        {
            _vip = vip;

            StartTimer();
        }

        public void StartTimer()
        {
            if (_vip)
            {
                if (_coroutine != null)
                    StopCoroutine(_coroutine);

                _coroutine = null;
                return;
            }

            _coroutine ??= StartCoroutine(CountdownTime());
        }

        public void Suspend() => _paused = true;
        public void Continue() => _paused = false;

        private void OnInterstitialClosed()
        {
            _showInterval = _settingsData.regular_timer;
            _elapsedTime = 0;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _adOfferCount++;

            if (_adOfferCount >= _actualAdOfferOpener)
            {
                _adOfferCount = 0;
                _actualAdOfferOpener = OpenAdOfferCount;
                OpenAdOffer?.Invoke();
            }

            _coroutine = StartCoroutine(CountdownTime());
        }

        private IEnumerator CountdownTime()
        {
            while (_elapsedTime < _showInterval)
            {
                if(_paused == false)
                    _elapsedTime += Time.deltaTime;

                Debug.Log($"Interstitial Player: elapsed time {_elapsedTime}, pause = {_paused}");
                yield return null;
            }

            _interstitialHandler.Show();
            _coroutine = null;
        }
    }
}
