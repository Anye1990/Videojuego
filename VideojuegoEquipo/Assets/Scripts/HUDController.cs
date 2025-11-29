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
        
        // Ocultamos el panel del boss al inicio (opcional)
        if(bossPanel != null) bossPanel.SetActive(false);
    }

    // Esta función será llamada por el Boss
    public void UpdateBossHealth(int currentHealth, int maxHealth)
    {
        if (bossPanel != null) bossPanel.SetActive(true); // Aseguramos que se vea

        for (int i = 0; i < bossHeartImages.Length; i++)
        {
            // Lógica de sprites: Lleno o Vacío
            if (i < currentHealth)
            {
                bossHeartImages[i].sprite = bossFullHeart;
            }
            else
            {
                bossHeartImages[i].sprite = bossEmptyHeart;
            }

            // Lógica de activado: Solo mostramos hasta el máximo de vida (por si el boss tiene menos de 5)
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