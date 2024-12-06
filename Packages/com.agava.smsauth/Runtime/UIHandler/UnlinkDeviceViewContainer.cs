using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Agava.Wink
{
    public class UnlinkDeviceViewContainer : MonoBehaviour
    {
        [SerializeField] private UnlinkDeviceView _unlinkDeviceViewTemplate;

        private List<UnlinkDeviceView> _unlinkDeviceViews = new();

        public int Count => _unlinkDeviceViews.Count;

        public event Action<UnlinkDeviceView> Closed;

        public void Add(string device)
        {
            UnlinkDeviceView unlinkDeviceView = Instantiate(_unlinkDeviceViewTemplate, transform);
            unlinkDeviceView.Initialize(device);
            _unlinkDeviceViews.Add(unlinkDeviceView);
            unlinkDeviceView.SetNumber(Count);
            unlinkDeviceView.Closed += OnUnlinked;
        }

        public void Clear()
        {
            while (Count > 0)
            {
                DestroyView(_unlinkDeviceViews.First());
            }
        }

        private void OnUnlinked(UnlinkDeviceView unlinkDeviceView)
        {
            Closed?.Invoke(unlinkDeviceView);
            DestroyView(unlinkDeviceView);
        }

        private void DestroyView(UnlinkDeviceView unlinkDeviceView)
        {
            _unlinkDeviceViews.Remove(unlinkDeviceView);
            unlinkDeviceView.Closed -= OnUnlinked;
            Destroy(unlinkDeviceView.gameObject);

            foreach (UnlinkDeviceView view in _unlinkDeviceViews)
                view.SetNumber(view.Number - 1);
        }
    }
}
