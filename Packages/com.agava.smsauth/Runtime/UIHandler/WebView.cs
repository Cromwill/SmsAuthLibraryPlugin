using UnityEngine;
using UnityEngine.UI;

public class WebView : MonoBehaviour
{
    [SerializeField] private WebViewObject _webViewObject;
    [SerializeField] private RectTransform _container;
    [SerializeField] private Image _loadingImage;

    public bool Initialized => _webViewObject.IsInitialized();

    private void Start()
    {
        _webViewObject.Init(
            cb: (msg) =>
            {
                Debug.Log(string.Format("CallFromJS[{0}]", msg));
            },
            err: (msg) =>
            {
                Debug.Log(string.Format("CallOnError[{0}]", msg));
            },
            httpErr: (msg) =>
            {
                Debug.Log(string.Format("CallOnHttpError[{0}]", msg));
            },
            started: (msg) =>
            {
                Debug.Log(string.Format("CallOnStarted[{0}]", msg));
            },
            hooked: (msg) =>
            {
                Debug.Log(string.Format("CallOnHooked[{0}]", msg));
            },
            cookies: (msg) =>
            {
                Debug.Log(string.Format("CallOnCookies[{0}]", msg));
            },
            ld: (msg) =>
            {
                OnWebLoad();
                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
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
                var js = "";
#endif
                _webViewObject.EvaluateJS(js + @"Unity.call('ua=' + navigator.userAgent)");
            },
            transparent: false,
            zoom: true,
            ua: "wink game player",
            radius: 22,
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

    private void Update()
    {
        _loadingImage.transform.localEulerAngles += new Vector3(0, 0, 2f);
    }

    public void OpenURL(string url)
    {
        _webViewObject.LoadURL(url.Replace(" ", "%20"));
    }

    public void ShowPage(string cachePagePath)
    {
        _webViewObject.LoadURL("file://" + cachePagePath);
    }

    public void Hide()
    {
        _webViewObject.SetVisibility(false);
    }

    private void OnWebLoad()
    {
        _webViewObject.SetVisibility(true);
    }
}
