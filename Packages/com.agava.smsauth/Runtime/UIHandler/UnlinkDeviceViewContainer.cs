using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Agava.Wink
{
    public class UnlinkDeviceViewContainer : MonoBehaviour
    {
        private const int MaxCount = 5;

        [SerializeField] private UnlinkDeviceView _unlinkDeviceViewTemplate;

        private List<UnlinkDeviceView> _unlinkDeviceViews = new();

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
                    unlinkDeviceView.Initialize(devicesList[i]);
                    unlinkDeviceView.DeviceRemoved += OnUnlinked;
                }
            }
        }

        public void Clear()
        {
            while (Count > 0)
            {
                DestroyView(_unlinkDeviceViews.First());
            }
        }

        private void OnUnlinked(UnlinkDeviceView unlinkDeviceView) => DeviceRemoved?.Invoke(unlinkDeviceView);

        private void DestroyView(UnlinkDeviceView unlinkDeviceView)
        {
            _unlinkDeviceViews.Remove(unlinkDeviceView);
            unlinkDeviceView.DeviceRemoved -= OnUnlinked;
            Destroy(unlinkDeviceView.gameObject);
        }
    }
}
