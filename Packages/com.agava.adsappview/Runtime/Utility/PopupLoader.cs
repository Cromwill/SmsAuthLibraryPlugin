using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AdsAppView.Program;
using UnityEngine.SceneManagement;

namespace AdsAppView.Utility
{
    public class PopupLoader : MonoBehaviour
    {
        [SerializeField] private Boot _boot;
        [SerializeField] private string _startSceneName;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => _boot.Constructed);

            SceneManager.LoadSceneAsync(_startSceneName);
        }
    }
}
