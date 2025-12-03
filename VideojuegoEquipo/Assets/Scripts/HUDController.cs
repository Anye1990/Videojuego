using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Jugador UI")]
    public NumberRenderer scoreDisplay;
    public NumberRenderer livesDisplay;
    public Image[] heartImages;

    [Header("Boss UI")]
    public GameObject bossPanel;
    public Image[] bossHeartImages;
    public Sprite bossFullHeart;
    public Sprite bossEmptyHeart;

    void Awake()
    {
        // 1. Ocultar el panel del boss por defecto al iniciar cualquier nivel
        if (bossPanel != null)
        {
            bossPanel.SetActive(false);
        }
    }
    void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterHUD(this);
        }
    }

    // El Boss llamará a esto en su Start(), lo que volverá a activar el panel
    public void UpdateBossHealth(int currentHealth, int maxHealth)
    {
        if (bossPanel == null || bossHeartImages == null) return;

        // 2. Si el Boss llama a esta función, encendemos el panel
        bossPanel.SetActive(true);

        for (int i = 0; i < bossHeartImages.Length; i++)
        {
            if (bossHeartImages[i] == null) continue;

            if (i < currentHealth) bossHeartImages[i].sprite = bossFullHeart;
            else bossHeartImages[i].sprite = bossEmptyHeart;

            if (i < maxHealth) bossHeartImages[i].enabled = true;
            else bossHeartImages[i].enabled = false;
        }
    }

    public void HideBossUI()
    {
        if (bossPanel != null) bossPanel.SetActive(false);
    }
}