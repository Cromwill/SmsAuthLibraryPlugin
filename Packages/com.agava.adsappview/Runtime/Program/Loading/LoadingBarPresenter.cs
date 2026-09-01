using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AdsAppView.Utility;
using System.Collections;
using UnityEngine.Scripting;
using System.Collections.Generic;

namespace AdsAppView.Program
{
    [Preserve]
    public class LoadingBarPresenter : MonoBehaviour
    {
        private float TextChangeStartValue = 0.75f;
        private float TextChangeLimit = 0.25f;

        [SerializeField] private XMLKeys _xMLKey;
        [SerializeField] private LoadingPopup _portraitPopup;
        [SerializeField] private LoadingPopup _landscapePopup;
        [SerializeField] private Image _portraitFill;
        [SerializeField] private Image _landscapeFill;
        [SerializeField] private List<TMP_Text> _portraitTexts;
        [SerializeField] private List<TMP_Text> _landscapeTexts;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;
        [SerializeField] private List<ClickableSupportText> _clickableSupportTexts;
        [SerializeField, Min(1)] private float _forceLoadTime;
        [SerializeField] private List<string> _defaultTexts;

        private LoadingPopup _loadingPopup;
        private Image _fill;
        private List<TMP_Text> _texts = new();
        private List<string> _remoteTexts = new();
        private int _max;
        private int _textIterator = 0;
        private float _textChangeValue = 0;

        public int CurrentProgress { get; private set; } = 0;
        public bool ForceLoaded { get; private set; } = false;

        public void Construct(AppOrientation appOrientation, Store storeName)
        {
            _loadingPopup = appOrientation == AppOrientation.Landscape ? _landscapePopup : _portraitPopup;
            _fill = appOrientation == AppOrientation.Landscape ? _landscapeFill : _portraitFill;
            _texts = appOrientation == AppOrientation.Landscape ? _landscapeTexts : _portraitTexts;
            _textChangeValue = TextChangeStartValue;

            if (SheetRemoteConfigs.Texts != null)
            {
                if(SheetRemoteConfigs.Texts.Data.TryGetValue(_xMLKey.ToString(), out Dictionary<string, string> data))
                {
                    _remoteTexts.Clear();
                    _remoteTexts.Add(data[XMLValues.Value1.ToString()]);
                    _remoteTexts.Add(data[XMLValues.Value2.ToString()]);
                    _remoteTexts.Add(data[XMLValues.Value3.ToString()]);
                    _remoteTexts.Add(data[XMLValues.Value4.ToString()]);
                }
                else
                {
                    _remoteTexts = _defaultTexts;
                }

                _loadingPopup.Construct(storeName);
            }
            else
            {
                Debug.Log($"XML TEXT: download remote failed, used prepared texts for key = {_xMLKey}.");

                _remoteTexts = _defaultTexts;
            }
        }

        public void FiiRemoteTexts(string supportLink)
        {
            _xmlConfigTexts.ForEach(t => t.FillText());
            _clickableSupportTexts.ForEach(t => t.Construct(supportLink));
        }

        public void Activate()
        {
            _textChangeValue = TextChangeStartValue;
            _loadingPopup.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            _loadingPopup.gameObject.SetActive(false);
        }

        public void UpdateProgress(float current, float max)
        {
            float value = Mathf.InverseLerp(max, 0, current);
            _fill.fillAmount = Mathf.Lerp(1, 0, value);
            TryChangeText(value);
        }

        public void SetMax(int max) => _max = max;

        public void UpdateAdditiveProgress()
        {
            if (_max <= 0)
                return;

            CurrentProgress++;
            float value = Mathf.InverseLerp(_max, 0, CurrentProgress);
            _fill.fillAmount = Mathf.Lerp(1, 0, value);
            TryChangeText(value);
        }

        public void ForceLoad()
        {
            StartCoroutine(Load());

            IEnumerator Load()
            {
                float t = 0;

                while (t < _forceLoadTime)
                {
                    t += Time.deltaTime;
                    UpdateProgress(t, _forceLoadTime);

                    yield return null;
                }

                _fill.fillAmount = 1;
                ForceLoaded = true;
            }
        }

        private void TryChangeText(float value)
        {
            if(value < _textChangeValue)
            {
                _textChangeValue -= TextChangeLimit;
                _textIterator++;

                if (_textIterator == _remoteTexts.Count)
                    _textIterator = 0;

                _texts.ForEach(t => t.text = _remoteTexts[_textIterator]);
            }
        }
    }
}
