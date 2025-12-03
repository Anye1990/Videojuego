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
    public Vector3 lastCheckPointPos; // Donde reapareceremos
    public bool checkpointActive = false; // Si hemos tocado un checkpoint

    private NumberRenderer scoreDisplay;
    private NumberRenderer livesDisplay;
    private Image[] heartImages;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Suscribirse al evento de carga
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Inicialización normal
        InitializeLevelData();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ... (código de limpiar UI que ya tienes) ...

        if (checkpointActive)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Jugador");
            if (player != null)
            {
                player.transform.position = lastCheckPointPos + (Vector3.up * 1.5f);

                // Aseguramos que las físicas y el tiempo estén activos
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero; // Reseteamos velocidad
            }

            // ... (código de desactivar panel) ...
        }

        // --- SEGURO ANTI-CONGELAMIENTO ---
        // A veces el tiempo se queda en 0 si hubo una pausa previa. Lo forzamos a 1.
        Time.timeScale = 1f;
    }

    void InitializeLevelData()
    {
        hasKey = false;
        collectiblesCollected = 0;
        totalCollectiblesInScene = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None).Length;

        // Reiniciamos la salud, pero NO las vidas ni el checkpoint
        ResetHealth();
        UpdateAllUI();
    }

    // --- NUEVO MÉTODO PARA ACTIVAR CHECKPOINT ---
    public void ActivateCheckpoint(Vector3 position)
    {
        lastCheckPointPos = position;
        checkpointActive = true;
        Debug.Log("Checkpoint Activado en: " + position);
    }

    // --- MODIFICACIÓN IMPORTANTE: Resetear Checkpoints al cambiar de nivel real ---
    // Llama a esto desde LevelExit.cs antes de cargar el siguiente nivel
    public void ResetCheckpointData()
    {
        checkpointActive = false;
        lastCheckPointPos = Vector3.zero;
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

    public void LoseLife()
    {
        lives--;
        UpdateAllUI();

        if (lives > 0)
        {
            // Recargar la escena actual (OnSceneLoaded se encargará de mover al PJ al checkpoint)
            if (LevelTransition.instance != null)
                LevelTransition.instance.LoadScene(SceneManager.GetActiveScene().name);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            // Game Over real: Reiniciamos todo
            ResetCheckpointData(); // Borramos checkpoint para que empiece de cero
            if (LevelTransition.instance != null)
                LevelTransition.instance.LoadScene("MainMenu");
            else
                SceneManager.LoadScene("MainMenu");

            Destroy(gameObject, 1f);
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
        // Si no hay array asignado, no hacemos nada
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