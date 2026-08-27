using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Agava.Wink
{
    public class ImagesCarousel : MonoBehaviour
    {
        private const float OneCycleSeconds = 0.65f;
        private const float PauseSeconds = 1f;

        [SerializeField] private List<CarouselItem> _items;
        [Header("Carousel header")]
        [SerializeField] private CarouselItem _headerItem;
        [SerializeField] private TMP_Text _header;
        [Header("Hide objects")]
        [SerializeField] private int _firstHideObject;
        [SerializeField] private int _lastHideObject;

        private CarouselSettings _carouselSettings;
        private List<CarouselData> _payedAppData;
        private int _assetIndex = 0;
        private Coroutine _cycle;
        private List<CarouselPosition> _carouselPositions = null;
        private int _headerPositionIndex;

        public void Construct(CarouselSettings carouselSettings)
        {
            _carouselSettings = carouselSettings ?? throw new ArgumentNullException(nameof(carouselSettings));
            _payedAppData = new();

            foreach (var data in _carouselSettings.CarouselDatas)
                if(data.AppMonetizationType == AppMonetizationType.PaidApp)
                    _payedAppData.Add(data);

            FillCarouselPositions();
            FillItems();
        }

        public void Enable()
        {
            _cycle = StartCoroutine(EndlessCycle());
        }

        public void Disable()
        {
            if (_cycle != null)
            {
                StopCoroutine(_cycle);
                _cycle = null;
            }
        }

        private IEnumerator EndlessCycle()
        {
            WaitForSeconds waitForCycleEnd = new WaitForSeconds(OneCycleSeconds + PauseSeconds);

            while (true)
            {
                OneCycle();
                yield return waitForCycleEnd;
            }
        }

        private void OneCycle()
        {
            CarouselItem item;
            int targetPositionIndex;
            Action<CarouselItem> onEnd;

            for (int i = 0; i < _items.Count; i++)
            {
                item = _items[i];

                if (item.Index == 0)
                {
                    targetPositionIndex = _carouselPositions.Count - 1;
                    item.Hide();

                    onEnd = (item) =>
                    {
                        item.Show();
                        item.Initialize(NextAsset());
                    };
                }
                else
                {
                    targetPositionIndex = item.Index - 1;
                    onEnd = null;
                }

                if (_headerPositionIndex == targetPositionIndex)
                    item.ShowBorder(OneCycleSeconds);
                else if(targetPositionIndex == _headerPositionIndex - 1)
                    item.HideBorder(OneCycleSeconds);


                    item.SetPositionIndex(targetPositionIndex);

                if (targetPositionIndex == _firstHideObject)
                    item.MakeTransparent(OneCycleSeconds);
                else if(targetPositionIndex == _lastHideObject - 1)
                    item.MakeOpaque(OneCycleSeconds);

                item.OneCycle(_carouselPositions[targetPositionIndex].Position, OneCycleSeconds, onEnd);
            }
        }

        private void FillItems()
        {
            if (_payedAppData.Count == 0)
            {
                Debug.LogError("Fill popup data!");
                return;
            }

            for (int i = 1; i < _items.Count; i++)
            {
                _items[i].Initialize(NextAsset());
            }
        }

        private void FillCarouselPositions()
        {
            CarouselItem item;
            _carouselPositions = new();

            for (int i = 0; i < _items.Count; i++)
            {
                item = _items[i];
                item.Construct();

                if (item == _headerItem)
                {
                    _headerPositionIndex = i;
                    item.ShowBorder(0);
                }

                if (i <= _firstHideObject || i >= _lastHideObject)
                    _items[i].MakeTransparent(0);

                item.SetPositionIndex(i);
                _carouselPositions.Add(new CarouselPosition(item.transform.localPosition));
            }
        }

        private CarouselData NextAsset()
        {
            if (_assetIndex == _payedAppData.Count)
                _assetIndex = 0;

            return _payedAppData[_assetIndex++];
        }

        private struct CarouselPosition
        {
            public Vector3 Position { get; private set; }

            public CarouselPosition(Vector3 position)
            {
                Position = position;
            }
        }
    }
}
