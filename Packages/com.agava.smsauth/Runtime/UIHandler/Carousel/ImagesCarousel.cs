using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Agava.Wink
{
    public class ImagesCarousel : MonoBehaviour
    {
        private const float OneCycleSeconds = 1f;

        [SerializeField] private List<CarouselItem> _items;
        [SerializeField] private CarouselItem _leftHiddenItem;
        [SerializeField] private CarouselItem _rightHiddenItem;
        [SerializeField] private List<CarouselItemAsset> _assets;
        [SerializeField] private TMP_Text _description;

        int assetIndex = 0;
        private Coroutine _cycle;
        private List<CarouselPosition> _carouselPositions = null;

        private bool _enabled => _cycle != null;

        private void Awake()
        {
            FillCarouselPositions();
            FillItems();
        }

        private void Update()
        {
            if (_enabled)
            {
                if (_description != null)
                {
                    _description.text = Lean.Localization.LeanLocalization.GetTranslationText(_assets[4].Description);
                }
            }
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
            while (true)
            {
                OneCycle();
                yield return new WaitForSeconds(OneCycleSeconds);
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
                        item.SetSprite(NextAsset().Sprite);
                    };
                }
                else
                {
                    targetPositionIndex = item.Index - 1;
                    onEnd = null;
                }


                item.SetPositionIndex(targetPositionIndex);
                item.OneCycle(_carouselPositions[targetPositionIndex].Position, _carouselPositions[targetPositionIndex].Scale, OneCycleSeconds, onEnd);
            }
        }

        private void FillItems()
        {
            if (_assets.Count == 0)
            {
                Debug.LogError("Fill sprites!");
                return;
            }

            for (int i = 1; i < _items.Count; i++)
            {
                _items[i].SetSprite(NextAsset().Sprite);
            }
        }

        private void FillCarouselPositions()
        {
            CarouselItem item;
            _carouselPositions = new();

            for (int i = 0; i < _items.Count; i++)
            {
                item = _items[i];
                item.SetPositionIndex(i);
                _carouselPositions.Add(new CarouselPosition(item.transform.localPosition, item.transform.localScale));
            }
        }

        private CarouselItemAsset NextAsset()
        {
            if (assetIndex == _assets.Count)
                assetIndex = 0;

            return _assets[assetIndex++];
        }

        private struct CarouselPosition
        {
            public Vector3 Position { get; private set; }
            public Vector3 Scale { get; private set; }

            public CarouselPosition(Vector3 position, Vector3 scale)
            {
                Position = position;
                Scale = scale;
            }
        }
    }
}
