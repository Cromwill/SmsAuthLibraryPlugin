using UnityEngine;

namespace Agava.Wink
{
    public class PhoneService : MonoBehaviour
    {
        [SerializeField] private WinkAccessManager _winkAccessManager;
        [Header("Placeholders")]
        [SerializeField] private TextPlaceholder[] _phoneNumberPlaceholders;

        public void SetPhone()
        {
            string number = "N/A";

            if (PlayerPrefs.HasKey(_winkAccessManager.PhoneNumber))
            {
                number = PhoneNumber.FormatNumber(PlayerPrefs.GetString(_winkAccessManager.PhoneNumber));
            }

            for (int i = 0; i < _phoneNumberPlaceholders.Length; i++)
            {
                _phoneNumberPlaceholders[i].ReplaceValue(number);
            }
        }
    }
}
