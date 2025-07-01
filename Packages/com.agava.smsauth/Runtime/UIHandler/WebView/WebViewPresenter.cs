using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections;

namespace Agava.Wink
{
    public class WebViewPresenter : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _button;
        [SerializeField] private WebView _webViewPrefab;

        private static WebViewPresenter instance;
        private static IWebViewLoader _webViewLoader;
        public static Action _webViewClosedAction;
        public static Action _subscriptionPurchasedAction;

        private WebView _webView;

        public bool Initialized { get; private set; } = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }

            Disable();
            StartCoroutine(Initialize());
        }

        public void Construct(IWebViewLoader webViewLoader, Action webViewClosedAction, Action subscriptionPurchasedAction)
        {
            _webViewLoader = webViewLoader;
            _webViewClosedAction = webViewClosedAction;
            _subscriptionPurchasedAction = subscriptionPurchasedAction;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnBackButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnBackButtonClick);
        }

        public void Show()
        {
            if (_webView != null && _webView.Initialized)
                _webView.ShowLastPage();
        }

        public void Hide()
        {
            if (_webView != null && _webView.Initialized)
                _webView.Hide();
        }

        private IEnumerator Initialize()
        {
#if (UNITY_ANDROID || UNITY_IOS) && WEBVIEW
            if (_webViewPrefab == null)
            {
                Debug.LogError("Web view prefab is null!");
            }
            else
            {
                _webView = Instantiate(_webViewPrefab, transform);
                yield return new WaitUntil(() => _webView.Initialized);
            }
#endif

            yield return new WaitUntil(() => Links.Initialized);

            Initialized = true;
        }

        public static void ShowWebView(string url)
        {
#if !UNITY_ANDROID && !UNITY_IOS
            Application.OpenURL(url);
#else

            if (instance == null)
            {
                OpenBrowser(url);
                return;
            }

            if (instance._webView == null)
            {
                OpenBrowser(url);
                return;
            }

            instance.Enable();
            instance._webView.OpenURL(url, _webViewLoader);
            instance._webView.WebPageEventReceived += OnEventReceived;
#endif
        }

        public static void HideWebView()
        {
            if (instance == null)
                return;

            if (instance._webView == null)
                return;

            instance._webView.Hide();
            instance.Disable();
            instance._webView.WebPageEventReceived -= OnEventReceived;
        }

        public static void OpenBrowser(string url)
        {
            Application.OpenURL(url);
        }

        private void Enable()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }

        private void Disable()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }

        private void OnBackButtonClick()
        {
            HideWebView();
        }

        private static void OnEventReceived(string eventName)
        {
            Variants variants = JsonConvert.DeserializeObject<Variants>(eventName);

            if (variants != null)
            {
                if (variants.CheckSubscription())
                {
                    _subscriptionPurchasedAction?.Invoke();
                    AnalyticsWinkService.SendSubscriptionPurchaseWasSuccessful();
                    HideWebView();
                }
                else if (variants.CheckCloseWebView())
                {
                    _webViewClosedAction?.Invoke();
                    AnalyticsWinkService.SendCancelSubscriptionPurchase();
                    HideWebView();
                }
            }
        }
    }

    [Serializable]
    internal class Variants
    {
        private const string VariantsEvent = "variants";
        private const string BuyEvent = "buy";
        private const string WebViewCloseEvent = "close";
        private const string SubscriptionSuccessEvent = "success";

        public string Name;
        public Data Data;

        public bool CheckSubscription() => Name == BuyEvent && Data.Type == SubscriptionSuccessEvent;
        public bool CheckCloseWebView() => Data.Type == WebViewCloseEvent;
    }

    [Serializable]
    internal class Data
    {
        public string Type;
    }
}
