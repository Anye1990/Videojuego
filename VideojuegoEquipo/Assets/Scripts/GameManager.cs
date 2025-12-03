using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // Necesario para Corrutinas

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

    // Variable para saber si debemos curar al cargar
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
        // Inicialización para la primera vez que abres el juego
        Time.timeScale = 1f;
        hasKey = false;
        ResetHealth();
        InitializeLevelData();
        UpdateAllUI();
    }

    // Se llama automáticamente cada vez que cambia o recarga la escena
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Iniciamos la espera de seguridad para evitar el congelamiento
        StartCoroutine(SecureLevelLoad());
    }

    // --- ESTA ES LA SOLUCIÓN AL CONGELAMIENTO ---
    IEnumerator SecureLevelLoad()
    {
        // 1. Esperar un frame. Esto permite que Unity destruya lo viejo y cree lo nuevo.
        yield return null;

        // 2. Asegurarnos de que el tiempo corre
        Time.timeScale = 1f;

        // 3. Limpiar referencias viejas de UI
        heartImages = null;
        scoreDisplay = null;
        livesDisplay = null;

        // 4. Intentar buscar el HUD nuevo automáticamente
        HUDController hud = FindAnyObjectByType<HUDController>();
        if (hud != null)
        {
            RegisterHUD(hud);
        }

        // 5. Inicializar datos
        InitializeLevelData();

        // 6. Restaurar Salud si venimos de morir
        if (restoreHealthOnLoad)
        {
            ResetHealth();
            restoreHealthOnLoad = false;
        }

        // 7. Mover al Checkpoint (Ahora es seguro porque esperamos un frame)
        if (checkpointActive)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Jugador");
            if (player != null)
            {
                // Movemos al jugador
                player.transform.position = lastCheckPointPos + (Vector3.up * 0.5f); // Un poco arriba para no atascarse

                // Frenamos cualquier inercia vieja
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero; // Unity 6
                    // rb.velocity = Vector2.zero; // Si usas Unity viejo, descomenta esto y comenta la línea de arriba
                }
            }

            GameObject msgPanel = GameObject.Find("MessagePanel");
            if (msgPanel != null) msgPanel.SetActive(false);
        }

        // 8. Actualizar UI final
        UpdateAllUI();
    }

    void InitializeLevelData()
    {
        totalCollectiblesInScene = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None).Length;
        collectiblesCollected = 0;
        hasKey = false;
    }

    public void LoseLife()
    {
        lives--;
        UpdateAllUI();

        if (lives > 0)
        {
            restoreHealthOnLoad = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            // Game Over - Reseteamos todo
            ResetCheckpointData();
            SceneManager.LoadScene("MainMenu");
            // Destruir este GameManager para que al volver a jugar se cree uno limpio
            Destroy(gameObject);
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
        // Protección extrema contra errores nulos que congelan el juego
        if (heartImages == null || heartImages.Length == 0) return;

        // Si el primer corazón ha sido destruido (porque cambiamos de escena), salimos
        if (heartImages[0] == null) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                if (i < currentHealth) heartImages[i].sprite = fullHeart;
                else heartImages[i].sprite = emptyHeart;

                if (i < maxHealth) heartImages[i].enabled = true;
                else heartImages[i].enabled = false;
            }
        }
    }
}