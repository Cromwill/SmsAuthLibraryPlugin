using TMPro;
using UnityEngine;
using SmsAuthAPI.Utility;
using System.Collections.Generic;

namespace Agava.Wink
{
    public class XmlConfigText : MonoBehaviour
    {
        private const string LineTransitionPattern = "end";

        [SerializeField] private TMP_Text _text;
        [SerializeField] private XMLKeys _xMLKey;
        [SerializeField] private XMLValues _xMLValue;
        [SerializeField] private string _fallbackText;

        public void FillText()
        {
            if (SheetRemoteConfigs.Texts != null)
            {
                Dictionary<string, string> data = SheetRemoteConfigs.Texts.Data[_xMLKey.ToString()];

                _text.text = data[_xMLValue.ToString()].Replace($"{{{LineTransitionPattern}}}", "\n");
            }
            else
            {
                _text.text = _fallbackText.Replace($"{{{LineTransitionPattern}}}", "\n");
            }
        }
    }
}
