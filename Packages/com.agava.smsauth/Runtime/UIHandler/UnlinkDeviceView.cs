using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Agava.Wink
{
    public class UnlinkDeviceView : MonoBehaviour
    {
        private const int MaxCount = 5;
        private const string DevicePattern = "Устройство {0}";

        [SerializeField] private TMP_Text _number;
        [SerializeField] private TMP_Text _deviceNumber;
        [SerializeField] private TMP_Text _freePlaceText;
        [SerializeField] private Image _bottomBorder;
        [SerializeField] private Button _closeButton;

        private string _deviceId;

        public event Action<UnlinkDeviceView> DeviceRemoved;

        public bool IsEmpty { get; private set; } = true;
        public string DeviceId => _deviceId;

        private void OnEnable() => _closeButton.onClick.AddListener(OnRemoveDeviceBtnClicked);

        private void OnDisable() => _closeButton.onClick.RemoveListener(OnRemoveDeviceBtnClicked);

        public void Initialize(string deviceId, int index)
        {
            _deviceId = deviceId;
            _deviceNumber.text = string.Format(DevicePattern, index);
            SetEmpty(true);

            if(index == MaxCount)
                _bottomBorder.gameObject.SetActive(false);
        }

        public void SetNumber(int number) => _number.text = number.ToString();

        private void OnRemoveDeviceBtnClicked()
        {
            DeviceRemoved?.Invoke(this);
            SetEmpty(false);
        }

        private void SetEmpty(bool empty)
        {
            _deviceNumber.gameObject.SetActive(empty);
            _closeButton.gameObject.SetActive(empty);
            _freePlaceText.gameObject.SetActive(empty == false);
            IsEmpty = empty == false;
        }
    }
}
