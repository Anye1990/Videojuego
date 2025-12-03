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
            transform.SetParent(null); // Desvincular de padres para poder no destruirlo
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Este método se ejecuta AUTOMÁTICAMENTE cada vez que carga un nivel (o reinicias)
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Forzamos el Fade In para desbloquear el juego
        StartCoroutine(FadeIn());
    }

    private void OnDestroy()
    {
        // Importante: Si este objeto se destruye, dejamos de escuchar el evento para evitar errores
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        // Protección por si el objeto fue destruido
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
            fadeCanvasGroup.blocksRaycasts = true; // Bloquea

            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                yield return null;
            }

            fadeCanvasGroup.alpha = 0;

            fadeCanvasGroup.blocksRaycasts = false;
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
                timer += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                yield return null;
            }

            fadeCanvasGroup.alpha = 1;
        }

        SceneManager.LoadScene(sceneName);
    }
}