using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Configuración de Juego")]
    public int lives = 3;          // Vidas totales (intentos)
    public int maxHealth = 3;      // Corazones máximos por vida
    public int currentHealth;      // Salud actual
    public int score = 0;

    [Header("Objetos y Nivel")]
    public int scoreToPassLevel = 100;
    public int totalCollectiblesInScene;
    public int collectiblesCollected;

    [Header("Referencias UI")]
    // Usamos NumberRenderer para los números bonitos. 
    // Si te da error aquí, asegura que creaste el script "NumberRenderer" que te di antes.
    public NumberRenderer scoreDisplay;
    public NumberRenderer livesDisplay;

    // Imágenes de los corazones (H1, H2, H3)
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Awake()
    {
        // Configuración del Singleton (para que solo haya un GameManager)
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
        // Al iniciar, reseteamos salud y contamos objetos
        ResetHealth();

        // Busca todos los objetos recolectables (Script CollectibleItem)
        totalCollectiblesInScene = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None).Length;

        UpdateAllUI();
    }

    // --- FUNCIONES QUE TE DABAN ERROR ---

    // 1. Función para recibir daño (llamada desde el Player o Enemigos)
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            LoseLife(); // Si se acaban los corazones, perdemos una vida
        }
    }

    // 2. Función interna para reiniciar corazones (llamada en Start y al perder vida)
    void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    // 3. Función para verificar si se puede pasar de nivel (llamada desde la Meta)
    public bool CanPassLevel()
    {
        if (collectiblesCollected >= totalCollectiblesInScene && score >= scoreToPassLevel)
        {
            return true;
        }
        else
        {
            Debug.Log("Faltan objetos o puntos para pasar.");
            return false;
        }
    }

    // --- OTRAS FUNCIONES NECESARIAS ---

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
            Debug.Log("Reiniciando nivel...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            ResetHealth();
        }
        else
        {
            Debug.Log("GAME OVER");
            SceneManager.LoadScene("MainMenu"); // Asegúrate de tener una escena llamada MainMenu
            Destroy(gameObject);
        }
    }

    // --- ACTUALIZACIÓN VISUAL (HUD) ---

    void UpdateAllUI()
    {
        // Actualiza los números (sprites)
        if (scoreDisplay != null) scoreDisplay.UpdateNumber(score);
        if (livesDisplay != null) livesDisplay.UpdateNumber(lives);

        // Actualiza los corazones
        UpdateHeartsUI();
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            // Si el índice es menor que la salud actual, dibuja corazón lleno
            if (i < currentHealth)
            {
                heartImages[i].sprite = fullHeart;
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
            }

            // Solo mostramos la cantidad de corazones máxima permitida
            if (i < maxHealth)
                heartImages[i].enabled = true;
            else
                heartImages[i].enabled = false;
        }
    }
}