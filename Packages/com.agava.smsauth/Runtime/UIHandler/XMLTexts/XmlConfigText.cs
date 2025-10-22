using TMPro;
using UnityEngine;
using SmsAuthAPI.Utility;
using System.Collections.Generic;

namespace Agava.Wink
{
    public class XmlConfigText : MonoBehaviour
    {
        protected const string LineTransitionPattern = "end";
        protected const string LinkPattern = "<link=\"{0}\">{1}</link>";

        [SerializeField] private TMP_Text _text;
        [SerializeField] private XMLKeys _xMLKey;
        [SerializeField] private XMLValues _xMLValue;
        [SerializeField] private string _fallbackText;
        [SerializeField] private bool _isLink;
        [SerializeField] private string _linkName;

        public virtual void FillText()
        {
            if (SheetRemoteConfigs.Texts != null)
            {
                Dictionary<string, string> data = SheetRemoteConfigs.Texts.Data[_xMLKey.ToString()];

                if(_isLink == false)
                    _text.text = data[_xMLValue.ToString()].Replace($"{{{LineTransitionPattern}}}", "\n");
                else
                    _text.text = string.Format(LinkPattern, _linkName, _text.text = data[_xMLValue.ToString()].Replace($"{{{LineTransitionPattern}}}", "\n"));
            }
            else
            {
                Debug.Log($"XML TEXT: download remote failed, used prepared texts for key = {_xMLKey}, value = {_xMLValue}.");

                if (_isLink == false)
                    _text.text = _fallbackText.Replace($"{{{LineTransitionPattern}}}", "\n");
                else
                    _text.text = string.Format(LinkPattern, _linkName, _fallbackText.Replace($"{{{LineTransitionPattern}}}", "\n"));
            }
        }

        public void ChangeText(string text) => _text.text = text;
    }
}
