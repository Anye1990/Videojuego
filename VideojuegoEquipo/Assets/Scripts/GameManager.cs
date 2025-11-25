using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int playerHealth = 3;
    public int score = 0;

    public Text healthText;
    public Text scoreText;

    void Awake()
    {
        instance = this;
        UpdateUI();
    }

    public void ChangeHealth(int amount)
    {
        playerHealth += amount;
        UpdateUI();

        if (playerHealth <= 0)
        {
            Debug.Log("Game Over");
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        healthText.text = "Vida: " + playerHealth;
        scoreText.text = "Puntos: " + score;
    }
}