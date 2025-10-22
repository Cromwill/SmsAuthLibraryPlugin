using UnityEngine;
using SmsAuthAPI.Utility;
using System.Collections.Generic;

namespace Agava.Wink
{
    public class XmlConfigSelectableText : XmlConfigText
    {
        [Header("123")]
        [SerializeField] private XMLKeys _xMLKeyforADVariant;
        [SerializeField] private XMLValues _xMLValueforADVariant;
        [SerializeField] private string _fallbackTextforADVariant;

        private string _aDVariantText;

        public override void FillText()
        {
            base.FillText();

            if (SheetRemoteConfigs.Texts != null)
            {
                Dictionary<string, string> data = SheetRemoteConfigs.Texts.Data[_xMLKeyforADVariant.ToString()];

                _aDVariantText = data[_xMLValueforADVariant.ToString()].Replace($"{{{LineTransitionPattern}}}", "\n");
            }
            else
            {
                Debug.Log($"XML TEXT: download remote failed, used prepared texts for key = {_xMLKeyforADVariant}, value = {_xMLValueforADVariant}.");

                _aDVariantText = _fallbackTextforADVariant.Replace($"{{{LineTransitionPattern}}}", "\n");
            }
        }

        public void UseAdVariantText() => ChangeText(_aDVariantText);
    }
}
