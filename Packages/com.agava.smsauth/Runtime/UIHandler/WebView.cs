using System;
using System.Collections;
using Agava.Wink;
using UnityEngine;
using UnityEngine.UI;

public class WebView : MonoBehaviour
{
    [SerializeField] private WebViewObject _webViewObject;
    [SerializeField] private RectTransform _container;
    [SerializeField] private Image _loadingImage;

    private IWebViewLoader _webViewLoader;

    public bool Initialized => _webViewObject.IsInitialized();

    public event Action<string> WebPageEventReceived;

    private void Awake()
    {
        _loadingImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        _webViewObject.Init(
            cb: (msg) =>
            {
                WebPageEventReceived?.Invoke(msg);
            },
            err: (msg) =>
            {
                Debug.Log(msg);
            },
            ld: (msg) =>
            {
                OnWebLoad();

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS
                // NOTE: the following js definition is required only for UIWebView; if
                // enabledWKWebView is true and runtime has WKWebView, Unity.call is defined
                // directly by the native plugin.
#if true
                var js = @"
                    if (!(window.webkit && window.webkit.messageHandlers)) {
                        window.Unity = {
                            call: function(msg) {
                                window.location = 'unity:' + msg;
                            }
                        };
                    }
                ";
#else
                // NOTE: depending on the situation, you might prefer this 'iframe' approach.
                // cf. https://github.com/gree/unity-webview/issues/189
                var js = @"
                    if (!(window.webkit && window.webkit.messageHandlers)) {
                        window.Unity = {
                            call: function(msg) {
                                var iframe = document.createElement('IFRAME');
                                iframe.setAttribute('src', 'unity:' + msg);
                                document.documentElement.appendChild(iframe);
                                iframe.parentNode.removeChild(iframe);
                                iframe = null;
                            }
                        };
                    }
                ";
#endif
#else
                var js = @"";
#endif
                _webViewObject.EvaluateJS(js + "window.AndroidBridge = Unity;");
                _webViewObject.EvaluateJS(js + "window.AndroidBridge.addEventListener(\"close\", (e) => Unity.call(e.data));");
                _webViewObject.EvaluateJS(js + "window.AndroidBridge.addEventListener(\"success\", (e) => Unity.call(e.data));");
                _webViewObject.EvaluateJS(js + "window.AndroidBridge.addEventListener(\"buy\", (e) => Unity.call(e.data));");
                _webViewObject.EvaluateJS(js + "window.AndroidBridge.addEventListener(\"variants\", (e) => Unity.call(e.data));");

            },
            transparent: false,
            zoom: true,
            ua: "wink game player",
            radius: 0,
            androidForceDarkMode: 0,
            enableWKWebView: true,
            wkContentMode: 0,
            wkAllowsLinkPreview: true,
            separated: false
            );

        int left = Mathf.CeilToInt(_container.offsetMin.x);
        int right = Mathf.CeilToInt(-_container.offsetMax.x);
        int top = Mathf.CeilToInt(-_container.offsetMax.y);
        int bottom = Mathf.CeilToInt(_container.offsetMin.y);

        _webViewObject.SetScrollbarsVisibility(false);
        _webViewObject.SetMargins(left, top, right, bottom);
        _webViewObject.SetTextZoom(100);
        _webViewObject.SetVisibility(false);
    }

    /*private void Update()
    {
        _loadingImage.transform.localEulerAngles += new Vector3(0, 0, 2f);
    }*/

    public void OpenURL(string url, IWebViewLoader webViewLoader)
    {
        _webViewLoader = webViewLoader;
        _webViewObject.LoadURL(url.Replace(" ", "%20"));
    }

    public void ShowPage(string cachePagePath)
    {
        _webViewObject.LoadURL("file://" + cachePagePath);
    }

    public void ShowLastPage()
    {
        _webViewObject.SetVisibility(true);
    }

    public void Hide()
    {
        _webViewObject.SetVisibility(false);
    }

    private void OnWebLoad()
    {
        StartCoroutine(Open());

        IEnumerator Open()
        {
            yield return new WaitUntil(() => _webViewLoader.Loaded);
            _webViewObject.SetVisibility(true);
        }
    }
}
