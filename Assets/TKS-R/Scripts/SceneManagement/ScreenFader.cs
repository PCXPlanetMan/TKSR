using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TKSR
{
    public class ScreenFader : MonoBehaviour
    {
        public enum FadeType
        {
            Black, Loading, GameOver,
        }
        
        public static ScreenFader Instance
        {
            get
            {
                if (s_Instance != null)
                    return s_Instance;

                s_Instance = FindFirstObjectByType<ScreenFader> ();

                if (s_Instance != null)
                    return s_Instance;

                Create ();

                return s_Instance;
            }
        }

        public static bool IsFading
        {
            get { return Instance.m_IsFading; }
        }

        protected static ScreenFader s_Instance;

        public static void Create ()
        {
            ScreenFader controllerPrefab = Resources.Load<ScreenFader> ("ScreenFader");
            s_Instance = Instantiate (controllerPrefab);
        }


        public CanvasGroup faderCanvasGroup;
        public CanvasGroup loadingCanvasGroup;
        public CanvasGroup gameOverCanvasGroup;
        public float fadeDuration = 1f;

        public Image loadingImage;
        public TextMeshProUGUI loadingText;

        protected bool m_IsFading;
    
        const int k_MaxSortingLayer = 32767;

        void Awake ()
        {
            if (Instance != this)
            {
                Destroy (gameObject);
                return;
            }
        
            DontDestroyOnLoad (gameObject);
        }

        protected IEnumerator Fade(float finalAlpha, CanvasGroup canvasGroup)
        {
            m_IsFading = true;
            canvasGroup.blocksRaycasts = true;
            float fadeSpeed = Mathf.Abs(canvasGroup.alpha - finalAlpha) / fadeDuration;
            while (!Mathf.Approximately(canvasGroup.alpha, finalAlpha))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, finalAlpha,
                    fadeSpeed * Time.deltaTime);
                yield return null;
            }
            canvasGroup.alpha = finalAlpha;
            m_IsFading = false;
            canvasGroup.blocksRaycasts = false;
        }

        public static void SetAlpha (float alpha)
        {
            Instance.faderCanvasGroup.alpha = alpha;
        }

        public static IEnumerator FadeSceneIn ()
        {
            CanvasGroup canvasGroup;
            if (Instance.faderCanvasGroup.alpha > 0.1f)
                canvasGroup = Instance.faderCanvasGroup;
            else if (Instance.gameOverCanvasGroup.alpha > 0.1f)
                canvasGroup = Instance.gameOverCanvasGroup;
            else
                canvasGroup = Instance.loadingCanvasGroup;
            
            yield return Instance.StartCoroutine(Instance.Fade(0f, canvasGroup));

            canvasGroup.gameObject.SetActive (false);
            
            
            // [TKSR] 重置黑屏效果默认值(有时剧情中会动态修改这些属性,例如白屏)
            Instance.fadeDuration = 0.5f;
            Instance.loadingImage.color = Color.black;
            Instance.loadingText.text = string.Empty;
        }

        public static IEnumerator FadeSceneOut (FadeType fadeType = FadeType.Black, string strTips = null)
        {
            CanvasGroup canvasGroup;
            switch (fadeType)
            {
                case FadeType.Black:
                    canvasGroup = Instance.faderCanvasGroup;
                    break;
                case FadeType.GameOver:
                    canvasGroup = Instance.gameOverCanvasGroup;
                    break;
                default:
                    canvasGroup = Instance.loadingCanvasGroup;
                    break;
            }
            
            canvasGroup.gameObject.SetActive (true);

            if (!string.IsNullOrEmpty(strTips))
            {
                Instance.loadingText.text = strTips;
            }
            else
            {
                Instance.loadingText.text = string.Empty;
            }
            
            yield return Instance.StartCoroutine(Instance.Fade(1f, canvasGroup));
        }
    }
}