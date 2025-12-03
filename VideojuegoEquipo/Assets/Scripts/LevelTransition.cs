using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    public static LevelTransition instance;

    [Header("Referencias")]
    public CanvasGroup fadeCanvasGroup;

    [Header("Configuración")]
    public float fadeDuration = 1.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // Desvincular de padres para asegurar que DontDestroyOnLoad funcione bien
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Ejecutar el Fade In normal
        StartCoroutine(FadeIn());
        StartCoroutine(FailsafeSecurity());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        if (this == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeIn()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1;
            fadeCanvasGroup.blocksRaycasts = true; // Bloquea clics

            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                yield return null;
            }

            fadeCanvasGroup.alpha = 0;
            fadeCanvasGroup.blocksRaycasts = false; // Desbloquea clics
        }
    }

    IEnumerator FadeOut(string sceneName)
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;

            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1;
        }

        SceneManager.LoadScene(sceneName);
    }

    // --- SISTEMA ANTI-PANTALLA NEGRA ---
    IEnumerator FailsafeSecurity()
    {
        // Esperamos 2 segundos reales (sin importar lag o pausas)
        yield return new WaitForSecondsRealtime(2f);

        // Si después de 2 segundos la pantalla sigue oscura...
        if (fadeCanvasGroup != null && fadeCanvasGroup.alpha > 0.05f)
        {
            Debug.LogWarning("SISTEMA DE SEGURIDAD: Se detectó pantalla negra congelada. Forzando apertura.");

            // Forzamos transparencia total
            fadeCanvasGroup.alpha = 0;

            // Importante: Permitir volver a jugar
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }
}