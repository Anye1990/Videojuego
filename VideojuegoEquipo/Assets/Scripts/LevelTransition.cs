using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    public static LevelTransition instance;

    [Header("Referencias")]
    public CanvasGroup fadeCanvasGroup; // Arrastra aquí tu objeto Panel/Fade con el CanvasGroup

    [Header("Configuración")]
    public float fadeDuration = 1.0f; // Cuánto tarda en desvanecerse

    private void Awake()
    {
        // Configuración Singleton simple para llamarlo desde otros scripts
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Al iniciar cualquier escena, hacemos un Fade In (de negro a transparente)
        StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log("Iniciando Fade Out hacia: " + sceneName); // <--- AÑADE ESTO
        StartCoroutine(FadeOut(sceneName));
    }
    IEnumerator FadeIn()
    {
        fadeCanvasGroup.alpha = 1; // Empezamos totalmente negro
        fadeCanvasGroup.blocksRaycasts = true; // Bloqueamos clics mientras carga

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Lerp va de 1 a 0
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0;
        fadeCanvasGroup.blocksRaycasts = false; // Permitimos jugar
    }

    // De Transparente a Negro (Al salir del nivel)
    IEnumerator FadeOut(string sceneName)
    {
        fadeCanvasGroup.blocksRaycasts = true; // Bloqueamos clics para que no se mueva el pj
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Lerp va de 0 a 1
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1;
        // Una vez la pantalla está negra, cargamos la escena
        SceneManager.LoadScene(sceneName);
    }
}