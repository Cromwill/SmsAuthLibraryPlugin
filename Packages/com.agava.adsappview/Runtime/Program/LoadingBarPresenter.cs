using System.Collections;
using System.Collections.Generic;
using AdsAppView.Utility;
using Codice.Client.BaseCommands;
using TMPro;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace AdsAppView.Program
{
    [Preserve]
    public class LoadingBarPresenter : MonoBehaviour
    {
        [SerializeField] private XMLKeys _xMLKey;
        [SerializeField] private RectTransform _portraitPopup;
        [SerializeField] private RectTransform _landscapePopup;
        [SerializeField] private Image _portraitFill;
        [SerializeField] private Image _landscapeFill;
        [SerializeField] private List<TMP_Text> _portraitTexts;
        [SerializeField] private List<TMP_Text> _landscapeTexts;
        [SerializeField] private List<XmlConfigText> _xmlConfigTexts;
        [SerializeField] private List<ClickableSupportText> _clickableSupportTexts;
        [SerializeField, Min(0.1f)] private float _changeTextDelay;
        [SerializeField, Min(1)] private float _forceLoadTime;
        [SerializeField] private List<string> _defaultTexts;

        private RectTransform _loadingPopup;
        private Image _fill;
        private List<TMP_Text> _texts = new();
        private List<string> _remoteTexts = new();
        private Coroutine _changeTextCoroutine;
        private int _max;

        public int CurrentProgress { get; private set; } = 0;
        public bool ForceLoaded { get; private set; } = false;

        public void Construct(AppOrientation appOrientation)
        {
            _loadingPopup = appOrientation == AppOrientation.Landscape ? _landscapePopup : _portraitPopup;
            _fill = appOrientation == AppOrientation.Landscape ? _landscapeFill : _portraitFill;
            _texts = appOrientation == AppOrientation.Landscape ? _landscapeTexts : _portraitTexts;

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
            _loadingPopup.gameObject.SetActive(true);

            _changeTextCoroutine = StartCoroutine(ChangeText());
        }

        public void Deactivate()
        {
            if(_changeTextCoroutine != null)
            {
                StopCoroutine(_changeTextCoroutine);
                _changeTextCoroutine = null;
            }

            _loadingPopup.gameObject.SetActive(false);
        }

        public void UpdateProgress(float current, float max)
        {
            float value = Mathf.InverseLerp(max, 0, current);
            _fill.fillAmount = Mathf.Lerp(1, 0, value);
        }

        public void SetMax(int max) => _max = max;

        public void UpdateAdditiveProgress()
        {
            if (_max <= 0)
                return;

            CurrentProgress++;
            float value = Mathf.InverseLerp(_max, 0, CurrentProgress);
            _fill.fillAmount = Mathf.Lerp(1, 0, value);
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

        private IEnumerator ChangeText()
        {
            int iterator = 0;

            while (true)
            {
                yield return new WaitForSeconds(_changeTextDelay);

                _texts.ForEach(t => t.text = _remoteTexts[iterator]);
                iterator++;

                if (iterator == _remoteTexts.Count)
                    iterator = 0;
            }
        }
    }
}
