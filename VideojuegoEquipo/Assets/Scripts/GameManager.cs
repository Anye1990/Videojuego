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
    public bool hasKey = false;

    [Header("Recursos (Sprites)")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Sistema de Checkpoint")]
    public Vector3 lastCheckPointPos;
    public bool checkpointActive = false;

    // --- CAMBIO 1: Variable para saber si debemos curar al cargar ---
    private bool restoreHealthOnLoad = false;

    private NumberRenderer scoreDisplay;
    private NumberRenderer livesDisplay;
    private Image[] heartImages;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        hasKey = false;
        // Al iniciar el juego desde cero, sí empezamos con salud llena
        ResetHealth();

        InitializeLevelData();
        UpdateAllUI();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Limpiar referencias viejas
        heartImages = null;
        scoreDisplay = null;
        livesDisplay = null;

        // 2. Inicializar datos del nivel
        InitializeLevelData();

        // --- CAMBIO 2: Solo reseteamos salud si venimos de una muerte ---
        if (restoreHealthOnLoad)
        {
            ResetHealth();
            restoreHealthOnLoad = false; // Apagamos la bandera
        }
        // Si no, mantenemos la salud que traíamos del nivel anterior

        // 3. Lógica de Checkpoint
        if (checkpointActive)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Jugador");
            if (player != null)
            {
                player.transform.position = lastCheckPointPos + (Vector3.up * 1.5f);

                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }

            GameObject msgPanel = GameObject.Find("MessagePanel");
            if (msgPanel != null) msgPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    void InitializeLevelData()
    {
        totalCollectiblesInScene = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None).Length;
        collectiblesCollected = 0;
        hasKey = false;

        // --- CAMBIO IMPORTANTE: ---
        // Quitamos ResetHealth() de aquí. 
        // Ya no curamos automáticamente al cambiar de nivel.
    }

    public void LoseLife()
    {
        lives--;
        UpdateAllUI();

        if (lives > 0)
        {
            // --- CAMBIO 3: Avisamos que al recargar, debemos tener vida llena ---
            restoreHealthOnLoad = true;

            // Usamos tu transición si existe
            if (LevelTransition.instance != null)
                LevelTransition.instance.LoadScene(SceneManager.GetActiveScene().name);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            // Game Over
            ResetCheckpointData();
            if (LevelTransition.instance != null)
                LevelTransition.instance.LoadScene("MainMenu");
            else
                SceneManager.LoadScene("MainMenu");

            Destroy(gameObject, 1f);
        }
    }

    public void RegisterHUD(HUDController hud)
    {
        scoreDisplay = hud.scoreDisplay;
        livesDisplay = hud.livesDisplay;
        heartImages = hud.heartImages;
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

    // ... Resto de tus métodos (ActivateCheckpoint, CanPassLevel, CollectObject, etc.) siguen igual ...

    public void ActivateCheckpoint(Vector3 position)
    {
        lastCheckPointPos = position;
        checkpointActive = true;
        Debug.Log("Checkpoint Activado en: " + position);
    }

    public void ResetCheckpointData()
    {
        checkpointActive = false;
        lastCheckPointPos = Vector3.zero;
    }

    public bool CanPassLevel()
    {
        bool basicRequirements = collectiblesCollected >= totalCollectiblesInScene && score >= scoreToPassLevel;
        if (SceneManager.GetActiveScene().name == "Level2")
        {
            if (!hasKey) return false;
        }
        return basicRequirements;
    }

    public void CollectObject(int amount)
    {
        score += amount;
        collectiblesCollected++;
        UpdateAllUI();
    }

    public void CollectKey()
    {
        hasKey = true;
    }

    void UpdateAllUI()
    {
        if (scoreDisplay != null) scoreDisplay.UpdateNumber(score);
        if (livesDisplay != null) livesDisplay.UpdateNumber(lives);
        UpdateHeartsUI();
    }

    void UpdateHeartsUI()
    {
        if (heartImages == null) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) return;

            if (i < currentHealth) heartImages[i].sprite = fullHeart;
            else heartImages[i].sprite = emptyHeart;

            if (i < maxHealth) heartImages[i].enabled = true;
            else heartImages[i].enabled = false;
        }
    }
}