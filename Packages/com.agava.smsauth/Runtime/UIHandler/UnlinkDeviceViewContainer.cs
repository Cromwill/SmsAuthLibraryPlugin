using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Agava.Wink
{
    public class UnlinkDeviceViewContainer : MonoBehaviour
    {
        private const int MaxCount = 5;

        [SerializeField] private UnlinkDeviceView _unlinkDeviceViewTemplate;
        [SerializeField] private TMP_Text _freeDeviceLabelText;

        private List<UnlinkDeviceView> _unlinkDeviceViews = new();
        private int _freeCount = 0;

        private Dictionary<int, string> _emptyDevicesTexts = new Dictionary<int, string>()
        {
            {0, "Свободно 5 мест" },
            {1, "Свободно 4 места" },
            {2, "Свободно 3 места" },
            {3, "Свободно 2 места" },
            {4, "Свободно 1 место" },
            {5, "Свободных мест нет" },
        };

        public int Count => _unlinkDeviceViews.Count;
        public bool HasFreePlaces => _unlinkDeviceViews.Any(view => view.IsEmpty);

        public event Action<UnlinkDeviceView> DeviceRemoved;

        public void Initialize(IReadOnlyList<string> devicesList)
        {
            for (int i = 0; i < MaxCount; i++)
            {
                UnlinkDeviceView unlinkDeviceView = Instantiate(_unlinkDeviceViewTemplate, transform);
                _unlinkDeviceViews.Add(unlinkDeviceView);
                unlinkDeviceView.SetNumber(i + 1);

                if (i < devicesList.Count)
                {
                    unlinkDeviceView.Initialize(devicesList[i], i + 1);
                    unlinkDeviceView.DeviceRemoved += OnUnlinked;
                }
            }

            _freeCount = MaxCount;
            _freeDeviceLabelText.text = _emptyDevicesTexts[_freeCount];
        }

        public void Clear()
        {
            while (Count > 0)
                DestroyView(_unlinkDeviceViews.First());
        }

        private void OnUnlinked(UnlinkDeviceView unlinkDeviceView)
        {
            DeviceRemoved?.Invoke(unlinkDeviceView);

            _freeCount--;
            _freeDeviceLabelText.text = _emptyDevicesTexts[_freeCount];
        }

        private void DestroyView(UnlinkDeviceView unlinkDeviceView)
        {
            _unlinkDeviceViews.Remove(unlinkDeviceView);
            unlinkDeviceView.DeviceRemoved -= OnUnlinked;
            Destroy(unlinkDeviceView.gameObject);
        }
    }
}
