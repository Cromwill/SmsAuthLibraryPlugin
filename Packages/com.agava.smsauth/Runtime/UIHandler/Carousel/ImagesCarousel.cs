using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Agava.Wink
{
    public class ImagesCarousel : MonoBehaviour
    {
        private const float OneCycleSeconds = 1f;

        [SerializeField] private List<CarouselItem> _items;
        [SerializeField] private CarouselItem _leftHiddenItem;
        [SerializeField] private CarouselItem _rightHiddenItem;
        [SerializeField] private List<Sprite> _sprites;

        int _spriteIndex = 0;
        private Coroutine _cycle;

        private void Awake()
        {
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
            }
        }

        private IEnumerator EndlessCycle()
        {
            while (true)
            {
                yield return OneCycle();
            }
        }

        private IEnumerator OneCycle()
        {
            CarouselItem item;
            CarouselItem targetItem;

            for (int i = 0; i < _items.Count; i++)
            {
                item = _items[i];

                if (i == 0)
                {
                    targetItem = _rightHiddenItem;
                    item.Hide();
                }
                else
                {
                    targetItem = _items[i - 1];
                }

                item.OneCycle(targetItem.Position, targetItem.Scale, OneCycleSeconds);
            }

            yield return new WaitForSeconds(OneCycleSeconds);

            _items.Add(_items[0]);
            _items.RemoveAt(0);
            _rightHiddenItem = _items.Last();
            _rightHiddenItem.Show();
            _rightHiddenItem.SetSprite(NextSprite());
        }

        private void FillItems()
        {
            if (_sprites.Count == 0)
            {
                Debug.LogError("Fill sprites!");
                return;
            }

            for (int i = 1; i < _items.Count; i++)
            {
                _items[i].SetSprite(NextSprite());
            }
        }

        private Sprite NextSprite()
        {
            if (_spriteIndex == _sprites.Count)
                _spriteIndex = 0;

            return _sprites[_spriteIndex++];
        }
    }
}
