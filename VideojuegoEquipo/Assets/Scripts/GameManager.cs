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

    // ESTAS VARIABLES YA NO SE ARRASTRAN EN EL GAME MANAGER
    // Se llenan automáticamente cuando el HUDController se conecta
    private NumberRenderer scoreDisplay;
    private NumberRenderer livesDisplay;
    private Image[] heartImages;

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

    void Start()
    {
        // Solo inicializamos lógica de datos, la UI espera al HUDController
        // totalCollectiblesInScene se debe buscar en cada nivel nuevo
        // Para eso usaremos el evento de carga de escena abajo
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeLevelData();
    }

    // Esta función se llama sola cada vez que carga un nivel
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeLevelData();
    }

    void InitializeLevelData()
    {
        totalCollectiblesInScene = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None).Length;
        collectiblesCollected = 0;
        // Si quieres resetear la salud al cambiar de nivel:
        ResetHealth();
    }

    // --- EL ENCHUFE MÁGICO ---
    public void RegisterHUD(HUDController hud)
    {
        // Recibimos las nuevas conexiones del Canvas nuevo
        scoreDisplay = hud.scoreDisplay;
        livesDisplay = hud.livesDisplay;
        heartImages = hud.heartImages;

        // Actualizamos todo inmediatamente para que se vea
        UpdateAllUI();
    }
    // -------------------------

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
        return collectiblesCollected >= totalCollectiblesInScene && score >= scoreToPassLevel;
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
        if (scoreDisplay != null) scoreDisplay.UpdateNumber(score);
        if (livesDisplay != null) livesDisplay.UpdateNumber(lives);
        UpdateHeartsUI();
    }

    void UpdateHeartsUI()
    {
        if (heartImages == null) return; // Protección por si no hay HUD

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth) heartImages[i].sprite = fullHeart;
            else heartImages[i].sprite = emptyHeart;

            if (i < maxHealth) heartImages[i].enabled = true;
            else heartImages[i].enabled = false;
        }
    }
}