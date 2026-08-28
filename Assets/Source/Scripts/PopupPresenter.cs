using System;
using UnityEngine;
using UnityEngine.UI;


public class PopupPresenter : MonoBehaviour
{
    [SerializeField] private AspectRatioFitter _logoRatioFitter;
    [SerializeField] private Image _background;
    [SerializeField] private Image _logo;
    [SerializeField] private Image _button;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _confirmButton;

    public event Action CloseButtonClicked;

    public void Show(PopupData popupData)
    {
        _logoRatioFitter.aspectRatio = (float)popupData.Logo.texture.width / popupData.Logo.texture.height;
        _background.sprite = popupData.Background;
        _logo.sprite = popupData.Logo;
        _button.sprite = popupData.Button;

        _closeButton.onClick.AddListener(ClosePopup);
        _confirmButton.onClick.AddListener(ClosePopup);
    }

    private void ClosePopup()
    {
        _closeButton.onClick.RemoveListener(ClosePopup);
        _confirmButton.onClick.RemoveListener(ClosePopup);

        CloseButtonClicked?.Invoke();
    }
}
