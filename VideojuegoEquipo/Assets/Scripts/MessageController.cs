using UnityEngine;
using TMPro; // Si usas TextMeshPro
using UnityEngine.SceneManagement;

public class MessageController : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject messagePanel;      
    public TextMeshProUGUI messageText; 

    [Header("Configuración del Panel")]
    public bool esPanelInicio = false;   
    public bool esPanelPausa = false;    

    // Estado interno
    private bool estaPausado = false;

    void Start()
    {
        // Lógica para el PANEL DE INICIO
        if (esPanelInicio)
        {
        }
        // Lógica para el PANEL DE PAUSA
        else if (esPanelPausa)
        {
            // Nos aseguramos que arranque apagado
            if (messagePanel != null) messagePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (esPanelPausa)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (estaPausado)
                {
                    Continuar(); // Si ya está pausado, quita la pausa
                }
                else
                {
                    // Solo pausa si el tiempo corre (para evitar pausar encima del menú de inicio)
                    if (Time.timeScale > 0)
                    {
                        ShowMessage("PAUSA");
                    }
                }
            }
        }
    }

    public void ShowMessage(string texto)
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
            if (messageText != null) messageText.text = texto;

            Time.timeScale = 0f; // Congelar el tiempo
            estaPausado = true;
        }
    }

    public void Continuar()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false); // Ocultar panel
            Time.timeScale = 1f;           // Reanudar tiempo
            estaPausado = false;
        }
    }
}