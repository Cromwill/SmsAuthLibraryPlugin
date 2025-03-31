using System;
using UnityEngine;
using DG.Tweening;

namespace Agava.Wink
{
    [Serializable]
    public class OrientationСhangeAnimation
    {
        private const float ShortAnimationTime = 0.5f;
        private const float LongAnimationTime = 1f;
        private const float LongArrowDistance = 170f;
        private const float MiddleArrowDistance = 125f;
        private const float ShortArrowDistance = 105f;
        private const float PhoneRotationAngle = 90f;

        [SerializeField] private RectTransform _phoneRT;
        [SerializeField] private RectTransform _arrowsRT;
        [SerializeField] private RectTransform _leftArrowRT;
        [SerializeField] private RectTransform _rightArrowRT;

        private Sequence _sequence;
        private Quaternion _phoneQuaternion;
        private Quaternion _arrowsQuaternion;
        private Vector3 _leftArrowPosition;
        private Vector3 _rightArrowPosition;

        public void Construct()
        {
            _phoneQuaternion = _phoneRT.localRotation;
            _arrowsQuaternion = _arrowsRT.localRotation;
            _leftArrowPosition = _leftArrowRT.localPosition;
            _rightArrowPosition = _rightArrowRT.localPosition;
        }

        public void StartAnimation()
        {
            _sequence = DOTween.Sequence();

            _sequence
                .Append(_leftArrowRT.DOLocalMoveX(-LongArrowDistance, ShortAnimationTime))
                .Join(_rightArrowRT.DOLocalMoveX(LongArrowDistance, ShortAnimationTime))
                .Append(_phoneRT.DOLocalRotate(new Vector3(0, 0, PhoneRotationAngle), LongAnimationTime))
                .Append(_leftArrowRT.DOLocalMoveX(-MiddleArrowDistance, ShortAnimationTime))
                .Join(_rightArrowRT.DOLocalMoveX(MiddleArrowDistance, ShortAnimationTime))
                .AppendInterval(ShortAnimationTime)
                .Append(_leftArrowRT.DOLocalMoveX(-LongArrowDistance, ShortAnimationTime))
                .Join(_rightArrowRT.DOLocalMoveX(LongArrowDistance, ShortAnimationTime))
                .Append(_phoneRT.DOLocalRotate(Vector3.zero, LongAnimationTime))
                .Append(_leftArrowRT.DOLocalMoveX(-ShortArrowDistance, ShortAnimationTime))
                .Join(_rightArrowRT.DOLocalMoveX(ShortArrowDistance, ShortAnimationTime))
                .SetDelay(ShortAnimationTime).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }

        public void StopAnimation()
        {
            _sequence.Kill();

            _phoneRT.localRotation = _phoneQuaternion;
            _arrowsRT.localRotation = _arrowsQuaternion;
            _leftArrowRT.localPosition = _leftArrowPosition;
            _rightArrowRT.localPosition = _rightArrowPosition;
        }
    }
}
