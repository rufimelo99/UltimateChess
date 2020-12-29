using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame1vs1()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }
    public void PlayGamevsCPU()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    //in menu 2 (index 1)
    public void PlayGame1vsCPU()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }
    public void PlayGameCPUvs1()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 4);
    }
    //in menu
    public void about()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    public void returnFromMenu2()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void returnFromAbout()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }


    public void returnFromGames()
    {
        SceneManager.LoadScene(0);
    }

}
