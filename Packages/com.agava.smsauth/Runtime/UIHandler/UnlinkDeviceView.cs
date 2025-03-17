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
        [SerializeField] private TMP_Text _freePlaceText;
        [SerializeField] private Button _closeButton;

        public event Action<UnlinkDeviceView> DeviceRemoved;

        public bool IsEmpty { get; private set; } = true;
        public string DeviceId => _deviceId.text;

        private void OnEnable() => _closeButton.onClick.AddListener(OnRemoveDeviceBtnClicked);

        private void OnDisable() => _closeButton.onClick.RemoveListener(OnRemoveDeviceBtnClicked);

        public void Initialize(string deviceId)
        {
            _deviceId.text = deviceId;
            SetEmpty(true);
        }

        public void SetNumber(int number) => _number.text = number.ToString();

        private void OnRemoveDeviceBtnClicked()
        {
            DeviceRemoved?.Invoke(this);
            SetEmpty(false);
        }

        private void SetEmpty(bool empty)
        {
            _deviceId.gameObject.SetActive(empty);
            _closeButton.gameObject.SetActive(empty);
            _freePlaceText.gameObject.SetActive(empty == false);
            IsEmpty = empty;
        }
    }
}
