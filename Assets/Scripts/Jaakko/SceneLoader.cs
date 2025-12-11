using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AG3958
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;

        [SerializeField] private CanvasGroup _fadeCanvasGroup;
        [SerializeField] private float _fadeOutDuration = 1f;
        [SerializeField] private float _fadeInDuration = 1f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadSceneWithFade(string sceneName, float delayBeforeFadeOut = 0f)
        {
            StartCoroutine(FadeAndSwitchScene(sceneName, delayBeforeFadeOut));
        }

        public void LoadSceneWithFade(int sceneID, float delayBeforeFadeOut = 0f)
        {
            StartCoroutine(FadeAndSwitchScene(sceneID, delayBeforeFadeOut));
        }

        public void SetFadeDurations(float fadeInDur, float fadeOutDur)
        {
            _fadeInDuration = fadeInDur;
            _fadeOutDuration = fadeOutDur;
        }

        private IEnumerator FadeAndSwitchScene(string sceneName, float delayBeforeFadeOut)
        {
            yield return StartCoroutine(FadeOut());

            if (delayBeforeFadeOut > 0)
                yield return new WaitForSeconds(delayBeforeFadeOut);

            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return StartCoroutine(FadeIn());
        }

        private IEnumerator FadeAndSwitchScene(int sceneID, float delayBeforeFadeOut)
        {
            yield return StartCoroutine(FadeOut());

            if (delayBeforeFadeOut > 0)
                yield return new WaitForSeconds(delayBeforeFadeOut);

            yield return SceneManager.LoadSceneAsync(sceneID);
            yield return StartCoroutine(FadeIn());
        }

        private IEnumerator FadeOut()
        {
            float time = 0f;
            while (time < _fadeOutDuration)
            {
                _fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, time / _fadeOutDuration);
                time += Time.deltaTime;
                yield return null;
            }
            _fadeCanvasGroup.alpha = 1f;
        }

        private IEnumerator FadeIn()
        {
            float time = 0f;
            while (time < _fadeInDuration)
            {
                _fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, time / _fadeInDuration);
                time += Time.deltaTime;
                yield return null;
            }
            _fadeCanvasGroup.alpha = 0f;
        }
    }

}