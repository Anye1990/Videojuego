using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Configuración de Juego")]
    public int lives = 3;
    public int maxHealth = 3;
    public int currentHealth;
    public int score = 0;

    [Header("Objetos y Nivel")]
    public int scoreToPassLevel = 100;
    public int totalCollectiblesInScene;
    public int collectiblesCollected;

    [Header("Recursos (Sprites)")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private NumberRenderer scoreDisplay;
    private NumberRenderer livesDisplay;
    private Image[] heartImages;
    public bool hasKey = false;


    void Start()
    {
        // --- IMPORTANTE: Resetear la llave al iniciar cualquier nivel ---
        hasKey = false;

        ResetHealth();
        totalCollectiblesInScene = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None).Length;
        UpdateAllUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeLevelData();
    }

    public void CollectKey()
    {
        hasKey = true;
        Debug.Log("¡Llave conseguida!");
        // Aquí podrías añadir código para mostrar un icono de llave en la pantalla
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeLevelData();
    }

    void InitializeLevelData()
    {
        score = 0;
        collectiblesCollected = 0;

        totalCollectiblesInScene = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None).Length;

        ResetHealth();

        UpdateAllUI();
    }

    public void RegisterHUD(HUDController hud)
    {
        // Recibimos las nuevas conexiones del Canvas nuevo
        scoreDisplay = hud.scoreDisplay;
        livesDisplay = hud.livesDisplay;
        heartImages = hud.heartImages;

        // Actualizamos todo inmediatamente para que se vea
        UpdateAllUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            LoseLife();
        }
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    public bool CanPassLevel()
    {
        // 1. Verificar requisitos básicos (Objetos y Puntos)
        bool basicRequirements = collectiblesCollected >= totalCollectiblesInScene && score >= scoreToPassLevel;

        // 2. Verificar requisito especial SOLO para el Nivel 2
        // Asegúrate de que tu escena se llame EXACTAMENTE "Level2" (respeta mayúsculas)
        if (SceneManager.GetActiveScene().name == "Level2")
        {
            if (!hasKey)
            {
                Debug.Log("¡Necesitas la LLAVE para salir de este nivel!");
                return false; // Si es nivel 2 y no tiene llave, no pasa.
            }
        }

        // 3. Si cumple lo básico (y la llave si era el Nivel 2), pasa.
        if (basicRequirements)
        {
            return true;
        }
        else
        {
            Debug.Log("Faltan objetos o puntos.");
            return false;
        }
    }

    public void CollectObject(int amount)
    {
        score += amount;
        collectiblesCollected++;
        UpdateAllUI();
    }

    public void LoseLife()
    {
        lives--;
        UpdateAllUI();

        if (lives > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            // No necesitamos llamar ResetHealth aquí, OnSceneLoaded lo hará
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
            // Al ir al menú, sí destruimos el GM para que se resetee todo el juego
            Destroy(gameObject);
        }
    }
    void UpdateAllUI()
    {
        if (scoreDisplay != null) scoreDisplay.UpdateNumber(score); // Actualiza Puntos
        if (livesDisplay != null) livesDisplay.UpdateNumber(lives); // Actualiza Vidas
        UpdateHeartsUI();
    }

    void UpdateHeartsUI()
    {
        // 1. Si la lista no existe, salimos
        if (heartImages == null) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;

            // Si la imagen existe, procedemos con la lógica normal
            if (i < currentHealth)
                heartImages[i].sprite = fullHeart;
            else
                heartImages[i].sprite = emptyHeart;

            if (i < maxHealth)
                heartImages[i].enabled = true;
            else
                heartImages[i].enabled = false;
        }
    }
}