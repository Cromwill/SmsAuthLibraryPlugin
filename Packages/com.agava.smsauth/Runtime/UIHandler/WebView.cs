using System;
using Agava.Wink;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WebView : MonoBehaviour
{
    [SerializeField] private WebViewObject _webViewObject;
    [SerializeField] private RectTransform _container;
    [SerializeField] private Image _loadingImage;
    [SerializeField] private List<string> _balckUrlList;

    private IWebViewLoader _webViewLoader;
    private bool _isLoaded;
    private bool _isOpened;
    private string _targetUrl;

    public bool Initialized => _webViewObject.IsInitialized();

    public event Action<string> WebPageEventReceived;

    private void Awake()
    {
        _loadingImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        Init();
        _webViewObject.SetVisibility(false);
    }

    /*private void Update()
    {
        _loadingImage.transform.localEulerAngles += new Vector3(0, 0, 2f);
    }*/

    public void OpenURL(string url, IWebViewLoader webViewLoader)
    {
#if UNITY_ANDROID
        if (_isLoaded == false)
        {
            _webViewObject.Reload();
            Init();
        }
#endif

        _webViewLoader = webViewLoader;
        _webViewObject.LoadURL(url.Replace(" ", "%20"));
        _targetUrl = url.Replace(" ", "%20");
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
        _webViewObject.Pause();
        _isLoaded = false;
        _isOpened = false;
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

    private void Init()
    {
        _webViewObject.Init(
            cb: (msg) =>
            {
                Debug.Log($"WebView: webview message callback - {msg}!");
                WebPageEventReceived?.Invoke(msg);
            },
            started: (msg) =>
            {
                Debug.Log($"WebView: webview message started - {msg}!");

#if UNITY_ANDROID
                _webViewObject.Resume();
#endif
                if (IsNotAllowedUrl(msg, out string blacklistedUrl))
                {
                    Debug.LogWarning($"WebView: Check black list");

                    if (string.IsNullOrEmpty(_targetUrl) == false)
                    {
                        Debug.LogWarning($"WebView: Redirect and load last url - {_targetUrl}");
                        _webViewObject.LoadURL(_targetUrl);
                        WebViewPresenter.OpenBrowser(blacklistedUrl);
                    }
                }
                else
                {
                    Debug.LogWarning($"WebView: No Redirect");
                }
            },
            ld: (msg) =>
            {
#if UNITY_ANDROID
                if (_isOpened)
                    return;
#endif

                _isOpened = true;

                Debug.Log($"WebView: webview message load - {msg}!");
                OnWebLoad();
                StringBuilder stringBuilder = new StringBuilder();

#if UNITY_IOS
                stringBuilder.Append(@"
                        window.Unity = {
                            call: function(msg) {
                                var iframe = document.createElement('iframe');
                                iframe.setAttribute('src', 'unity:' + msg);
                                document.documentElement.appendChild(iframe);
                                iframe.parentNode.removeChild(iframe);
                                iframe = null;
                            }
                        };");

                stringBuilder.Append(@"window.parent = Unity;");
                stringBuilder.Append(@"window.parent = { postMessage: function (message) { window.Unity.call(JSON.stringify(message)); } };");
#elif UNITY_ANDROID
                stringBuilder.Append("window.AndroidBridge = Unity;");

                stringBuilder.Append(@"
                    window.AndroidBridge = {
                            sendMessage: function(message) {
                               window.Unity.call(message);
                               }
                        }
                ");
#endif

                _webViewObject.EvaluateJS(stringBuilder.ToString());
            },
            transparent: false,
            zoom: true,
            radius: 0,
            androidForceDarkMode: 0,
            enableWKWebView: true,
            wkContentMode: 0,
            wkAllowsLinkPreview: true,
            separated: false
            );

        Setsettings();
        _isLoaded = true;
    }

    private bool IsNotAllowedUrl(string msg, out string blackListedUrl)
    {
        blackListedUrl = null;
        bool isNotAllowed = false;

        foreach (string url in _balckUrlList)
        {
            blackListedUrl = url;

            if (msg.Contains(url))
            {
                Application.OpenURL(msg);
                isNotAllowed = true;
            }
            else
            {
                isNotAllowed = false;
            }
        }

        return isNotAllowed;
    }

    private void Setsettings()
    {
        int left = Mathf.CeilToInt(_container.offsetMin.x);
        int right = Mathf.CeilToInt(-_container.offsetMax.x);
        int top = Mathf.CeilToInt(-_container.offsetMax.y);
        int bottom = Mathf.CeilToInt(_container.offsetMin.y);

        _webViewObject.SetScrollbarsVisibility(false);
        _webViewObject.SetMargins(left, top, right, bottom);
        _webViewObject.SetTextZoom(100);
    }
}
