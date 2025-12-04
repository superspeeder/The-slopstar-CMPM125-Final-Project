using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _credits;
    [SerializeField] private GameObject _creditsButton;
    [SerializeField] private GameObject _play;
    [SerializeField] private GameObject _options;
    [SerializeField] private GameObject _quit;
    public void PlayGame()
    {
        SceneManager.LoadScene("LevelScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Credits()
    {
        _credits.SetActive(true);
        _creditsButton.SetActive(false);
        _play.SetActive(false);
        _options.SetActive(false);
        _quit.SetActive(false);
    }

    public void Return()
    {
        _credits.SetActive(false);
        _creditsButton.SetActive(true);
        _play.SetActive(true);
        _options.SetActive(true);
        _quit.SetActive(true);
    }
}
