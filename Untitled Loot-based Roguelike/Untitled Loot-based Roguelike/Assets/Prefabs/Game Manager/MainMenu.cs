using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Scene gameLevel;

    public AudioClip clickAudio;
    public AudioSource source;

    public void PlayGame()
    {
        LevelInfo.level = 0;
        LevelInfo.numRooms = 10;
        LevelInfo.gridSize = 3;
        SceneManager.LoadScene("Play");
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void showTutorial()
    {
        gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Tutorial Text").gameObject.SetActive(true);
    }

    public void closeTutorial()
    {
        GameObject.Find("Canvas").transform.Find("Tutorial Text").gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void click()
    {
        source.PlayOneShot(clickAudio, 1f);
    }

    public void quit()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
