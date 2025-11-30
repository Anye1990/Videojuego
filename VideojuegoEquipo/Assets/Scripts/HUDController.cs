using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Jugador UI")]
    public NumberRenderer scoreDisplay;
    public NumberRenderer livesDisplay;
    public Image[] heartImages; // H1, H2, H3

    [Header("Boss UI")]
    public GameObject bossPanel;    // Para ocultar/mostrar la vida del boss
    public Image[] bossHeartImages; // Arrastra aquí las 5 imágenes de corazones del boss
    public Sprite bossFullHeart;    // Sprite corazón lleno (puede ser el mismo del jugador)
    public Sprite bossEmptyHeart;   // Sprite corazón vacío

    void Start()
    {
        // Al nacer, buscamos al GameManager y nos "enchufamos" a él
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterHUD(this);
        }
    }

    public void UpdateBossHealth(int currentHealth, int maxHealth)
    {
        // 1. Comprobación de seguridad: Si no hay panel o imágenes, no hacemos nada
        if (bossPanel == null || bossHeartImages == null)
        {
            Debug.LogWarning("¡Falta asignar el BossPanel o las imágenes de corazones en el HUDController!");
            return;
        }

        bossPanel.SetActive(true);

        for (int i = 0; i < bossHeartImages.Length; i++)
        {
            // 2. Comprobación extra: Si un hueco del array está vacío
            if (bossHeartImages[i] == null) continue;

            if (i < currentHealth)
            {
                bossHeartImages[i].sprite = bossFullHeart;
            }
            else
            {
                bossHeartImages[i].sprite = bossEmptyHeart;
            }

            if (i < maxHealth)
            {
                bossHeartImages[i].enabled = true;
            }
            else
            {
                bossHeartImages[i].enabled = false;
            }
        }
    }

    public void HideBossUI()
    {
        if(bossPanel != null) bossPanel.SetActive(false);
    }
}