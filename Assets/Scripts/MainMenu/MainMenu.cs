using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void LoadBotGame()
    {
        GameManager.isPlayWithBot = true;
        SceneManager.LoadScene(1); // loading Game Scene
    }

    public void LoadMultiPlayerGame()
    {
        GameManager.isPlayWithBot = false;
        SceneManager.LoadScene(1); // loading Game Scene
    }
}