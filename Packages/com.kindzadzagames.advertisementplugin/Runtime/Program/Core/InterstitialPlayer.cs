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
        private InterstitialHandler _interstitialHandler;
        private AdsSdkSettingsData _settingsData;
        private bool _vip;
        private float _showInterval;
        private float _elapsedTime = 0;
        private Coroutine _coroutine;

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

        private void OnInterstitialClosed()
        {
            _showInterval = _settingsData.regular_timer;
            _elapsedTime = 0;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(CountdownTime());
        }

        private IEnumerator CountdownTime()
        {
            while (_elapsedTime < _showInterval)
            {
                _elapsedTime += Time.deltaTime;

                yield return null;
            }

            _interstitialHandler.Show();
            _coroutine = null;
        }
    }
}
