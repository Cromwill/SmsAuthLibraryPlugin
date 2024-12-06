using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agava.Wink
{
    public class UnlinkDeviceView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _number;
        [SerializeField] private TMP_Text _deviceId;
        [SerializeField] private Button _closeButton;

        public event Action<UnlinkDeviceView> Closed;

        public string DeviceId => _deviceId.text;
        public int Number { get; private set; } = 0;

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }

        public void Initialize(string deviceId)
        {
            _deviceId.text = deviceId;
        }

        public void SetNumber(int number)
        {
            Number = number;
            _number.text = number.ToString();
        }

        private void OnCloseButtonClicked()
        {
            Closed?.Invoke(this);
        }
    }
}
