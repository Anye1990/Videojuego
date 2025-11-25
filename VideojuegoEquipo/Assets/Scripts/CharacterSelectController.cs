using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectController : MonoBehaviour
{
    public AnimatorOverrideController[] characterControllers; 
    void Start()
    {
        int index = PlayerPrefs.GetInt("SelectedCharacter");
        GetComponent<Animator>().runtimeAnimatorController = characterControllers[index];
    }
    public void SelectCharacter(int characterIndex)
    {
        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
        SceneManager.LoadScene("Level1");
    }
}