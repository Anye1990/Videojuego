using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int playerHealth = 3;
    public int score = 0;

    // NUEVAS VARIABLES
    [Header("Condiciones de Victoria")]
    public int scoreToPassLevel = 100; // Puntaje mínimo necesario
    public int totalCollectiblesInScene; // Total de objetos en el nivel
    public int collectiblesCollected;    // Cuántos lleva el jugador

    [Header("UI")]
    public Text healthText;
    public Text scoreText;
    public Text itemsText; // (Opcional) Para mostrar "3/10 objetos"

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Busca todos los CollectibleItem sin ordenarlos y los cuenta
        totalCollectiblesInScene = FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None).Length;

        UpdateUI();
    }

    public void ChangeHealth(int amount)
    {
        playerHealth += amount;
        UpdateUI();

        if (playerHealth <= 0)
        {
            Debug.Log("Game Over");
            // Aquí podrías reiniciar el nivel
        }
    }

    // Modificamos AddScore para que también cuente el objeto recolectado
    public void CollectObject(int amount)
    {
        score += amount;
        collectiblesCollected++; // Sumamos 1 al contador de objetos
        UpdateUI();
    }

    // Esta función valida si el jugador puede pasar
    public bool CanPassLevel()
    {
        // Condición: Haber recogido TODOS los objetos Y tener el puntaje mínimo
        if (collectiblesCollected >= totalCollectiblesInScene && score >= scoreToPassLevel)
        {
            return true;
        }
        else
        {
            Debug.Log($"Faltan requisitos. Objetos: {collectiblesCollected}/{totalCollectiblesInScene}. Puntos: {score}/{scoreToPassLevel}");
            return false;
        }
    }

    void UpdateUI()
    {
        if (healthText) healthText.text = "Vida: " + playerHealth;
        if (scoreText) scoreText.text = "Puntos: " + score;
        // (Opcional)
        if (itemsText) itemsText.text = "Items: " + collectiblesCollected + "/" + totalCollectiblesInScene;
    }
}