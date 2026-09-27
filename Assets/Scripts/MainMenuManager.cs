using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    [Header("Game Scene")]
    public string gameSceneName = "SampleScene";
    [Header("About Panel")]
    public GameObject aboutPanel;
    void Start()
    {
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(false);
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
    public void QuitGame()
    {
        Debug.Log("Quitting SpineWood...");
        Application.Quit();
    }
    public void OpenAboutPanel()
    {
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(true);
        }
    }
    public void CloseAboutPanel()
    {
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(false);
        }
    }
}