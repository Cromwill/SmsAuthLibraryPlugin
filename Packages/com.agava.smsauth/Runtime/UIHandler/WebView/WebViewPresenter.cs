using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Agava.Wink
{
    public class WebViewPresenter : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _button;
        [SerializeField] private WebView _webView;

        private static WebViewPresenter instance;

        private Dictionary<string, string> _cacheUrlPages;

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

        private void OnEnable()
        {
            _button.onClick.AddListener(OnBackButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnBackButtonClick);
        }

        private IEnumerator Initialize()
        {
            //if (_webView == null)
            //{
            //    Debug.LogError("Web view is null!");
            //}
            //else
            //{
            //    _cacheUrlPages = new();

            //    yield return new WaitUntil(() => _webView.Initialized);
            yield return new WaitUntil(() => Links.Initialized);
            //    //yield return DownloadPage(Links.Support, Links.SupportRmtKey);
            //    //yield return DownloadPage(Links.Agreement, Links.AgreementRmtKey);
            //    //yield return DownloadPage(Links.Privacy, Links.PrivacyRmtKey);
            //    //yield return DownloadPage(Links.Subscription, Links.SubscriptionRmtKey);
            //}

            Initialized = true;
            yield return null;
        }

        public static void ShowWebView(string url)
        {
            Application.OpenURL(url);
            return;

            if (instance == null)
            {
                OpenURL(url);
                return;
            }

            if (instance._webView == null)
            {
                OpenURL(url);
                return;
            }

            instance.Enable();

            if (instance._cacheUrlPages.TryGetValue(url, out string path))
            {
                instance._webView.ShowPage(path);
            }
            else
            {
                instance._webView.OpenURL(url);
            }
        }

        public static void HideWebView()
        {
            if (instance == null)
                return;

            if (instance._webView == null)
                return;

            instance._webView.Hide();
            instance.Disable();
        }

        //private IEnumerator DownloadPage(string url, string pageName)
        //{
        //    byte[] bytes = null;

        //    using (UnityWebRequest request = UnityWebRequest.Get(url))
        //    {
        //        yield return request.SendWebRequest();
        //        bytes = request.downloadHandler.data;
        //    }

        //    if (bytes != null)
        //    {
        //        if (TryCacheBytes(bytes, pageName, out string cachePagePath))
        //        {
        //            _cacheUrlPages[url] = cachePagePath;
        //        }
        //    }

        //    yield return null;
        //}

        //private bool TryCacheBytes(byte[] pageBytes, string pageName, out string cachePagePath)
        //{
        //    cachePagePath = Path.Join(Application.temporaryCachePath, pageName + ".html");
        //    Debug.Log($"Cache path for {pageName}: {cachePagePath}");

        //    try
        //    {
        //        File.WriteAllBytes(cachePagePath, pageBytes);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Log(ex.Message);
        //        return false;
        //    }
        //}

        private static void OpenURL(string url)
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
    }
}
