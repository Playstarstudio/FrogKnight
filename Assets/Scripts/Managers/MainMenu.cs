using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("Sample");
    }
    public void ExitButton()
    {
        Application.Quit();
    }

}
