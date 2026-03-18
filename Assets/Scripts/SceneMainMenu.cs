using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMainMenu : MonoBehaviour
{
    public Text bestDistanceText;
    public Text maxCoinsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bestDistanceText.text = "Best Running Distance Score: " + PlayerPrefs.GetInt("highscoreD") + "M";
        maxCoinsText.text = "Max. Coin Collected: " + PlayerPrefs.GetInt("highscoreC");
    }

    public void ToGame()
    {
       SceneManager.LoadScene("TempleRun");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}