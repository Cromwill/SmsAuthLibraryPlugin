using System;
using System.Collections;
using System.Text;
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
        Init();
    }

    /*private void Update()
    {
        _loadingImage.transform.localEulerAngles += new Vector3(0, 0, 2f);
    }*/

    public void OpenURL(string url, IWebViewLoader webViewLoader)
    {
        Init();
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

    private void Init()
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
                stringBuilder.Append(@"window.parent = { postMessage: function (message) { window.Unity.call(message); } };");
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

        int left = Mathf.CeilToInt(_container.offsetMin.x);
        int right = Mathf.CeilToInt(-_container.offsetMax.x);
        int top = Mathf.CeilToInt(-_container.offsetMax.y);
        int bottom = Mathf.CeilToInt(_container.offsetMin.y);

        _webViewObject.SetScrollbarsVisibility(false);
        _webViewObject.SetMargins(left, top, right, bottom);
        _webViewObject.SetTextZoom(100);
        _webViewObject.SetVisibility(false);
    }
}
