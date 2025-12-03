using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Configuracion Global")]
    public int lives = 3;
    public int maxHealth = 3;
    public int currentHealth;
    public int score = 0;

    [Header("Estado del Nivel")]
    public int scoreToPassLevel = 100;
    public int totalCollectiblesInScene;
    public int collectiblesCollected;
    public bool hasKey = false;

    // CHECKPOINT Y RESPAWN
    public Vector3 lastCheckpointPos;
    public bool checkpointActivated = false;
    private Vector3 initialLevelPos; // Guardamos donde empieza el nivel por si muere sin checkpoint

    [Header("UI Referencias")]
    public Sprite fullHeart;
    public Sprite emptyHeart;
    private NumberRenderer scoreDisplay;
    private NumberRenderer livesDisplay;
    private Image[] heartImages;

    [Header("Pantallas y Efectos")]
    public GameObject pauseMenuUI;
    public Image fadePanel;
    private bool isPaused = false;

    [Header("Límites de Vuelo")]
    public float alturaMaxima = 2.0f; // Ajusta este valor en el Inspector
    public bool usarLimiteAltura = true;

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
        // Solo la primera vez que se abre el juego reseteamos la salud
        ResetHealth();

        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(DelayedInitialization());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        if (usarLimiteAltura)
        {
            // Si el enemigo sube más de la altura permitida, lo forzamos a bajar
            if (transform.position.y > alturaMaxima)
            {
                transform.position = new Vector3(transform.position.x, alturaMaxima, transform.position.z);
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(isPaused);

        if (isPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    public void UpdateCheckpoint(Vector3 pos)
    {
        lastCheckpointPos = pos;
        checkpointActivated = true;
        Debug.Log("Checkpoint Guardado: " + pos);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Limpiar referencias de UI viejas
        heartImages = null;
        scoreDisplay = null;
        livesDisplay = null;

        // Resetear checkpoint al entrar a un nivel NUEVO
        checkpointActivated = false;

        StartCoroutine(DelayedInitialization());

        if (fadePanel != null) StartCoroutine(FadeIn());
    }

    IEnumerator DelayedInitialization()
    {
        yield return null;
        InitializeLevelData();
    }

    IEnumerator FadeIn()
    {
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            Color c = fadePanel.color;
            for (float alpha = 1f; alpha >= 0; alpha -= Time.deltaTime)
            {
                c.a = alpha;
                fadePanel.color = c;
                yield return null;
            }
            fadePanel.gameObject.SetActive(false);
        }
    }

    void InitializeLevelData()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

        hasKey = false;

        // Contar objetos
        var collectibles = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None);
        totalCollectiblesInScene = (collectibles != null) ? collectibles.Length : 0;
        collectiblesCollected = 0; // Esto se reinicia por nivel para calcular si puedes pasar

        // --- GUARDAR POSICION INICIAL DEL NIVEL ---
        GameObject player = GameObject.FindGameObjectWithTag("Jugador");
        if (player != null)
        {
            initialLevelPos = player.transform.position;
        }

        UpdateAllUI();
    }

    public void CollectObject(int amount)
    {
        score += amount;
        collectiblesCollected++;
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

    public void LoseLife()
    {
        lives--;
        UpdateAllUI();

        if (lives > 0)
        {
            Debug.Log("Vida perdida. Reapareciendo...");

            // AL MORIR: Si reseteamos salud (para poder seguir jugando)
            ResetHealth();

            // MOVER AL JUGADOR (Sin recargar escena)
            GameObject player = GameObject.FindGameObjectWithTag("Jugador");
            if (player != null)
            {
                // Resetear fisicas
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;

                // Elegir destino: Checkpoint o Inicio
                if (checkpointActivated)
                {
                    player.transform.position = lastCheckpointPos;
                    Debug.Log("Respawn en Checkpoint");
                }
                else
                {
                    player.transform.position = initialLevelPos;
                    Debug.Log("Respawn en Inicio del Nivel");
                }
            }
        }
        else
        {
            // Game Over real
            checkpointActivated = false;
            SceneManager.LoadScene("MainMenu");
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
            if (heartImages[i] == null) continue;

            if (i < currentHealth) heartImages[i].sprite = fullHeart;
            else heartImages[i].sprite = emptyHeart;

            heartImages[i].enabled = (i < maxHealth);
        }
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    public void CollectKey() { hasKey = true; }

    public bool CanPassLevel()
    {
        bool basic = collectiblesCollected >= totalCollectiblesInScene && score >= scoreToPassLevel;
        if (SceneManager.GetActiveScene().name == "Level2" && !hasKey) return false;
        return basic;
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}