using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectController : MonoBehaviour
{
    [Header("Configuración")]
    // Arrastra aquí al personaje único que está en el podio
    public Animator displayCharacterAnimator;

    // Arrastra aquí los MISMOS controladores que pusiste en tu PlayerController
    // (Beige, Green, Pink, etc.)
    public RuntimeAnimatorController[] characterData;

    private int selectedIndex = 0;

    void Start()
    {
        // Recuperar la última elección o empezar en 0
        selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        UpdateCharacterShowcase();
    }

    public void NextCharacter()
    {
        selectedIndex++;
        if (selectedIndex >= characterData.Length)
        {
            selectedIndex = 0;
        }
        UpdateCharacterShowcase();
    }

    public void PreviousCharacter()
    {
        selectedIndex--;
        if (selectedIndex < 0)
        {
            selectedIndex = characterData.Length - 1;
        }
        UpdateCharacterShowcase();
    }

    void UpdateCharacterShowcase()
    {
        // Aquí ocurre la magia: cambiamos el "cerebro" de animación del maniquí
        if (characterData.Length > 0 && displayCharacterAnimator != null)
        {
            displayCharacterAnimator.runtimeAnimatorController = characterData[selectedIndex];
        }
    }

    public void PlayGame()
    {
        PlayerPrefs.SetInt("SelectedCharacter", selectedIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Level1");
    }
}
