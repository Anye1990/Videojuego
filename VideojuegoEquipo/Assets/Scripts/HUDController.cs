using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Arrastra aquí los objetos del Canvas")]
    public NumberRenderer scoreDisplay;
    public NumberRenderer livesDisplay;
    public Image[] heartImages; // H1, H2, H3

    void Start()
    {
        // Al nacer, buscamos al GameManager y nos "enchufamos" a él
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterHUD(this);
        }
    }
}