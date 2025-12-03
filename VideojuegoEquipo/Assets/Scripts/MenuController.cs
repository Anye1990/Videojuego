using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        LevelTransition.instance.LoadScene("CharacterSelect");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}