using UnityEngine;
using SmsAuthAPI.Utility;

namespace Agava.Wink
{
    public class XmlConfigCompositeText : XmlConfigText
    {
        [SerializeField] private string _textPattern = "{0}, {1}";
        [SerializeField] private XMLKeys _lineEndXMLKey;
        [SerializeField] private XMLValues _lineEndXMLValue;
        [SerializeField] private string _lineEndFallbackText;

        public override void FillText()
        {
            if (SheetRemoteConfigs.Texts != null)
            {
                Text.text = string.Format(_textPattern, SheetRemoteConfigs.Texts.Data[XMLKeys.ToString()][XMLValues.ToString()], SheetRemoteConfigs.Texts.Data[_lineEndXMLKey.ToString()][_lineEndXMLValue.ToString()]);
            }
            else
            {
                Debug.Log($"XML TEXT: download xml failed, used prepared texts for key = {XMLKeys} & {_lineEndXMLKey}, value = {XMLValues} & {_lineEndXMLValue}.");

                Text.text = string.Format(_textPattern, FallbackText, _lineEndFallbackText);
            }
        }
    }
}
